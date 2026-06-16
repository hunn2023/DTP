import { Alert, Container, Spinner } from 'react-bootstrap'
import { Link } from 'react-router'

import PageBreadcrumb from '@/components/PageBreadcrumb'
import EsimWizard from '@/features/products/esim-wizard/components/EsimWizard'
import EsimWizardFooter from '@/features/products/esim-wizard/components/EsimWizardFooter'
import EsimWizardUnsavedModal from '@/features/products/esim-wizard/components/EsimWizardUnsavedModal'
import WizardCarriersTab from '@/features/products/esim-wizard/components/WizardCarriersTab'
import WizardFeaturesTab from '@/features/products/esim-wizard/components/WizardFeaturesTab'
import WizardPackageTab from '@/features/products/esim-wizard/components/WizardPackageTab'
import WizardPriceTab from '@/features/products/esim-wizard/components/WizardPriceTab'
import WizardReviewTab from '@/features/products/esim-wizard/components/WizardReviewTab'
import WizardVariantTab from '@/features/products/esim-wizard/components/WizardVariantTab'
import { getEsimWizardFormId, useEsimWizardPage } from '@/features/products/esim-wizard/useEsimWizardPage'
import type { EsimWizardTab } from '@/features/products/esim-wizard/types'

const renderFooter = (
  isSetupFlow: boolean,
  activeTab: EsimWizardTab,
  isSaving: boolean,
  onContinue: () => void,
  onSave: () => void,
) => (
  <div className="border-top mt-4 pt-3">
    <EsimWizardFooter
      isSetupFlow={isSetupFlow}
      activeTab={activeTab}
      isSaving={isSaving}
      formId={getEsimWizardFormId(activeTab)}
      onContinue={onContinue}
      onSave={onSave}
    />
  </div>
)

const EsimWizardPage = () => {
  const {
    isNew,
    isSetupFlow,
    variantId,
    activeTab,
    isSaving,
    setIsSaving,
    wizard,
    canAccessTab,
    hasPackage,
    requestTabChange,
    handleVariantSaved,
    handlePriceSaved,
    handlePackageSaved,
    handleContinue,
    handleSaveCurrentTab,
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
    cardTitle,
    showLoading,
    showNotFound,
    showUnsavedModal,
    closeUnsavedModal,
    discardPendingTabChange,
    confirmUnsavedSave,
  } = useEsimWizardPage()

  if (showLoading) {
    return (
      <Container fluid className="text-center py-5">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải gói eSIM...
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

  const onContinue = () => void handleContinue()
  const onSave = () => void handleSaveCurrentTab()
  const renderStepFooter = (tab: EsimWizardTab) =>
    renderFooter(isSetupFlow, tab, isSaving, onContinue, onSave)

  return (
    <Container fluid>
      <PageBreadcrumb title={pageTitle} subtitle="eSIM du lịch" />

      <EsimWizard
        title={cardTitle}
        activeTab={activeTab}
        canAccessTab={canAccessTab}
        onStepChange={requestTabChange}
        variantStep={
          <>
            <WizardVariantTab
              isNew={isNew}
              productOptions={wizard.productOptions}
              initialValues={wizard.variant}
              onSaved={(id, pId) => void handleVariantSaved(id, pId)}
              onSavingChange={setIsSaving}
              onDirtyChange={setVariantsDirty}
              onRegisterSave={registerVariantSave}
            />
            {renderStepFooter('variants')}
          </>
        }
        priceStep={
          <>
            {variantId && wizard.productId ? (
              <WizardPriceTab
                productId={wizard.productId}
                variantId={variantId}
                initialValues={wizard.price}
                onSaved={(priceId) => void handlePriceSaved(priceId)}
                onSavingChange={setIsSaving}
                onDirtyChange={setPricesDirty}
                onRegisterSave={registerPriceSave}
              />
            ) : (
              <p className="text-muted mb-0">Lưu biến thể trước khi thiết lập giá.</p>
            )}
            {renderStepFooter('prices')}
          </>
        }
        packageStep={
          <>
            {variantId && wizard.productId ? (
              <WizardPackageTab
                productId={wizard.productId}
                variantId={variantId}
                defaultCountryId={wizard.packageForm?.countryId ?? wizard.defaultCountryId}
                initialValues={wizard.packageForm}
                onSaved={(id) => void handlePackageSaved(id)}
                onSavingChange={setIsSaving}
                onDirtyChange={setPackagesDirty}
                onRegisterSave={registerPackageSave}
              />
            ) : (
              <p className="text-muted mb-0">Lưu biến thể trước khi tạo gói eSIM.</p>
            )}
            {renderStepFooter('packages')}
          </>
        }
        carrierStep={
          <>
            {!hasPackage && (
              <Alert variant="warning" className="fs-sm">
                Lưu gói eSIM ở bước trước để gắn nhà mạng vào gói khi bấm Tiếp tục.
              </Alert>
            )}
            <WizardCarriersTab
              selectedCarrierIds={wizard.selectedCarrierIds}
              savedCarrierIds={wizard.packageForm?.carrierIds ?? []}
              countryId={wizard.packageForm?.countryId ?? wizard.defaultCountryId}
              onChange={wizard.setSelectedCarrierIds}
              onDirtyChange={setCarriersDirty}
            />
            {renderStepFooter('carriers')}
          </>
        }
        featureStep={
          <>
            {variantId ? (
              <WizardFeaturesTab
                variantId={variantId}
                onRegisterSave={registerFeaturesSave}
                onSavingChange={setIsSaving}
                onDirtyChange={setFeaturesDirty}
              />
            ) : (
              <p className="text-muted mb-0">Lưu biến thể trước khi thêm tính năng.</p>
            )}
            {renderStepFooter('features')}
          </>
        }
        reviewStep={
          <>
            <WizardReviewTab summary={summary} isSetupFlow={isSetupFlow} />
          </>
        }
      />

      <EsimWizardUnsavedModal
        show={showUnsavedModal}
        isSaving={isSaving}
        onHide={closeUnsavedModal}
        onDiscard={discardPendingTabChange}
        onSave={() => void confirmUnsavedSave()}
      />
    </Container>
  )
}

export default EsimWizardPage
