import { Button, Spinner } from 'react-bootstrap'
import { Link } from 'react-router'

import type { EsimWizardTab } from '@/features/products/esim-wizard/types'

type EsimWizardFooterProps = {
  activeTab: EsimWizardTab
  isSaving: boolean
  formId?: string
  onContinue: () => void
}

const EsimWizardFooter = ({ activeTab, isSaving, formId, onContinue }: EsimWizardFooterProps) => {
  const isLastStep = activeTab === 'review'
  const continueLabel = isLastStep ? 'Quay về danh sách' : 'Tiếp tục →'

  return (
    <div className="d-flex align-items-center justify-content-between border-top pt-3 mt-4">
      <Link to="/products/esim/packages" className="btn btn-light">
        Hủy
      </Link>
      {formId ? (
        <Button type="submit" form={formId} variant="primary" disabled={isSaving}>
          {isSaving ? (
            <>
              <Spinner animation="border" size="sm" className="me-2" />
              Đang lưu...
            </>
          ) : (
            continueLabel
          )}
        </Button>
      ) : (
        <Button type="button" variant="primary" onClick={onContinue}>
          {continueLabel}
        </Button>
      )}
    </div>
  )
}

export default EsimWizardFooter
