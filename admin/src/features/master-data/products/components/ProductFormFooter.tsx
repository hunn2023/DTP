import { Button, Spinner } from 'react-bootstrap'
import { Link } from 'react-router'

import type { ProductFormTab } from '@/features/master-data/products/types'

type ProductFormFooterProps = {
  activeTab: ProductFormTab
  isSaving: boolean
  onContinue: () => void
}

const ProductFormFooter = ({ activeTab, isSaving, onContinue }: ProductFormFooterProps) => {
  const isLastStep = activeTab === 'contents'
  const continueLabel = isLastStep ? 'Hoàn tất' : 'Tiếp tục →'

  return (
    <div className="d-flex align-items-center justify-content-between border-top pt-3 mt-4">
      <Link to="/settings/products" className="btn btn-light">
        Hủy
      </Link>
      {activeTab === 'product' ? (
        <Button type="submit" form="product-info-form" variant="primary" disabled={isSaving}>
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

export default ProductFormFooter
