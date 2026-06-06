import { Button, Card, CardHeader, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import { buildImageColumns } from '@/features/master-data/products/detail/imageColumns'
import { useProductImagesCrud } from '@/features/master-data/products/detail/useProductImagesCrud'
import EntityFormModal from '@/modules/crud/form/EntityFormModal'

type Props = { productId: string }

const ProductImagesTab = ({ productId }: Props) => {
  const crud = useProductImagesCrud({ productId, buildColumns: buildImageColumns })

  return (
    <Card className="border-0 shadow-none">
      <CardHeader className="border-light justify-content-between px-0">
        <div className="app-search">
          <input
            type="search"
            className="form-control form-control-sm"
            placeholder="Tìm URL, alt..."
            value={crud.globalFilter}
            onChange={(e) => crud.setGlobalFilter(e.target.value)}
          />
          <LuSearch className="app-search-icon text-muted" />
        </div>
        <Button variant="primary" size="sm" onClick={crud.openCreate}>
          <LuPlus className="fs-sm me-1" />
          Thêm ảnh
        </Button>
      </CardHeader>

      {crud.isLoading ? (
        <div className="text-center py-4">
          <Spinner animation="border" size="sm" />
        </div>
      ) : (
        <DataTable table={crud.table} emptyMessage="Chưa có hình ảnh" />
      )}

      <DeleteConfirmationModal
        show={crud.showDeleteModal}
        onHide={crud.closeDeleteModal}
        onConfirm={() => void crud.confirmDelete()}
        selectedCount={1}
        itemName="hình ảnh"
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        Bạn có chắc muốn xóa hình ảnh này?
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
    </Card>
  )
}

export default ProductImagesTab
