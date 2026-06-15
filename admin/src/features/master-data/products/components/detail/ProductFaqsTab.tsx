import { Button, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import { buildFaqColumns } from '@/features/master-data/products/components/detail/faqColumns'
import { useProductFaqsCrud } from '@/features/master-data/products/components/detail/useProductFaqsCrud'
import EntityFormModal from '@/modules/crud/form/EntityFormModal'

type Props = {
  productId: string
  isTabActive: boolean
}

const ProductFaqsTab = ({ productId, isTabActive }: Props) => {
  const crud = useProductFaqsCrud({ productId, isTabActive, buildColumns: buildFaqColumns })

  return (
    <div>
      <div className="d-flex flex-wrap align-items-end justify-content-between gap-3 mb-3">
        <div className="app-search" style={{ maxWidth: 320, minWidth: 200 }}>
          <input
            type="search"
            className="form-control form-control-sm"
            placeholder="Tìm FAQ..."
            value={crud.globalFilter}
            onChange={(e) => crud.setGlobalFilter(e.target.value)}
          />
          <LuSearch className="app-search-icon text-muted" />
        </div>

        <Button
          variant="primary"
          size="sm"
          onClick={crud.openCreate}
          disabled={crud.isFormLoading}
          className="flex-shrink-0">
          <LuPlus className="fs-sm me-1" />
          Thêm FAQ
        </Button>
      </div>

      {crud.isLoading || crud.isFormLoading ? (
        <div className="text-center py-4">
          <Spinner animation="border" size="sm" className="me-2" />
          {crud.isFormLoading ? 'Đang tải chi tiết FAQ...' : 'Đang tải danh sách FAQ...'}
        </div>
      ) : (
        <DataTable table={crud.table} emptyMessage="Chưa có FAQ" />
      )}

      <DeleteConfirmationModal
        show={crud.showDeleteModal}
        onHide={crud.closeDeleteModal}
        onConfirm={() => void crud.confirmDelete()}
        selectedCount={1}
        itemName="FAQ"
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        Bạn có chắc muốn xóa FAQ này?
      </DeleteConfirmationModal>

      {crud.formMode && crud.formValues && !crud.isFormLoading && (
        <EntityFormModal
          show
          mode={crud.formMode}
          entityName={crud.formConfig.entityName}
          fields={crud.formConfig.fields}
          initialValues={crud.formValues}
          onHide={crud.closeFormModal}
          onSubmit={(values) => void crud.saveForm(values)}
        />
      )}
    </div>
  )
}

export default ProductFaqsTab
