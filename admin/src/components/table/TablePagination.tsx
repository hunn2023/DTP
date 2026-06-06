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

            {Array.from({ length: pageCount }).map((_, index) => (
              <li key={index} className={`page-item ${pageIndex === index ? 'active' : ''}`}>
                <button className="page-link" onClick={() => setPageIndex(index)}>
                  {index + 1}
                </button>
              </li>
            ))}

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
