import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useLocation, useNavigate, useParams, useSearchParams } from 'react-router'

import { fetchEsimPackageDetail, updateEsimPackage } from '@/apis/esimPackagesApi'
import { fetchProductDetail } from '@/apis/productsApi'
import { fetchVariantFeatures } from '@/apis/productVariantFeaturesApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import { getEsimStepByTab } from '@/features/products/esim-wizard/esimWizardSteps'
import { mapPackageToForm, toPackagePayload } from '@/features/products/esim-wizard/mapPackageForm'
import type { EsimWizardSummary, EsimWizardTab } from '@/features/products/esim-wizard/types'
import { useEsimWizardLoader } from '@/features/products/esim-wizard/useEsimWizardLoader'

const TAB_KEYS: EsimWizardTab[] = ['variants', 'prices', 'packages', 'carriers', 'features', 'review']

function parseTab(value: string | null): EsimWizardTab {
  if (value && TAB_KEYS.includes(value as EsimWizardTab)) return value as EsimWizardTab
  return 'variants'
}

function isNewWizardRoute(routeId: string | undefined, pathname: string): boolean {
  if (routeId === 'new') return true
  return pathname.replace(/\/$/, '').endsWith('/wizard/new')
}

export function getEsimWizardFormId(tab: EsimWizardTab): string | undefined {
  if (tab === 'variants') return 'esim-wizard-variant-form'
  if (tab === 'prices') return 'esim-wizard-price-form'
  if (tab === 'packages') return 'esim-wizard-package-form'
  return undefined
}

export function formatEsimWizardPrice(value: number | undefined, currency: string | undefined): string {
  if (!value) return 'Chưa có giá'
  return `${value.toLocaleString('vi-VN')} ${currency ?? 'VND'}`
}

