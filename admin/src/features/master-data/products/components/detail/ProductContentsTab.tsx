import { Button, Card, CardBody, Col, Row, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import ProductContentCard from '@/features/master-data/products/components/detail/ProductContentCard'
import { useProductContentsList } from '@/features/master-data/products/components/detail/useProductContentsList'
import ListFilterSelect from '@/modules/crud/components/ListFilterSelect'

import './product-contents-grid.scss'

type Props = {
  productId: string
  isTabActive: boolean
  onOpenCreate: () => void
  onOpenEdit: (contentId: string) => void
}

const ProductContentsTab = ({ productId, isTabActive, onOpenCreate, onOpenEdit }: Props) => {
  const list = useProductContentsList({ productId, isTabActive })

  return (
    <div>
      <div className="d-flex flex-wrap align-items-end justify-content-between gap-3 mb-3">
        <div className="d-flex flex-wrap align-items-end gap-3">
          <div className="app-search" style={{ maxWidth: 280, minWidth: 200 }}>
            <input
              type="search"
              className="form-control form-control-sm"
              placeholder="Tìm nội dung..."
              value={list.globalFilter}
              onChange={(e) => list.setGlobalFilter(e.target.value)}
            />
            <LuSearch className="app-search-icon text-muted" />
          </div>

          <ListFilterSelect
            label="Loại nội dung"
            value={list.contentTypeFilter}
            onChange={list.setContentTypeFilter}
            options={list.contentTypeOptions}
            allLabel="Tất cả loại"
            minWidth="11rem"
          />

          <ListFilterSelect
            label="Trạng thái"
            value={list.onlyActiveFilter}
            onChange={list.setOnlyActiveFilter}
            options={[...list.onlyActiveOptions]}
            allLabel="Tất cả"
            minWidth="9rem"
          />
        </div>

        <Button variant="primary" size="sm" onClick={onOpenCreate} className="flex-shrink-0">
          <LuPlus className="fs-sm me-1" />
          Thêm nội dung
        </Button>
      </div>

      {list.isLoading ? (
        <div className="text-center py-4">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải nội dung...
        </div>
      ) : list.items.length === 0 ? (
        <Card className="shadow-none border">
          <CardBody className="text-center text-muted py-5">Chưa có nội dung</CardBody>
        </Card>
      ) : (
        <Row className="row-cols-1 row-cols-md-2 g-3 product-contents-list">
          {list.items.map((item) => (
            <Col key={item.id}>
              <ProductContentCard
                item={item}
                onClick={() => onOpenEdit(item.id)}
                onDelete={list.requestDelete}
              />
            </Col>
          ))}
        </Row>
      )}

      <DeleteConfirmationModal
        show={list.showDeleteModal}
        onHide={list.closeDeleteModal}
        onConfirm={() => void list.confirmDelete()}
        selectedCount={1}
        itemName="nội dung"
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        Bạn có chắc muốn xóa nội dung này?
      </DeleteConfirmationModal>
    </div>
  )
}

export default ProductContentsTab
