import { Button, Spinner } from 'react-bootstrap'
import { Link } from 'react-router'

import type { ProductFormTab } from '@/features/master-data/products/types'

type ProductFormFooterProps = {
  activeTab: ProductFormTab
  isNew: boolean
  isSaving: boolean
}

const ProductFormFooter = ({ activeTab, isNew, isSaving }: ProductFormFooterProps) => {
  const showContinue = isNew && activeTab === 'product'
  const showSave = activeTab === 'product' && !showContinue

  return (
    <div className="d-flex align-items-center justify-content-between border-top pt-3 mt-4">
      <Link to="/settings/products" className="btn btn-light">
        Quay lại danh sách
      </Link>
      {showContinue && (
        <Button type="submit" form="product-info-form" variant="primary" disabled={isSaving}>
          {isSaving ? (
            <>
              <Spinner animation="border" size="sm" className="me-2" />
              Đang lưu...
            </>
          ) : (
            'Tiếp tục →'
          )}
        </Button>
      )}
      {showSave && (
        <Button type="submit" form="product-info-form" variant="primary" disabled={isSaving}>
          {isSaving ? (
            <>
              <Spinner animation="border" size="sm" className="me-2" />
              Đang lưu...
            </>
          ) : (
            'Lưu'
          )}
        </Button>
      )}
    </div>
  )
}

export default ProductFormFooter