export function useEsimWizardPage() {
  const { wizardId: routeWizardId } = useParams()
  const location = useLocation()
  const [searchParams, setSearchParams] = useSearchParams()
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()

  const isNew = isNewWizardRoute(routeWizardId, location.pathname)
  const variantId = isNew ? null : (routeWizardId ?? null)
  const productIdParam = searchParams.get('productId')
  const packageIdParam = searchParams.get('packageId')
  const activeTab = parseTab(searchParams.get('tab'))

  const buildWizardParams = useCallback(
    (tab: EsimWizardTab, overrides?: { packageId?: string }) => {
      const params: Record<string, string> = { tab }
      if (productIdParam) params.productId = productIdParam
      const pkgId = overrides?.packageId ?? packageIdParam
      if (pkgId) params.packageId = pkgId
      return params
    },
    [productIdParam, packageIdParam],
  )

  const currentStep = getEsimStepByTab(activeTab)
  const [isSaving, setIsSaving] = useState(false)
  const featuresSaveRef = useRef<(() => Promise<boolean>) | null>(null)

  const onBootstrapError = useCallback(() => {
    showNotification({
      title: 'Lỗi',
      message: 'Không tải được dữ liệu tham chiếu',
      variant: 'danger',
      delay: 4000,
    })
  }, [showNotification])

  const wizard = useEsimWizardLoader({
    isNew,
    variantId,
    productIdParam,
    packageIdParam,
    activeTab,
    onBootstrapError,
  })

  useEffect(() => {
    if (isNew && activeTab !== 'variants') {
      setSearchParams({ tab: 'variants' }, { replace: true })
    }
  }, [isNew, activeTab, setSearchParams])

  const canAccessSubTabs = Boolean(variantId)
  const hasPackage = Boolean(wizard.packageForm?.id || wizard.esimPackage?.id)

  const setActiveTab = useCallback(
    (tab: EsimWizardTab) => {
      if (tab !== 'variants' && !canAccessSubTabs) return
      setSearchParams(buildWizardParams(tab))
    },
    [canAccessSubTabs, buildWizardParams, setSearchParams],
  )

  const goNextTab = useCallback(
    (tab: EsimWizardTab, overrides?: { packageId?: string }) => {
      setSearchParams(buildWizardParams(tab, overrides))
    },
    [buildWizardParams, setSearchParams],
  )

  const handleVariantSaved = useCallback(
    async (id: string, pId: string) => {
      const pName = wizard.productOptions.find((o) => o.value === pId)?.label ?? ''
      wizard.setProductName(pName)
      const product = await fetchProductDetail(pId)
      if (product?.countryId) wizard.setDefaultCountryId(product.countryId)
      showNotification({ title: 'Thành công', message: 'Đã lưu biến thể', variant: 'success', delay: 2000 })
      navigate(`/products/esim/wizard/${id}?tab=prices&productId=${pId}`, { replace: true })
    },
    [wizard, showNotification, navigate],
  )

  const handlePriceSaved = useCallback(async () => {
    if (!variantId) return
    wizard.invalidateTab('prices')
    await wizard.refreshPrice()
    showNotification({ title: 'Thành công', message: 'Đã lưu giá', variant: 'success', delay: 2000 })
    goNextTab('packages')
  }, [variantId, wizard, showNotification, goNextTab])

  const handlePackageSaved = useCallback(
    async (packageId: string) => {
      if (!variantId) return
      wizard.invalidateTab('packages')
      const pkg = await fetchEsimPackageDetail(packageId)
      if (pkg) {
        wizard.setEsimPackage(pkg)
        wizard.setPackageForm(mapPackageToForm(pkg))
        wizard.setProviderName(pkg.providerName)
      }
      showNotification({ title: 'Thành công', message: 'Đã lưu gói eSIM', variant: 'success', delay: 2000 })
      goNextTab('carriers', { packageId })
    },
    [variantId, wizard, showNotification, goNextTab],
  )

  const saveCarriers = useCallback(async (): Promise<boolean> => {
    if (!wizard.packageForm?.id) {
      showNotification({ title: 'Lỗi', message: 'Chưa có gói eSIM', variant: 'danger', delay: 3000 })
      return false
    }
    if (wizard.selectedCarrierIds.length === 0) {
      showNotification({
        title: 'Lỗi',
        message: 'Vui lòng chọn ít nhất 1 nhà mạng',
        variant: 'danger',
        delay: 3000,
      })
      return false
    }

    setIsSaving(true)
    try {
      await updateEsimPackage(
        wizard.packageForm.id,
        toPackagePayload({ ...wizard.packageForm, carrierIds: wizard.selectedCarrierIds }),
      )
      wizard.setPackageForm((p) => (p ? { ...p, carrierIds: wizard.selectedCarrierIds } : p))
      const refreshed = await fetchEsimPackageDetail(wizard.packageForm.id)
      if (refreshed) wizard.setEsimPackage(refreshed)
      showNotification({ title: 'Thành công', message: 'Đã lưu nhà mạng', variant: 'success', delay: 2000 })
      return true
    } catch (e) {
      showNotification({
        title: 'Lỗi',
        message: e instanceof Error ? e.message : 'Không lưu được nhà mạng',
        variant: 'danger',
        delay: 4000,
      })
      return false
    } finally {
      setIsSaving(false)
    }
  }, [wizard, showNotification])

  const handleContinue = useCallback(async () => {
    if (activeTab === 'carriers') {
      const ok = await saveCarriers()
      if (ok) goNextTab('features')
      return
    }
    if (activeTab === 'features') {
      const ok = await featuresSaveRef.current?.()
      if (!ok) return
      if (variantId) {
        const features = await fetchVariantFeatures(variantId)
        wizard.setFeatureCount(features.length)
      }
      showNotification({ title: 'Thành công', message: 'Đã lưu tính năng', variant: 'success', delay: 2000 })
      goNextTab('review')
      return
    }
    if (activeTab === 'review') {
      navigate('/products/esim/packages')
    }
  }, [activeTab, saveCarriers, goNextTab, variantId, wizard, showNotification, navigate])

  const summary: EsimWizardSummary = useMemo(
    () => ({
      productName: wizard.productName,
      variantName: wizard.variant?.name ?? '',
      salePrice: wizard.price?.salePrice ?? 0,
      originalPrice: wizard.price?.originalPrice ?? 0,
      currency: wizard.price?.currency ?? 'VND',
      packageName: wizard.esimPackage?.name ?? wizard.packageForm?.name ?? '',
      providerName: wizard.providerName || wizard.esimPackage?.providerName || '',
      carrierNames:
        wizard.esimPackage?.carriers.map((c) => c.carrierName) ??
        wizard.selectedCarrierIds.map(
          (id) => wizard.packageForm?.carriers.find((c) => c.carrierId === id)?.carrierName ?? id,
        ),
      featureCount: wizard.featureCount,
      isActive: wizard.packageForm?.isActive ?? wizard.esimPackage?.isActive ?? true,
    }),
    [wizard],
  )

  const pageTitle = isNew ? 'Tạo gói eSIM' : `Chỉnh sửa: ${wizard.variant?.name ?? 'eSIM'}`
  const showLoading = !isNew && wizard.isLoading
  const showNotFound = !isNew && !wizard.isLoading && !wizard.variant

  return {
    isNew,
    variantId,
    activeTab,
    currentStep,
    isSaving,
    setIsSaving,
    featuresSaveRef,
    wizard,
    canAccessSubTabs,
    hasPackage,
    setActiveTab,
    handleVariantSaved,
    handlePriceSaved,
    handlePackageSaved,
    handleContinue,
    summary,
    pageTitle,
    showLoading,
    showNotFound,
  }
}
