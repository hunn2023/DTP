import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useLocation, useNavigate, useParams, useSearchParams } from 'react-router'

import { fetchEsimPackageDetail, updateEsimPackage } from '@/apis/esimPackagesApi'
import { fetchProductDetail } from '@/apis/productsApi'
import { fetchProductVariants } from '@/apis/productVariantsApi'
import { fetchVariantFeatures } from '@/apis/productVariantFeaturesApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import { getEsimStepByTab } from '@/features/products/esim-wizard/esimWizardSteps'
import {
  canAccessWizardTab,
  getNextSetupTab,
  getWizardFallbackTab,
  resolveIsSetupFlow,
} from '@/features/products/esim-wizard/esimWizardAccess'
import { mapPackageToForm, toWizardPackagePayload } from '@/features/products/esim-wizard/mapPackageForm'
import { getDefaultPriceValues } from '@/features/products/esim-wizard/wizardDefaults'
import type { EsimWizardSummary, EsimWizardTab } from '@/features/products/esim-wizard/types'
import { useEsimWizardLoader } from '@/features/products/esim-wizard/useEsimWizardLoader'
import { toWizardSaveOutcome, type WizardSaveFn, type WizardSaveOutcome } from '@/features/products/esim-wizard/wizardSave'

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

  const currentStep = getEsimStepByTab(activeTab)
  const [isSaving, setIsSaving] = useState(false)
  const [dirtyTabs, setDirtyTabs] = useState<Set<EsimWizardTab>>(new Set())
  const [pendingTab, setPendingTab] = useState<EsimWizardTab | null>(null)
  const [showUnsavedModal, setShowUnsavedModal] = useState(false)

  const variantSaveRef = useRef<(() => Promise<boolean>) | null>(null)
  const priceSaveRef = useRef<WizardSaveFn | null>(null)
  const packageSaveRef = useRef<(() => Promise<boolean>) | null>(null)
  const featuresSaveRef = useRef<(() => Promise<boolean>) | null>(null)
  const savedPackageIdRef = useRef<string | null>(null)
  const pendingTabUnlockRef = useRef<EsimWizardTab | null>(null)

  const markTabDirty = useCallback((tab: EsimWizardTab, dirty: boolean) => {
    setDirtyTabs((prev) => {
      if (prev.has(tab) === dirty) return prev
      const next = new Set(prev)
      if (dirty) next.add(tab)
      else next.delete(tab)
      return next
    })
  }, [])

  const setVariantsDirty = useCallback((dirty: boolean) => markTabDirty('variants', dirty), [markTabDirty])
  const setPricesDirty = useCallback((dirty: boolean) => markTabDirty('prices', dirty), [markTabDirty])
  const setPackagesDirty = useCallback((dirty: boolean) => markTabDirty('packages', dirty), [markTabDirty])
  const setCarriersDirty = useCallback((dirty: boolean) => markTabDirty('carriers', dirty), [markTabDirty])
  const setFeaturesDirty = useCallback((dirty: boolean) => markTabDirty('features', dirty), [markTabDirty])

  const registerVariantSave = useCallback((fn: () => Promise<boolean>) => {
    variantSaveRef.current = fn
  }, [])
  const registerPriceSave = useCallback((fn: WizardSaveFn) => {
    priceSaveRef.current = fn
  }, [])
  const registerPackageSave = useCallback((fn: () => Promise<boolean>) => {
    packageSaveRef.current = fn
  }, [])
  const registerFeaturesSave = useCallback((fn: () => Promise<boolean>) => {
    featuresSaveRef.current = fn
  }, [])

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
      setSearchParams({ tab: 'variants', setup: '1' }, { replace: true })
    }
  }, [isNew, activeTab, setSearchParams])

  const canAccessSubTabs = Boolean(variantId)
  const hasPackage = Boolean(wizard.packageForm?.id || wizard.esimPackage?.id)
  const hasPrice = Boolean(wizard.price?.id)
  const isSetupFlow = resolveIsSetupFlow({
    isNewRoute: isNew,
    setupParam: searchParams.get('setup') === '1',
    packageIdParam,
    variantId,
    hasPackage,
    isLoading: wizard.isLoading,
  })

  const buildWizardParams = useCallback(
    (tab: EsimWizardTab, overrides?: { packageId?: string }) => {
      const params: Record<string, string> = { tab }
      if (productIdParam) params.productId = productIdParam
      const pkgId = overrides?.packageId ?? packageIdParam
      if (pkgId) params.packageId = pkgId
      if (isSetupFlow || isNew) params.setup = '1'
      return params
    },
    [productIdParam, packageIdParam, isSetupFlow, isNew],
  )

  const wizardAccess = useMemo(
    () => ({ variantId, isSetupFlow, hasPrice, hasPackage }),
    [variantId, isSetupFlow, hasPrice, hasPackage],
  )

  const canAccessTab = useCallback(
    (tab: EsimWizardTab) => {
      if (pendingTabUnlockRef.current === tab) return true
      return canAccessWizardTab(tab, wizardAccess)
    },
    [wizardAccess],
  )

  useEffect(() => {
    const pending = pendingTabUnlockRef.current
    if (!pending) return
    if (activeTab === pending && canAccessWizardTab(activeTab, wizardAccess)) {
      pendingTabUnlockRef.current = null
    }
  }, [activeTab, wizardAccess])

  useEffect(() => {
    if (isNew || wizard.isLoading || packageIdParam) return
    if (!isSetupFlow || searchParams.get('setup') === '1') return
    setSearchParams(buildWizardParams(activeTab), { replace: true })
  }, [isNew, wizard.isLoading, packageIdParam, isSetupFlow, searchParams, activeTab, buildWizardParams, setSearchParams])

  useEffect(() => {
    if (isNew || wizard.isLoading) return
    if (canAccessTab(activeTab)) return
    const fallback = getWizardFallbackTab(wizardAccess)
    if (activeTab !== fallback) {
      setSearchParams(buildWizardParams(fallback), { replace: true })
    }
  }, [isNew, wizard.isLoading, activeTab, wizardAccess, canAccessTab, buildWizardParams, setSearchParams])

  const setActiveTab = useCallback(
    (tab: EsimWizardTab) => {
      if (!canAccessTab(tab)) return
      setSearchParams(buildWizardParams(tab))
    },
    [canAccessTab, buildWizardParams, setSearchParams],
  )

  const requestTabChange = useCallback(
    (tab: EsimWizardTab) => {
      if (tab === activeTab) return
      if (!canAccessTab(tab)) return
      if (dirtyTabs.has(activeTab)) {
        setPendingTab(tab)
        setShowUnsavedModal(true)
        return
      }
      setActiveTab(tab)
    },
    [activeTab, canAccessTab, dirtyTabs, setActiveTab],
  )

  const goNextTab = useCallback(
    (tab: EsimWizardTab, overrides?: { packageId?: string }) => {
      pendingTabUnlockRef.current = tab
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

      const variants = await fetchProductVariants(pId)
      const found = variants.find((v) => v.id === id)
      if (found) wizard.setVariant(found)

      markTabDirty('variants', false)
      showNotification({ title: 'Thành công', message: 'Đã lưu biến thể', variant: 'success', delay: 2000 })

      if (isNew || isSetupFlow) {
        navigate(`/products/esim/wizard/${id}?tab=prices&productId=${pId}&setup=1`, { replace: true })
      }
    },
    [wizard, showNotification, navigate, isNew, isSetupFlow, markTabDirty],
  )

  const handlePriceSaved = useCallback(
    async (priceId: string) => {
      if (!variantId) return
      showNotification({ title: 'Thành công', message: 'Đã lưu giá', variant: 'success', delay: 2000 })
      if (isSetupFlow) {
        wizard.setPrice((prev) => {
          if (prev) return { ...prev, id: priceId }
          if (!wizard.productId || !variantId) return prev
          return { ...getDefaultPriceValues(wizard.productId, variantId), id: priceId }
        })
        markTabDirty('prices', false)
        return
      }
      wizard.invalidateTab('prices')
      await wizard.refreshPrice()
      markTabDirty('prices', false)
    },
    [variantId, wizard, showNotification, markTabDirty, isSetupFlow],
  )

  const handlePackageSaved = useCallback(
    async (packageId: string) => {
      savedPackageIdRef.current = packageId
      if (!variantId) return
      wizard.invalidateTab('packages')
      const pkg = await fetchEsimPackageDetail(packageId)
      if (pkg) {
        wizard.setEsimPackage(pkg)
        wizard.setPackageForm(mapPackageToForm(pkg))
        wizard.setProviderName(pkg.providerName)
      }
      markTabDirty('packages', false)
      showNotification({ title: 'Thành công', message: 'Đã lưu gói eSIM', variant: 'success', delay: 2000 })
    },
    [variantId, wizard, showNotification, markTabDirty],
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
        toWizardPackagePayload({ ...wizard.packageForm, carrierIds: wizard.selectedCarrierIds }),
      )
      wizard.setPackageForm((p) => (p ? { ...p, carrierIds: wizard.selectedCarrierIds } : p))
      const refreshed = await fetchEsimPackageDetail(wizard.packageForm.id)
      if (refreshed) wizard.setEsimPackage(refreshed)
      markTabDirty('carriers', false)
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
  }, [wizard, showNotification, markTabDirty])

  const saveActiveTab = useCallback(async (): Promise<WizardSaveOutcome> => {
    switch (activeTab) {
      case 'variants': {
        const ok = (await variantSaveRef.current?.()) ?? false
        return ok ? { ok: true } : { ok: false, message: 'Không lưu được biến thể' }
      }
      case 'prices':
        return toWizardSaveOutcome(await priceSaveRef.current?.())
      case 'packages': {
        const ok = (await packageSaveRef.current?.()) ?? false
        return ok ? { ok: true } : { ok: false, message: 'Không lưu được gói eSIM' }
      }
      case 'carriers': {
        const ok = await saveCarriers()
        return ok ? { ok: true } : { ok: false, message: 'Không lưu được nhà mạng' }
      }
      case 'features': {
        const ok = (await featuresSaveRef.current?.()) ?? false
        if (!ok) return { ok: false, message: 'Không lưu được tính năng' }
        if (variantId) {
          const features = await fetchVariantFeatures(variantId)
          wizard.setFeatureCount(features.length)
        }
        markTabDirty('features', false)
        showNotification({ title: 'Thành công', message: 'Đã lưu tính năng', variant: 'success', delay: 2000 })
        return { ok: true }
      }
      default:
        return { ok: true }
    }
  }, [activeTab, saveCarriers, variantId, wizard, markTabDirty, showNotification])

  const handleContinue = useCallback(async () => {
    if (activeTab === 'review') {
      navigate('/products/esim/packages')
      return
    }

    const result = await saveActiveTab()
    if (!result.ok) {
      showNotification({
        title: 'Lỗi',
        message: result.message,
        variant: 'danger',
        delay: 4000,
      })
      return
    }

    if (!isSetupFlow) return

    if (activeTab === 'variants' && isNew) return

    const nextTab = getNextSetupTab(activeTab)
    if (!nextTab) return

    if (activeTab === 'prices') {
      wizard.invalidateTab('prices')
      await wizard.refreshPrice()
      markTabDirty('prices', false)
      pendingTabUnlockRef.current = nextTab
    }

    if (activeTab === 'packages') {
      const pkgId =
        savedPackageIdRef.current ?? wizard.packageForm?.id ?? wizard.esimPackage?.id ?? packageIdParam
      goNextTab(nextTab, pkgId ? { packageId: pkgId } : undefined)
      return
    }

    goNextTab(nextTab)
  }, [
    activeTab,
    saveActiveTab,
    isSetupFlow,
    isNew,
    wizard,
    showNotification,
    markTabDirty,
    goNextTab,
    navigate,
    packageIdParam,
  ])

  const handleSaveCurrentTab = useCallback(async () => {
    await saveActiveTab()
  }, [saveActiveTab])

  const closeUnsavedModal = useCallback(() => {
    setShowUnsavedModal(false)
    setPendingTab(null)
  }, [])

  const discardPendingTabChange = useCallback(() => {
    if (!pendingTab) return
    wizard.invalidateTab(activeTab)
    markTabDirty(activeTab, false)
    setActiveTab(pendingTab)
    setPendingTab(null)
    setShowUnsavedModal(false)
  }, [pendingTab, activeTab, wizard, markTabDirty, setActiveTab])

  const confirmUnsavedSave = useCallback(async () => {
    const result = await saveActiveTab()
    if (!result.ok) return
    if (pendingTab) {
      setActiveTab(pendingTab)
      setPendingTab(null)
    }
    setShowUnsavedModal(false)
  }, [saveActiveTab, pendingTab, setActiveTab])

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

  const pageTitle = isSetupFlow ? 'Tạo gói eSIM' : `Chỉnh sửa: ${wizard.variant?.name ?? 'eSIM'}`
  const cardTitle = currentStep.title
  const showLoading = !isNew && wizard.isLoading
  const showNotFound = !isNew && !wizard.isLoading && !wizard.variant

  return {
    isNew,
    isSetupFlow,
    variantId,
    activeTab,
    currentStep,
    cardTitle,
    isSaving,
    setIsSaving,
    variantSaveRef,
    priceSaveRef,
    packageSaveRef,
    featuresSaveRef,
    wizard,
    canAccessSubTabs,
    canAccessTab,
    hasPackage,
    requestTabChange,
    handleVariantSaved,
    handlePriceSaved,
    handlePackageSaved,
    handleContinue,
    handleSaveCurrentTab,
    markTabDirty,
    setVariantsDirty,
    setPricesDirty,
    setPackagesDirty,
    setCarriersDirty,
    setFeaturesDirty,
    registerVariantSave,
    registerPriceSave,
    registerPackageSave,
    registerFeaturesSave,
    summary,
    pageTitle,
    showLoading,
    showNotFound,
    showUnsavedModal,
    closeUnsavedModal,
    discardPendingTabChange,
    confirmUnsavedSave,
  }
}
