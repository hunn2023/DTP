import { Badge, Card, Col, Container, Row, Spinner, Tab } from 'react-bootstrap'
import { Link } from 'react-router'

import PageBreadcrumb from '@/components/PageBreadcrumb'
import EsimWizardFooter from '@/features/products/esim-wizard/components/EsimWizardFooter'
import EsimWizardStepper from '@/features/products/esim-wizard/components/EsimWizardStepper'
import WizardCarriersTab from '@/features/products/esim-wizard/components/WizardCarriersTab'
import WizardFeaturesTab from '@/features/products/esim-wizard/components/WizardFeaturesTab'
import WizardPackageTab from '@/features/products/esim-wizard/components/WizardPackageTab'
import WizardPriceTab from '@/features/products/esim-wizard/components/WizardPriceTab'
import WizardReviewTab from '@/features/products/esim-wizard/components/WizardReviewTab'
import WizardVariantTab from '@/features/products/esim-wizard/components/WizardVariantTab'
import {
  formatEsimWizardPrice,
  getEsimWizardFormId,
  useEsimWizardPage,
} from '@/features/products/esim-wizard/useEsimWizardPage'
import {
  getDefaultPackageValues,
  getDefaultPriceValues,
} from '@/features/products/esim-wizard/wizardDefaults'

const EsimWizardPage = () => {
  const {
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
  } = useEsimWizardPage()

  if (showLoading) {
    return (
      <Container fluid className="text-center py-5">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải...
      </Container>
    )
  }

  if (showNotFound) {
    return (
      <Container fluid>
        <PageBreadcrumb title="Không tìm thấy" subtitle="eSIM du lịch" />
        <p className="text-muted">
          Biến thể không tồn tại. <Link to="/products/esim/packages">Quay lại danh sách</Link>
        </p>
      </Container>
    )
  }

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
                  <div className="fw-semibold">
                    {formatEsimWizardPrice(wizard.price?.salePrice, wizard.price?.currency)}
                  </div>
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
              formId={getEsimWizardFormId(activeTab)}
              onContinue={() => void handleContinue()}
            />
          )}
        </Card.Body>
      </Card>
    </Container>
  )
}

export default EsimWizardPage
