import { Button, Card, CardBody, Col, Row, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'
import { useCallback } from 'react'
import { useNavigate } from 'react-router'

import { CONTENT_ARTICLE_PAGE_SIZE_OPTIONS } from '@/apis/contentArticlesApi'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import ArticleCard from '@/features/content/articles/components/ArticleCard'
import { articlesLabels } from '@/features/content/articles/data'
import {
  type ArticleFeaturedFilter,
  type ArticleStatusFilter,
  useArticlesCrud,
} from '@/features/content/articles/useArticlesCrud'
import { CONTENT_STATUS_OPTIONS } from '@/features/content/shared/contentStatus'
import ListFilterSelect from '@/modules/crud/components/ListFilterSelect'

import './articles-grid.scss'

const FEATURED_FILTER_OPTIONS = [
  { value: 'true', label: 'Nổi bật' },
  { value: 'false', label: 'Không nổi bật' },
] as const

const ArticlesCrudTable = () => {
  const navigate = useNavigate()
  const crud = useArticlesCrud()

  const openEdit = useCallback(
    (id: string) => navigate(`/website/articles/${id}`),
    [navigate],
  )

  return (
    <>
      <Row className="g-3 align-items-start">
        <Col xs={12} lg={3} xl={2}>
          <Card className="articles-filter-card shadow-none border h-100">
            <CardBody className="d-flex flex-column gap-3">
              <div className="app-search">
                <input
                  type="search"
                  className="form-control"
                  placeholder={articlesLabels.searchPlaceholder}
                  value={crud.globalFilter}
                  onChange={(e) => crud.setGlobalFilter(e.target.value)}
                />
                <LuSearch className="app-search-icon text-muted" />
              </div>

              <ListFilterSelect
                label="Danh mục"
                value={crud.categoryFilter}
                onChange={crud.setCategoryFilter}
                options={crud.categoryFilterOptions}
                allLabel="Tất cả danh mục"
                minWidth="0"
              />

              <div className="d-flex flex-column">
                <label className="form-label mb-1 small text-muted">Trạng thái</label>
                <select
                  className="form-select form-select-sm"
                  aria-label="Lọc theo trạng thái"
                  value={crud.statusFilter}
                  onChange={(e) => crud.setStatusFilter(e.target.value as ArticleStatusFilter)}>
                  <option value="">Tất cả trạng thái</option>
                  {CONTENT_STATUS_OPTIONS.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </div>

              <div className="d-flex flex-column">
                <label className="form-label mb-1 small text-muted">Nổi bật</label>
                <select
                  className="form-select form-select-sm"
                  aria-label="Lọc theo nổi bật"
                  value={crud.featuredFilter}
                  onChange={(e) => crud.setFeaturedFilter(e.target.value as ArticleFeaturedFilter)}>
                  <option value="all">Tất cả</option>
                  {FEATURED_FILTER_OPTIONS.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </div>

              <Button variant="primary" className="w-100 mt-1" onClick={() => navigate('/website/articles/new')}>
                <LuPlus className="fs-sm me-1" />
                {articlesLabels.addButton}
              </Button>
            </CardBody>
          </Card>
        </Col>

        <Col xs={12} lg={9} xl={10}>
          {crud.isLoading ? (
            <div className="text-center py-5">
              <Spinner animation="border" size="sm" className="me-2" />
              Đang tải bài viết...
            </div>
          ) : crud.items.length === 0 ? (
            <Card className="shadow-none border">
              <CardBody className="text-center text-muted py-5">{articlesLabels.emptyMessage}</CardBody>
            </Card>
          ) : (
            <Row className="row-cols-1 row-cols-md-2 row-cols-xl-3 g-3">
              {crud.items.map((article) => (
                <Col key={article.id}>
                  <ArticleCard
                    article={article}
                    categoryName={crud.categoryNameByCode.get(article.categoryCode)}
                    onClick={() => openEdit(article.id)}
                    onDelete={crud.requestDelete}
                  />
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
                itemsName={articlesLabels.itemName}
                pageSize={crud.pageSize}
                pageSizeOptions={CONTENT_ARTICLE_PAGE_SIZE_OPTIONS}
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
        itemName={articlesLabels.itemName}
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        Bạn có chắc muốn xóa {articlesLabels.itemName} này?
      </DeleteConfirmationModal>
    </>
  )
}

export default ArticlesCrudTable
