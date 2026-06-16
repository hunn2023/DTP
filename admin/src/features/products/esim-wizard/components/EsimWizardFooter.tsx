import { Button, Spinner } from 'react-bootstrap'
import { Link } from 'react-router'

import type { EsimWizardTab } from '@/features/products/esim-wizard/types'

type EsimWizardFooterProps = {
  isSetupFlow: boolean
  activeTab: EsimWizardTab
  isSaving: boolean
  formId?: string
  onContinue: () => void
  onSave: () => void
}

const EsimWizardFooter = ({
  isSetupFlow,
  activeTab,
  isSaving,
  formId,
  onContinue,
  onSave,
}: EsimWizardFooterProps) => {
  const isReview = activeTab === 'review'
  const showPrimary = isSetupFlow || !isReview
  const primaryLabel = isSetupFlow ? (isReview ? 'Hoàn tất' : 'Tiếp tục →') : 'Lưu'

  const renderPrimaryButton = () => {
    if (isSetupFlow) {
      return (
        <Button type="button" variant="primary" onClick={() => void onContinue()} disabled={isSaving}>
          {isSaving ? (
            <>
              <Spinner animation="border" size="sm" className="me-2" />
              Đang lưu...
            </>
          ) : (
            primaryLabel
          )}
        </Button>
      )
    }

    if (formId) {
      return (
        <Button type="submit" form={formId} variant="primary" disabled={isSaving}>
          {isSaving ? (
            <>
              <Spinner animation="border" size="sm" className="me-2" />
              Đang lưu...
            </>
          ) : (
            primaryLabel
          )}
        </Button>
      )
    }

    return (
      <Button type="button" variant="primary" onClick={() => void onSave()} disabled={isSaving}>
        {isSaving && <Spinner animation="border" size="sm" className="me-2" />}
        {isSaving ? 'Đang lưu...' : primaryLabel}
      </Button>
    )
  }

  return (
    <div className="d-flex align-items-center justify-content-between border-top pt-3 mt-4">
      <Link to="/products/esim/packages" className="btn btn-light">
        Hủy
      </Link>
      {showPrimary && renderPrimaryButton()}
    </div>
  )
}

export default EsimWizardFooter
