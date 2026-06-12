import { useCallback, useEffect, useRef, useState } from 'react'
import { Badge, Card, Col, Container, Row, Spinner, Tab } from 'react-bootstrap'
import { Link, useLocation, useNavigate, useParams, useSearchParams } from 'react-router'

import PageBreadcrumb from '@/components/PageBreadcrumb'
import { useNotificationContext } from '@/context/useNotificationContext'
import { fetchProductDetail } from '@/apis/productsApi'
import {
  fetchEsimPackageDetail,
  updateEsimPackage,
} from '@/apis/esimPackagesApi'
import EsimWizardFooter from '@/features/products/esim-wizard/components/EsimWizardFooter'
import EsimWizardStepper from '@/features/products/esim-wizard/components/EsimWizardStepper'
import { getEsimStepByTab } from '@/features/products/esim-wizard/esimWizardSteps'
import { mapPackageToForm, toPackagePayload } from '@/features/products/esim-wizard/mapPackageForm'
import { fetchVariantFeatures } from '@/apis/productVariantFeaturesApi'
import type { EsimWizardSummary, EsimWizardTab } from '@/features/products/esim-wizard/types'
import { useEsimWizardLoader } from '@/features/products/esim-wizard/useEsimWizardLoader'
import WizardCarriersTab from '@/features/products/esim-wizard/components/WizardCarriersTab'
import WizardFeaturesTab from '@/features/products/esim-wizard/components/WizardFeaturesTab'
import WizardPackageTab from '@/features/products/esim-wizard/components/WizardPackageTab'
import WizardPriceTab from '@/features/products/esim-wizard/components/WizardPriceTab'
import WizardReviewTab from '@/features/products/esim-wizard/components/WizardReviewTab'
import WizardVariantTab from '@/features/products/esim-wizard/components/WizardVariantTab'
import {
  getDefaultPackageValues,
  getDefaultPriceValues,
} from '@/features/products/esim-wizard/wizardDefaults'

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

function formatPrice(value: number | undefined, currency: string | undefined): string {
  if (!value) return 'Chưa có giá'
  return `${value.toLocaleString('vi-VN')} ${currency ?? 'VND'}`
}

