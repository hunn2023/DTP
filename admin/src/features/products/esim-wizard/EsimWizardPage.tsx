import { Alert, Container, Spinner } from 'react-bootstrap'
import { Link } from 'react-router'

import PageBreadcrumb from '@/components/PageBreadcrumb'
import EsimWizard from '@/features/products/esim-wizard/components/EsimWizard'
import EsimWizardFooter from '@/features/products/esim-wizard/components/EsimWizardFooter'
import WizardCarriersTab from '@/features/products/esim-wizard/components/WizardCarriersTab'
import WizardFeaturesTab from '@/features/products/esim-wizard/components/WizardFeaturesTab'
import WizardPackageTab from '@/features/products/esim-wizard/components/WizardPackageTab'
import WizardPriceTab from '@/features/products/esim-wizard/components/WizardPriceTab'
import WizardReviewTab from '@/features/products/esim-wizard/components/WizardReviewTab'
import WizardVariantTab from '@/features/products/esim-wizard/components/WizardVariantTab'
import { getEsimWizardFormId, useEsimWizardPage } from '@/features/products/esim-wizard/useEsimWizardPage'
import {
  getDefaultPackageValues,
  getDefaultPriceValues,
} from '@/features/products/esim-wizard/wizardDefaults'

const renderFooter = (
  activeTab: Parameters<typeof EsimWizardFooter>[0]['activeTab'],
  isSaving: boolean,
  onContinue: () => void,
) => (
  <div className="border-top mt-4 pt-3">
    <EsimWizardFooter
      activeTab={activeTab}
      isSaving={isSaving}
      formId={getEsimWizardFormId(activeTab)}
      onContinue={onContinue}
    />
  </div>
)

const EsimWizardPage = () => {
  const {
    isNew,
    variantId,
    activeTab,
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

  return (
    <Container fluid>
      <PageBreadcrumb title={pageTitle} subtitle="eSIM du lịch" />

      <EsimWizard
        title={pageTitle}
        activeTab={activeTab}
        canAccessSubTabs={canAccessSubTabs}
        onStepChange={setActiveTab}
        variantStep={
          <>
            <WizardVariantTab
              isNew={isNew}
              productOptions={wizard.productOptions}
              initialValues={wizard.variant}
              onSaved={handleVariantSaved}
              onSavingChange={setIsSaving}
            />
            {renderFooter('variants', isSaving, onContinue)}
          </>
        }
        priceStep={
          <>
            {variantId && wizard.productId ? (
              <WizardPriceTab
                productId={wizard.productId}
                variantId={variantId}
                initialValues={wizard.price ?? getDefaultPriceValues(wizard.productId, variantId)}
                onSaved={() => void handlePriceSaved()}
                onSavingChange={setIsSaving}
              />
            ) : (
              <p className="text-muted mb-0">Lưu biến thể trước khi thiết lập giá.</p>
            )}
            {renderFooter('prices', isSaving, onContinue)}
          </>
        }
        packageStep={
          <>
            {variantId && wizard.productId ? (
              <WizardPackageTab
                productId={wizard.productId}
                variantId={variantId}
                defaultCountryId={wizard.packageForm?.countryId ?? wizard.defaultCountryId}
                countryOptions={wizard.countryOptions}
                initialValues={
                  wizard.packageForm ??
                  getDefaultPackageValues(wizard.productId, variantId, wizard.defaultCountryId)
                }
                onSaved={(id) => void handlePackageSaved(id)}
                onSavingChange={setIsSaving}
              />
            ) : (
              <p className="text-muted mb-0">Lưu biến thể trước khi tạo gói eSIM.</p>
            )}
            {renderFooter('packages', isSaving, onContinue)}
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
              countryId={wizard.packageForm?.countryId ?? wizard.defaultCountryId}
              onChange={wizard.setSelectedCarrierIds}
            />
            {renderFooter('carriers', isSaving, onContinue)}
          </>
        }
        featureStep={
          <>
            {variantId ? (
              <WizardFeaturesTab
                variantId={variantId}
                onRegisterSave={(fn) => {
                  featuresSaveRef.current = fn
                }}
                onSavingChange={setIsSaving}
              />
            ) : (
              <p className="text-muted mb-0">Lưu biến thể trước khi thêm tính năng.</p>
            )}
            {renderFooter('features', isSaving, onContinue)}
          </>
        }
        reviewStep={
          <>
            <WizardReviewTab summary={summary} isNew={isNew} />
            {renderFooter('review', isSaving, onContinue)}
          </>
        }
      />
    </Container>
  )
}

export default EsimWizardPage
