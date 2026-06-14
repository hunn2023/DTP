import { Button, Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'
import { useCallback } from 'react'
import { useNavigate } from 'react-router'

import { CONTENT_BANNER_PAGE_SIZE_OPTIONS } from '@/apis/contentBannersApi'
import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import { buildBannerColumns } from '@/features/content/banners/columns'
import { bannersLabels } from '@/features/content/banners/data'
import { useBannersCrud } from '@/features/content/banners/useBannersCrud'
import type { ContentBanner } from '@/features/content/types'

const BannersCrudTable = () => {
  const navigate = useNavigate()

  const openEdit = useCallback(
    (row: ContentBanner) => {
      navigate(`/website/banners/${row.id}`)
    },
    [navigate],
  )

  const crud = useBannersCrud({
    buildColumns: buildBannerColumns,
    onEdit: openEdit,
  })

  const deleteMessage =
    crud.pendingDeleteCount > 1
      ? `Bạn có chắc muốn xóa ${crud.pendingDeleteCount} ${bannersLabels.itemName} đã chọn?`
      : `Bạn có chắc muốn xóa ${bannersLabels.itemName} này?`

  const statusColumn = crud.table.getColumn('isActive')

  return (
    <Card>
      <CardHeader className="border-light justify-content-between">
        <div className="d-flex align-items-center gap-2 flex-wrap">
          <div className="app-search">
            <input
              type="search"
              className="form-control"
              placeholder={bannersLabels.searchPlaceholder}
              value={crud.globalFilter}
              onChange={(e) => crud.setGlobalFilter(e.target.value)}
            />
            <LuSearch className="app-search-icon text-muted" />
          </div>
          {crud.selectedCount > 0 && (
            <Button variant="danger" size="sm" onClick={crud.requestBulkDelete}>
              Xóa ({crud.selectedCount})
            </Button>
          )}
        </div>
        <div className="card-action d-flex flex-nowrap align-items-center gap-2">
          {statusColumn && (
            <select
              className="form-select form-select-sm"
              style={{ minWidth: '9.75rem', width: 'auto' }}
              aria-label="Lọc theo trạng thái"
              value={String(statusColumn.getFilterValue() ?? 'all')}
              onChange={(e) => {
                const value = e.target.value
                statusColumn.setFilterValue(value === 'all' ? undefined : value === 'true')
              }}>
              <option value="all">Tất cả</option>
              <option value="true">Hoạt động</option>
              <option value="false">Ngưng hoạt động</option>
            </select>
          )}
          <Button
            variant="primary"
            size="sm"
            className="text-nowrap"
            onClick={() => navigate('/website/banners/new')}>
            <LuPlus className="fs-sm me-1" />
            {bannersLabels.addButton}
          </Button>
        </div>
      </CardHeader>

      {crud.isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải banner...
        </div>
      ) : (
        <DataTable
          table={crud.table}
          emptyMessage={bannersLabels.emptyMessage}
          onRowClick={openEdit}
        />
      )}

      {!crud.isLoading && crud.paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={crud.paginationInfo.total}
            start={crud.paginationInfo.start}
            end={crud.paginationInfo.end}
            itemsName={bannersLabels.itemName}
            pageSize={crud.pageSize}
            pageSizeOptions={CONTENT_BANNER_PAGE_SIZE_OPTIONS}
            onPageSizeChange={crud.setPageSize}
            previousPage={crud.table.previousPage}
            canPreviousPage={crud.table.getCanPreviousPage()}
            pageCount={crud.pageCount}
            pageIndex={crud.table.getState().pagination.pageIndex}
            setPageIndex={crud.table.setPageIndex}
            nextPage={crud.table.nextPage}
            canNextPage={crud.table.getCanNextPage()}
          />
        </CardFooter>
      )}

      <DeleteConfirmationModal
        show={crud.showDeleteModal}
        onHide={crud.closeDeleteModal}
        onConfirm={() => void crud.confirmDelete()}
        selectedCount={crud.pendingDeleteCount}
        itemName={bannersLabels.itemName}
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        {deleteMessage}
      </DeleteConfirmationModal>
    </Card>
  )
}

export default BannersCrudTable
