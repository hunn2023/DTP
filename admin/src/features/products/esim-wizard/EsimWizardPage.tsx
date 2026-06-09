import { useCallback, useEffect, useRef, useState } from 'react'
import { Card, Container, Spinner, Tab } from 'react-bootstrap'
import { Link, useLocation, useNavigate, useParams, useSearchParams } from 'react-router'

import PageBreadcrumb from '@/components/PageBreadcrumb'
import { useNotificationContext } from '@/context/useNotificationContext'
import { fetchProductPrices } from '@/features/master-data/product-prices/product-prices.api'
import { fetchProductVariants } from '@/features/master-data/products/product-variants.api'
import { fetchAdminProducts, fetchProductDetail } from '@/features/master-data/products/products.api'
import type { ProductPriceRow, ProductVariant } from '@/features/master-data/products/types'
import { fetchCountries } from '@/features/master-data/countries/countries.api'
import {
  fetchAdminEsimPackages,
  fetchEsimPackageDetail,
  updateEsimPackage,
} from '@/features/products/esim-packages/esim-packages.api'
import type { EsimPackage } from '@/features/products/esim-packages/types'
import EsimWizardFooter from '@/features/products/esim-wizard/EsimWizardFooter'
import EsimWizardStepper from '@/features/products/esim-wizard/EsimWizardStepper'
import { getEsimStepByTab } from '@/features/products/esim-wizard/esimWizardSteps'
import { mapPackageToForm, toPackagePayload } from '@/features/products/esim-wizard/mapPackageForm'
import { fetchVariantFeatures } from '@/features/products/esim-wizard/product-variant-features.api'
import type { EsimPackageForm, EsimWizardSummary, EsimWizardTab } from '@/features/products/esim-wizard/types'
import WizardCarriersTab from '@/features/products/esim-wizard/WizardCarriersTab'
import WizardFeaturesTab from '@/features/products/esim-wizard/WizardFeaturesTab'
import WizardPackageTab from '@/features/products/esim-wizard/WizardPackageTab'
import WizardPriceTab from '@/features/products/esim-wizard/WizardPriceTab'
import WizardReviewTab from '@/features/products/esim-wizard/WizardReviewTab'
import WizardVariantTab from '@/features/products/esim-wizard/WizardVariantTab'
import {
  getDefaultPackageValues,
  getDefaultPriceValues,
  getDefaultVariantValues,
} from '@/features/products/esim-wizard/wizardDefaults'
import { fetchProviderOptions } from '@/features/providers/providers.api'
import type { FormFieldOption } from '@/modules/crud/form/types'

const TAB_KEYS: EsimWizardTab[] = ['variants', 'prices', 'packages', 'carriers', 'features', 'review']

function parseTab(value: string | null): EsimWizardTab {
  if (value && TAB_KEYS.includes(value as EsimWizardTab)) return value as EsimWizardTab
  return 'variants'
}

function isNewWizardRoute(routeId: string | undefined, pathname: string): boolean {
  if (routeId === 'new') return true
  return pathname.replace(/\/$/, '').endsWith('/wizard/new')
}

function getFormId(tab: EsimWizardTab): string | undefined {
  if (tab === 'variants') return 'esim-wizard-variant-form'
  if (tab === 'prices') return 'esim-wizard-price-form'
  if (tab === 'packages') return 'esim-wizard-package-form'
  return undefined
}

