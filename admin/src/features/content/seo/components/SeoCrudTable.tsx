import { Button, Card, CardBody, Col, Row, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'
import { useCallback } from 'react'
import { useNavigate } from 'react-router'

import { SEO_METADATA_PAGE_SIZE_OPTIONS } from '@/apis/seoMetadataApi'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import SeoCard from '@/features/content/seo/components/SeoCard'
import { SEO_ENTITY_TYPE_OPTIONS, seoLabels } from '@/features/content/seo/data'
import { useSeoCrud } from '@/features/content/seo/useSeoCrud'
import ListFilterSelect from '@/modules/crud/components/ListFilterSelect'

import './seo-grid.scss'

const SeoCrudTable = () => {
  const navigate = useNavigate()
  const crud = useSeoCrud()

  const openEdit = useCallback((id: string) => navigate(`/website/seo/${id}`), [navigate])

  return (
    <>
      <Row className="g-3 align-items-start">
        <Col xs={12} lg={3} xl={2}>
          <Card className="seo-filter-card shadow-none border h-100">
            <CardBody className="d-flex flex-column gap-3">
              <div className="app-search">
                <input
                  type="search"
                  className="form-control"
                  placeholder={seoLabels.searchPlaceholder}
                  value={crud.globalFilter}
                  onChange={(e) => crud.setGlobalFilter(e.target.value)}
                />
                <LuSearch className="app-search-icon text-muted" />
              </div>

              <ListFilterSelect
                label="Entity type"
                value={crud.entityTypeFilter}
                onChange={crud.setEntityTypeFilter}
                options={[...SEO_ENTITY_TYPE_OPTIONS]}
                allLabel="Tất cả entity"
                minWidth="0"
              />

              <Button variant="primary" className="w-100 mt-1" onClick={() => navigate('/website/seo/new')}>
                <LuPlus className="fs-sm me-1" />
                {seoLabels.addButton}
              </Button>
            </CardBody>
          </Card>
        </Col>

        <Col xs={12} lg={9} xl={10}>
          {crud.isLoading ? (
            <div className="text-center py-5">
              <Spinner animation="border" size="sm" className="me-2" />
              Đang tải SEO...
            </div>
          ) : crud.items.length === 0 ? (
            <Card className="shadow-none border">
              <CardBody className="text-center text-muted py-5">{seoLabels.emptyMessage}</CardBody>
            </Card>
          ) : (
            <Row className="row-cols-1 row-cols-md-2 row-cols-xl-3 g-3">
              {crud.items.map((item) => (
                <Col key={item.id}>
                  <SeoCard item={item} onClick={() => openEdit(item.id)} onDelete={crud.requestDelete} />
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
                itemsName={seoLabels.itemName}
                pageSize={crud.pageSize}
                pageSizeOptions={SEO_METADATA_PAGE_SIZE_OPTIONS}
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
        itemName={seoLabels.itemName}
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        Bạn có chắc muốn xóa {seoLabels.itemName} này?
      </DeleteConfirmationModal>
    </>
  )
}

export default SeoCrudTable
