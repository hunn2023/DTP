import type { CSSProperties, ReactNode } from 'react'
import { Card, Spinner } from 'react-bootstrap'

import { TABLE_ACCENT_ICONS } from '@/features/reports/reportKpiConfig'
import type { ReportTableAccent } from '@/features/reports/reportTypes'

type ReportTableColumn<T> = {
  key: string
  header: string
  cell: (row: T) => ReactNode
  className?: string
}

type ReportDataTableProps<T> = {
  title: string
  columns: ReportTableColumn<T>[]
  rows: T[]
  emptyMessage?: string
  isLoading?: boolean
  accent?: ReportTableAccent
  rowKey: (row: T, index: number) => string
}

function resolveColumnWidths(columnCount: number): string {
  if (columnCount === 3) return '46% 30% 24%'
  if (columnCount === 2) return '62% 38%'
  return `repeat(${columnCount}, minmax(0, 1fr))`
}

function ReportDataTable<T>({
  title,
  columns,
  rows,
  emptyMessage = 'Chưa có dữ liệu',
  isLoading = false,
  accent = 'primary',
  rowKey,
}: ReportDataTableProps<T>) {
  const Icon = TABLE_ACCENT_ICONS[accent]
  const gridColumns = resolveColumnWidths(columns.length)

  return (
    <Card className={`report-data-card report-data-card--${accent}`}>
      <Card.Header className="report-data-card__header flex-shrink-0">
        <div className="d-flex align-items-center gap-2">
          <span className="report-data-card__icon">
            <Icon size={16} />
          </span>
          <Card.Title as="h6" className="mb-0 fw-semibold">
            {title}
          </Card.Title>
          {!isLoading && rows.length > 0 && (
            <span className="badge bg-light text-muted ms-auto">{rows.length} dòng</span>
          )}
        </div>
      </Card.Header>
      <Card.Body className="report-data-card__body p-0">
        {isLoading ? (
          <div className="text-center py-4">
            <Spinner animation="border" size="sm" className="me-2" />
            Đang tải...
          </div>
        ) : rows.length === 0 ? (
          <div className="text-center text-muted py-5">{emptyMessage}</div>
        ) : (
          <div
            className="report-data-card__table-wrap"
            style={{ '--report-grid-cols': gridColumns } as CSSProperties}>
            <div className="report-data-card__head" role="rowgroup">
              <div className="report-data-card__row report-data-card__row--head" role="row">
                {columns.map((col) => (
                  <div
                    key={col.key}
                    role="columnheader"
                    className={`report-data-card__cell report-data-card__cell--head ${col.className ?? ''}`}>
                    {col.header}
                  </div>
                ))}
              </div>
            </div>
            <div className="report-data-card__scroll" role="rowgroup">
              {rows.map((row, index) => (
                <div
                  key={`${rowKey(row, index)}-${index}`}
                  className="report-data-card__row report-data-card__row--body"
                  role="row">
                  {columns.map((col) => (
                    <div
                      key={col.key}
                      role="cell"
                      className={`report-data-card__cell ${col.className ?? ''}`}>
                      {col.cell(row)}
                    </div>
                  ))}
                </div>
              ))}
            </div>
          </div>
        )}
      </Card.Body>
    </Card>
  )
}

export type { ReportTableColumn }
export default ReportDataTable