const EsimWizardPage = () => {
  const { wizardId: routeWizardId } = useParams()
  const location = useLocation()
  const [searchParams, setSearchParams] = useSearchParams()
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()

  const isNew = isNewWizardRoute(routeWizardId, location.pathname)
  const variantId = isNew ? null : (routeWizardId ?? null)
  const activeTab = parseTab(searchParams.get('tab'))
  const currentStep = getEsimStepByTab(activeTab)

  const [isLoading, setIsLoading] = useState(!isNew)
  const [isSaving, setIsSaving] = useState(false)
  const [productOptions, setProductOptions] = useState<FormFieldOption[]>([])
  const [providerOptions, setProviderOptions] = useState<FormFieldOption[]>([])
  const [countryOptions, setCountryOptions] = useState<FormFieldOption[]>([])

  const [productId, setProductId] = useState('')
  const [variant, setVariant] = useState<ProductVariant | null>(null)
  const [price, setPrice] = useState<ProductPriceRow | null>(null)
  const [packageForm, setPackageForm] = useState<EsimPackageForm | null>(null)
  const [esimPackage, setEsimPackage] = useState<EsimPackage | null>(null)
  const [selectedCarrierIds, setSelectedCarrierIds] = useState<string[]>([])
  const [featureCount, setFeatureCount] = useState(0)
  const [productName, setProductName] = useState('')
  const [providerName, setProviderName] = useState('')
  const [defaultCountryId, setDefaultCountryId] = useState('')

  const featuresSaveRef = useRef<(() => Promise<boolean>) | null>(null)

  const canAccessSubTabs = Boolean(variantId)

  const loadLookups = useCallback(async () => {
    const [products, providers, countries] = await Promise.all([
      fetchAdminProducts(1, 200, { isActive: true }),
      fetchProviderOptions(),
      fetchCountries(),
    ])
    setProductOptions(products.items.map((p) => ({ value: p.id, label: p.name })))
    setProviderOptions(providers)
    setCountryOptions(
      countries.map((c) => ({ value: c.id, label: `${c.isoCode} ${c.name}` })),
    )
  }, [])

  const reloadSummary = useCallback(async (vId: string) => {
    const [prices, packages, features] = await Promise.all([
      fetchProductPrices({ productVariantId: vId }),
      fetchAdminEsimPackages(1, 1, { productVariantId: vId }),
      fetchVariantFeatures(vId),
    ])
    setPrice(prices[0] ?? null)
    const pkg = packages.items[0] ?? null
    setEsimPackage(pkg)
    if (pkg) {
      const form = mapPackageToForm(pkg)
      setPackageForm(form)
      setSelectedCarrierIds(form.carrierIds)
      setProviderName(pkg.providerName)
    }
    setFeatureCount(features.length)
  }, [])

  const loadEditData = useCallback(
    async (vId: string) => {
      setIsLoading(true)
      try {
        await loadLookups()
        const products = await fetchAdminProducts(1, 200)
        let foundVariant: ProductVariant | null = null
        let foundProductId = ''

        for (const product of products.items) {
          const variants = await fetchProductVariants(product.id)
          const match = variants.find((v) => v.id === vId)
          if (match) {
            foundVariant = match
            foundProductId = product.id
            setProductName(product.name)
            break
          }
        }

        if (!foundVariant) return

        setProductId(foundProductId)
        setVariant(foundVariant)
        const product = await fetchProductDetail(foundProductId)
        if (product?.countryId) setDefaultCountryId(product.countryId)
        await reloadSummary(vId)
      } finally {
        setIsLoading(false)
      }
    },
    [loadLookups, reloadSummary],
  )

  useEffect(() => {
    if (isNew) {
      void loadLookups().catch(() => {
        showNotification({
          title: 'Lỗi',
          message: 'Không tải được dữ liệu tham chiếu',
          variant: 'danger',
          delay: 4000,
        })
      })
      return
    }
    if (variantId) void loadEditData(variantId)
  }, [isNew, variantId, loadLookups, loadEditData, showNotification])

  useEffect(() => {
    if (isNew && activeTab !== 'variants') {
      setSearchParams({ tab: 'variants' }, { replace: true })
    }
  }, [isNew, activeTab, setSearchParams])

  const setActiveTab = (tab: EsimWizardTab) => {
    if (tab !== 'variants' && !canAccessSubTabs) return
    setSearchParams({ tab })
  }

  const goNextTab = (tab: EsimWizardTab) => setSearchParams({ tab })

  const handleVariantSaved = async (id: string, pId: string) => {
    const pName = productOptions.find((o) => o.value === pId)?.label ?? ''
    setProductName(pName)
    setProductId(pId)
    setVariant((prev) => ({ ...(prev ?? getDefaultVariantValues(pId)), id, productId: pId }))
    const product = await fetchProductDetail(pId)
    if (product?.countryId) setDefaultCountryId(product.countryId)
    showNotification({ title: 'Thành công', message: 'Đã lưu variant', variant: 'success', delay: 2000 })
    navigate(`/products/esim/wizard/${id}?tab=prices`, { replace: true })
  }

  const handlePriceSaved = async (priceId: string) => {
    if (!variantId) return
    const prices = await fetchProductPrices({ productVariantId: variantId })
    setPrice(prices.find((p) => p.id === priceId) ?? prices[0] ?? null)
    showNotification({ title: 'Thành công', message: 'Đã lưu giá', variant: 'success', delay: 2000 })
    goNextTab('packages')
  }

  const handlePackageSaved = async (packageId: string) => {
    if (!variantId) return
    const packages = await fetchAdminEsimPackages(1, 1, { productVariantId: variantId })
    const pkg = packages.items.find((p) => p.id === packageId) ?? packages.items[0]
    if (pkg) {
      setEsimPackage(pkg)
      const form = mapPackageToForm(pkg)
      setPackageForm(form)
      setProviderName(pkg.providerName)
    }
    showNotification({ title: 'Thành công', message: 'Đã lưu gói eSIM', variant: 'success', delay: 2000 })
    goNextTab('carriers')
  }

  const saveCarriers = async (): Promise<boolean> => {
    if (!packageForm?.id) {
      showNotification({ title: 'Lỗi', message: 'Chưa có gói eSIM', variant: 'danger', delay: 3000 })
      return false
    }
    if (selectedCarrierIds.length === 0) {
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
        packageForm.id,
        toPackagePayload({ ...packageForm, carrierIds: selectedCarrierIds }),
      )
      setPackageForm((p) => (p ? { ...p, carrierIds: selectedCarrierIds } : p))
      const refreshed = await fetchEsimPackageDetail(packageForm.id)
      if (refreshed) setEsimPackage(refreshed)
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
  }

  const handleContinue = async () => {
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
        setFeatureCount(features.length)
      }
      showNotification({ title: 'Thành công', message: 'Đã lưu tính năng', variant: 'success', delay: 2000 })
      goNextTab('review')
      return
    }
    if (activeTab === 'review') {
      navigate('/products/esim/packages')
    }
  }

  if (!isNew && isLoading) {
    return (
      <Container fluid className="text-center py-5">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải...
      </Container>
    )
  }

  if (!isNew && !isLoading && !variant) {
    return (
      <Container fluid>
        <PageBreadcrumb title="Không tìm thấy" subtitle="eSIM du lịch" />
        <p className="text-muted">
          Variant không tồn tại. <Link to="/products/esim/packages">Quay lại danh sách</Link>
        </p>
      </Container>
    )
  }

  const summary: EsimWizardSummary = {
    productName,
    variantName: variant?.name ?? '',
    salePrice: price?.salePrice ?? 0,
    originalPrice: price?.originalPrice ?? 0,
    currency: price?.currency ?? 'VND',
    packageName: esimPackage?.name ?? packageForm?.name ?? '',
    providerName: providerName || esimPackage?.providerName || '',
    carrierNames:
      esimPackage?.carriers.map((c) => c.carrierName) ??
      selectedCarrierIds.map(
        (id) => packageForm?.carriers.find((c) => c.carrierId === id)?.carrierName ?? id,
      ),
    featureCount,
    isActive: packageForm?.isActive ?? esimPackage?.isActive ?? true,
  }

  const pageTitle = isNew ? 'Tạo eSIM Package' : `Chỉnh sửa: ${variant?.name ?? 'eSIM'}`

  return (
    <Container fluid>
      <PageBreadcrumb title={pageTitle} subtitle="eSIM du lịch" />

      <Card>
        <Card.Body className="p-4">
          <h4 className="mb-3 fw-semibold">
            {currentStep.step}. {currentStep.title}
          </h4>

          <EsimWizardStepper
            activeTab={activeTab}
            canAccessSubTabs={canAccessSubTabs}
            onStepClick={setActiveTab}
          />

          <Tab.Container activeKey={activeTab}>
            <Tab.Content>
              <Tab.Pane eventKey="variants" mountOnEnter>
                <WizardVariantTab
                  isNew={isNew}
                  productOptions={productOptions}
                  initialValues={variant}
                  onSaved={handleVariantSaved}
                  onSavingChange={setIsSaving}
                />
              </Tab.Pane>

              {variantId && productId && (
                <Tab.Pane eventKey="prices" mountOnEnter>
                  <WizardPriceTab
                    productId={productId}
                    variantId={variantId}
                    initialValues={price ?? getDefaultPriceValues(productId, variantId)}
                    onSaved={(id) => void handlePriceSaved(id)}
                    onSavingChange={setIsSaving}
                  />
                </Tab.Pane>
              )}

              {variantId && productId && (
                <Tab.Pane eventKey="packages" mountOnEnter>
                  <WizardPackageTab
                    productId={productId}
                    variantId={variantId}
                    defaultCountryId={packageForm?.countryId ?? defaultCountryId}
                    providerOptions={providerOptions}
                    countryOptions={countryOptions}
                    initialValues={
                      packageForm ?? getDefaultPackageValues(productId, variantId, defaultCountryId)
                    }
                    onSaved={(id) => void handlePackageSaved(id)}
                    onSavingChange={setIsSaving}
                  />
                </Tab.Pane>
              )}

              {(packageForm?.id || esimPackage?.id) && (
                <Tab.Pane eventKey="carriers" mountOnEnter>
                  <WizardCarriersTab
                    selectedCarrierIds={selectedCarrierIds}
                    onChange={setSelectedCarrierIds}
                  />
                </Tab.Pane>
              )}

              {variantId && (
                <Tab.Pane eventKey="features" mountOnEnter>
                  <WizardFeaturesTab
                    variantId={variantId}
                    onRegisterSave={(fn) => {
                      featuresSaveRef.current = fn
                    }}
                    onSavingChange={setIsSaving}
                  />
                </Tab.Pane>
              )}

              <Tab.Pane eventKey="review" mountOnEnter>
                <WizardReviewTab summary={summary} isNew={isNew} />
              </Tab.Pane>
            </Tab.Content>
          </Tab.Container>

          {activeTab !== 'review' && (
            <EsimWizardFooter
              activeTab={activeTab}
              isSaving={isSaving}
              formId={getFormId(activeTab)}
              onContinue={() => void handleContinue()}
            />
          )}
        </Card.Body>
      </Card>
    </Container>
  )
}

export default EsimWizardPage
