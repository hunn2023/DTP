import { Button, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import { buildAttributeColumns } from '@/features/master-data/products/detail/attributeColumns'
import { useProductAttributesCrud } from '@/features/master-data/products/detail/useProductAttributesCrud'
import EntityFormModal from '@/modules/crud/form/EntityFormModal'

type Props = { productId: string }

const ProductAttributesTab = ({ productId }: Props) => {
  const crud = useProductAttributesCrud({ productId, buildColumns: buildAttributeColumns })

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-3">
        <h5 className="mb-0 fw-semibold">Thuộc tính sản phẩm</h5>
        <Button variant="primary" size="sm" onClick={crud.openCreate}>
          <LuPlus className="fs-sm me-1" />
          Thêm thuộc tính
        </Button>
      </div>

      <div className="app-search mb-3" style={{ maxWidth: 320 }}>
        <input
          type="search"
          className="form-control form-control-sm"
          placeholder="Tìm thuộc tính..."
          value={crud.globalFilter}
          onChange={(e) => crud.setGlobalFilter(e.target.value)}
        />
        <LuSearch className="app-search-icon text-muted" />
      </div>

      {crud.isLoading ? (
        <div className="text-center py-4">
          <Spinner animation="border" size="sm" />
        </div>
      ) : (
        <DataTable table={crud.table} emptyMessage="Chưa có thuộc tính" />
      )}

      <DeleteConfirmationModal
        show={crud.showDeleteModal}
        onHide={crud.closeDeleteModal}
        onConfirm={() => void crud.confirmDelete()}
        selectedCount={1}
        itemName="thuộc tính"
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        Bạn có chắc muốn xóa thuộc tính này?
      </DeleteConfirmationModal>

      {crud.formMode && crud.formValues && (
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

export default ProductAttributesTab