const EsimWizardPage = () => {
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

  const buildWizardParams = (tab: EsimWizardTab, overrides?: { packageId?: string }) => {
    const params: Record<string, string> = { tab }
    if (productIdParam) params.productId = productIdParam
    const pkgId = overrides?.packageId ?? packageIdParam
    if (pkgId) params.packageId = pkgId
    return params
  }
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

  const setActiveTab = (tab: EsimWizardTab) => {
    if (tab !== 'variants' && !canAccessSubTabs) return
    setSearchParams(buildWizardParams(tab))
  }

  const goNextTab = (tab: EsimWizardTab, overrides?: { packageId?: string }) => {
    setSearchParams(buildWizardParams(tab, overrides))
  }

  const handleVariantSaved = async (id: string, pId: string) => {
    const pName = wizard.productOptions.find((o) => o.value === pId)?.label ?? ''
    wizard.setProductName(pName)
    const product = await fetchProductDetail(pId)
    if (product?.countryId) wizard.setDefaultCountryId(product.countryId)
    showNotification({ title: 'Thành công', message: 'Đã lưu biến thể', variant: 'success', delay: 2000 })
    navigate(`/products/esim/wizard/${id}?tab=prices&productId=${pId}`, { replace: true })
  }

  const handlePriceSaved = async () => {
    if (!variantId) return
    wizard.invalidateTab('prices')
    await wizard.refreshPrice()
    showNotification({ title: 'Thành công', message: 'Đã lưu giá', variant: 'success', delay: 2000 })
    goNextTab('packages')
  }

  const handlePackageSaved = async (packageId: string) => {
    if (!variantId) return
    wizard.invalidateTab('packages')
    const pkg = await fetchEsimPackageDetail(packageId)
    if (pkg) {
      wizard.setEsimPackage(pkg)
      const form = mapPackageToForm(pkg)
      wizard.setPackageForm(form)
      wizard.setProviderName(pkg.providerName)
    }
    showNotification({ title: 'Thành công', message: 'Đã lưu gói eSIM', variant: 'success', delay: 2000 })
    goNextTab('carriers', { packageId })
  }

  const saveCarriers = async (): Promise<boolean> => {
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
        wizard.setFeatureCount(features.length)
      }
      showNotification({ title: 'Thành công', message: 'Đã lưu tính năng', variant: 'success', delay: 2000 })
      goNextTab('review')
      return
    }
    if (activeTab === 'review') {
      navigate('/products/esim/packages')
    }
  }

  if (!isNew && wizard.isLoading) {
    return (
      <Container fluid className="text-center py-5">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải...
      </Container>
    )
  }

  if (!isNew && !wizard.isLoading && !wizard.variant) {
    return (
      <Container fluid>
        <PageBreadcrumb title="Không tìm thấy" subtitle="eSIM du lịch" />
        <p className="text-muted">
          Biến thể không tồn tại. <Link to="/products/esim/packages">Quay lại danh sách</Link>
        </p>
      </Container>
    )
  }

  const summary: EsimWizardSummary = {
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
  }

  const pageTitle = isNew ? 'Tạo gói eSIM' : `Chỉnh sửa: ${wizard.variant?.name ?? 'eSIM'}`

  return (
    <Container fluid>
      <PageBreadcrumb title={pageTitle} subtitle="eSIM du lịch" />

      <Card className="border-0 shadow-sm mb-3">
        <Card.Body className="p-4">
          <Row className="align-items-center g-3">
            <Col lg={7}>
              <div className="d-flex align-items-center gap-2 mb-2">
                <Badge bg={isNew ? 'success-subtle' : 'primary-subtle'} text={isNew ? 'success' : 'primary'}>
                  {isNew ? 'Tạo mới' : 'Đang chỉnh sửa'}
                </Badge>
                <span className="text-muted fs-sm">Bước {currentStep.step}/6</span>
              </div>
              <h4 className="mb-1 fw-semibold">{currentStep.title}</h4>
              <p className="text-muted mb-0">{currentStep.subtitle}</p>
            </Col>
            <Col lg={5}>
              <div className="d-flex gap-2 flex-wrap justify-content-lg-end">
                <div className="rounded bg-light px-3 py-2">
                  <div className="text-muted fs-xs">Sản phẩm</div>
                  <div className="fw-semibold text-truncate" style={{ maxWidth: '12rem' }}>
                    {wizard.productName || 'Chưa chọn'}
                  </div>
                </div>
                <div className="rounded bg-light px-3 py-2">
                  <div className="text-muted fs-xs">Giá bán</div>
                  <div className="fw-semibold">{formatPrice(wizard.price?.salePrice, wizard.price?.currency)}</div>
                </div>
              </div>
            </Col>
          </Row>
        </Card.Body>
      </Card>

      <Card className="border-0 shadow-sm">
        <Card.Body className="p-4">
          <EsimWizardStepper
            activeTab={activeTab}
            canAccessSubTabs={canAccessSubTabs}
            onStepClick={setActiveTab}
          />

          <Tab.Container activeKey={activeTab}>
            <Tab.Content className="pt-3">
              <Tab.Pane eventKey="variants" mountOnEnter>
                <WizardVariantTab
                  isNew={isNew}
                  productOptions={wizard.productOptions}
                  initialValues={wizard.variant}
                  onSaved={handleVariantSaved}
                  onSavingChange={setIsSaving}
                />
              </Tab.Pane>

              {variantId && wizard.productId && (
                <Tab.Pane eventKey="prices" mountOnEnter>
                  <WizardPriceTab
                    productId={wizard.productId}
                    variantId={variantId}
                    initialValues={wizard.price ?? getDefaultPriceValues(wizard.productId, variantId)}
                    onSaved={() => void handlePriceSaved()}
                    onSavingChange={setIsSaving}
                  />
                </Tab.Pane>
              )}

              {variantId && wizard.productId && (
                <Tab.Pane eventKey="packages" mountOnEnter>
                  <WizardPackageTab
                    productId={wizard.productId}
                    variantId={variantId}
                    defaultCountryId={wizard.packageForm?.countryId ?? wizard.defaultCountryId}
                    providerOptions={wizard.providerOptions}
                    countryOptions={wizard.countryOptions}
                    initialValues={
                      wizard.packageForm ??
                      getDefaultPackageValues(wizard.productId, variantId, wizard.defaultCountryId)
                    }
                    onSaved={(id) => void handlePackageSaved(id)}
                    onSavingChange={setIsSaving}
                  />
                </Tab.Pane>
              )}

              {hasPackage && (
                <Tab.Pane eventKey="carriers" mountOnEnter>
                  <WizardCarriersTab
                    selectedCarrierIds={wizard.selectedCarrierIds}
                    onChange={wizard.setSelectedCarrierIds}
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
                {wizard.isTabLoading ? (
                  <div className="text-center py-4">
                    <Spinner animation="border" size="sm" className="me-2" />
                    Đang tải thông tin...
                  </div>
                ) : (
                  <WizardReviewTab summary={summary} isNew={isNew} />
                )}
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
