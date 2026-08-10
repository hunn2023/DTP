import clsx from 'clsx'
import { Col, Row } from 'react-bootstrap'
import { TbChevronLeft, TbChevronRight } from 'react-icons/tb'

import TablePageSizeSelect from '@/components/table/TablePageSizeSelect'

export type TablePaginationProps = {
  totalItems: number
  start: number
  end: number
  itemsName?: string
  showInfo?: boolean
  pageSize?: number
  pageSizeOptions?: readonly number[]
  onPageSizeChange?: (size: number) => void
  previousPage: () => void
  canPreviousPage: boolean
  pageCount: number
  pageIndex: number
  setPageIndex: (index: number) => void
  nextPage: () => void
  canNextPage: boolean
}

type PaginationItem = number | 'ellipsis-start' | 'ellipsis-end'

function getPaginationItems(pageCount: number, pageIndex: number): PaginationItem[] {
  if (pageCount <= 7) return Array.from({ length: Math.max(0, pageCount) }, (_, index) => index)

  const current = Math.min(Math.max(pageIndex, 0), pageCount - 1)
  const last = pageCount - 1

  if (current <= 3) return [0, 1, 2, 3, 4, 'ellipsis-end', last]
  if (current >= last - 3) return [0, 'ellipsis-start', last - 4, last - 3, last - 2, last - 1, last]

  return [0, 'ellipsis-start', current - 1, current, current + 1, 'ellipsis-end', last]
}

const TablePagination = ({
  totalItems,
  start,
  end,
  itemsName = 'mục',
  showInfo,
  pageSize,
  pageSizeOptions,
  onPageSizeChange,
  previousPage,
  canPreviousPage,
  pageCount,
  pageIndex,
  setPageIndex,
  nextPage,
  canNextPage,
}: TablePaginationProps) => {
  const showPageSizeSelect =
    pageSize !== undefined &&
    pageSizeOptions !== undefined &&
    pageSizeOptions.length > 0 &&
    onPageSizeChange !== undefined

  const showLeftColumn = showPageSizeSelect || showInfo
  const paginationItems = getPaginationItems(pageCount, pageIndex)

  return (
    <Row
      className={clsx(
        'align-items-center text-center text-sm-start',
        showLeftColumn ? 'justify-content-between' : 'justify-content-end',
      )}>
      {showPageSizeSelect && (
        <Col sm="auto" className="mt-3 mt-sm-0">
          <TablePageSizeSelect
            pageSize={pageSize}
            pageSizeOptions={pageSizeOptions}
            onPageSizeChange={onPageSizeChange}
          />
        </Col>
      )}
      {!showPageSizeSelect && showInfo && (
        <Col sm>
          <div className="text-muted">
            Hiển thị <span className="fw-semibold">{start}</span>–<span className="fw-semibold">{end}</span> trong tổng{' '}
            <span className="fw-semibold">{totalItems}</span> {itemsName}
          </div>
        </Col>
      )}
      <Col sm="auto" className="mt-3 mt-sm-0">
        <div>
          <ul className="pagination pagination-boxed mb-0 justify-content-center">
            <li className="page-item">
              <button className="page-link" onClick={() => previousPage()} disabled={!canPreviousPage}>
                <TbChevronLeft />
              </button>
            </li>

            {paginationItems.map((item) => {
              if (typeof item !== 'number') {
                return (
                  <li key={item} className="page-item disabled" aria-hidden="true">
                    <span className="page-link">…</span>
                  </li>
                )
              }

              return (
                <li key={item} className={`page-item ${pageIndex === item ? 'active' : ''}`}>
                  <button
                    className="page-link"
                    onClick={() => setPageIndex(item)}
                    aria-label={`Trang ${item + 1}`}
                    aria-current={pageIndex === item ? 'page' : undefined}>
                    {item + 1}
                  </button>
                </li>
              )
            })}

            <li className="page-item">
              <button className="page-link" onClick={() => nextPage()} disabled={!canNextPage}>
                <TbChevronRight />
              </button>
            </li>
          </ul>
        </div>
      </Col>
    </Row>
  )
}

export default TablePagination
