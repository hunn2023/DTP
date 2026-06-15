import { Button, Card, CardBody, Col, Row, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'
import { useCallback } from 'react'
import { useNavigate } from 'react-router'

import { CONTENT_PAGE_PAGE_SIZE_OPTIONS } from '@/apis/contentPagesApi'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import PageCard from '@/features/content/pages/components/PageCard'
import { pagesLabels } from '@/features/content/pages/data'
import { type PageStatusFilter, usePagesCrud } from '@/features/content/pages/usePagesCrud'
import { CONTENT_STATUS_OPTIONS } from '@/features/content/shared/contentStatus'

import './pages-grid.scss'

const PagesCrudTable = () => {
  const navigate = useNavigate()
  const crud = usePagesCrud()

  const openEdit = useCallback((id: string) => navigate(`/website/pages/${id}`), [navigate])

  return (
    <>
      <Row className="g-3 align-items-start">
        <Col xs={12} lg={3} xl={2}>
          <Card className="pages-filter-card shadow-none border h-100">
            <CardBody className="d-flex flex-column gap-3">
              <div className="app-search">
                <input
                  type="search"
                  className="form-control"
                  placeholder={pagesLabels.searchPlaceholder}
                  value={crud.globalFilter}
                  onChange={(e) => crud.setGlobalFilter(e.target.value)}
                />
                <LuSearch className="app-search-icon text-muted" />
              </div>

              <div className="d-flex flex-column">
                <label className="form-label mb-1 small text-muted">Trạng thái</label>
                <select
                  className="form-select form-select-sm"
                  aria-label="Lọc theo trạng thái"
                  value={crud.statusFilter}
                  onChange={(e) => crud.setStatusFilter(e.target.value as PageStatusFilter)}>
                  <option value="">Tất cả trạng thái</option>
                  {CONTENT_STATUS_OPTIONS.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </div>

              <Button variant="primary" className="w-100 mt-1" onClick={() => navigate('/website/pages/new')}>
                <LuPlus className="fs-sm me-1" />
                {pagesLabels.addButton}
              </Button>
            </CardBody>
          </Card>
        </Col>

        <Col xs={12} lg={9} xl={10}>
          {crud.isLoading ? (
            <div className="text-center py-5">
              <Spinner animation="border" size="sm" className="me-2" />
              Đang tải trang tĩnh...
            </div>
          ) : crud.items.length === 0 ? (
            <Card className="shadow-none border">
              <CardBody className="text-center text-muted py-5">{pagesLabels.emptyMessage}</CardBody>
            </Card>
          ) : (
            <Row className="row-cols-1 row-cols-md-2 row-cols-xl-3 g-3">
              {crud.items.map((page) => (
                <Col key={page.id}>
                  <PageCard page={page} onClick={() => openEdit(page.id)} onDelete={crud.requestDelete} />
                </Col>
              ))}
            </Row>
          )}

          {!crud.isLoading && crud.paginationInfo.total > 0 && (
            <div className="mt-3">
              <TablePagination
                totalItems={crud.paginationInfo.total}
                start={crud.paginationInfo.start}
                end={crud.paginationInfo.end}
                itemsName={pagesLabels.itemName}
                pageSize={crud.pageSize}
                pageSizeOptions={CONTENT_PAGE_PAGE_SIZE_OPTIONS}
                onPageSizeChange={crud.setPageSize}
                previousPage={crud.table.previousPage}
                canPreviousPage={crud.table.getCanPreviousPage()}
                pageCount={crud.pageCount}
                pageIndex={crud.table.getState().pagination.pageIndex}
                setPageIndex={crud.table.setPageIndex}
                nextPage={crud.table.nextPage}
                canNextPage={crud.table.getCanNextPage()}
              />
            </div>
          )}
        </Col>
      </Row>

      <DeleteConfirmationModal
        show={crud.showDeleteModal}
        onHide={crud.closeDeleteModal}
        onConfirm={() => void crud.confirmDelete()}
        selectedCount={crud.pendingDeleteCount}
        itemName={pagesLabels.itemName}
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        Bạn có chắc muốn xóa {pagesLabels.itemName} này?
      </DeleteConfirmationModal>
    </>
  )
}

export default PagesCrudTable
