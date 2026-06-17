import { Badge, Button, Card, Col, Form, Row } from 'react-bootstrap'
import { LuCalendarRange, LuFilter } from 'react-icons/lu'

import { parseReportGroupType } from '@/features/reports/reportDateRange'
import { REPORT_GROUP_TYPE_LABELS, type ReportFilters } from '@/features/reports/reportTypes'

type ReportDateFilterProps = {
  value: ReportFilters
  onChange: (next: ReportFilters) => void
  onApply: () => void
  isLoading?: boolean
  showGroupType?: boolean
  title?: string
}

const ReportDateFilter = ({
  value,
  onChange,
  onApply,
  isLoading = false,
  showGroupType = true,
  title = 'Bộ lọc báo cáo',
}: ReportDateFilterProps) => {
  const patch = (partial: Partial<ReportFilters>) => onChange({ ...value, ...partial })

  return (
    <Card className="report-filter-card mb-4">
      <Card.Body className="py-3">
        <div className="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-3">
          <div className="d-flex align-items-center gap-2 text-primary">
            <LuFilter size={18} />
            <span className="fw-semibold fs-sm report-filter-card__title text-uppercase">{title}</span>
          </div>
          <div className="report-range-badges d-flex flex-wrap gap-2">
            <Badge bg="primary-subtle" text="primary" className="d-inline-flex align-items-center gap-1">
              <LuCalendarRange size={14} />
              {value.fromDate} → {value.toDate}
            </Badge>
            {showGroupType && (
              <Badge bg="info-subtle" text="info">
                {REPORT_GROUP_TYPE_LABELS[value.groupType]}
              </Badge>
            )}
          </div>
        </div>
        <Row className="g-2 align-items-end">
          <Col xs={12} sm={6} md={3} lg={2}>
            <Form.Label className="text-muted fs-xxs text-uppercase mb-1">Từ ngày</Form.Label>
            <Form.Control
              type="date"
              value={value.fromDate}
              onChange={(e) => patch({ fromDate: e.target.value })}
            />
          </Col>
          <Col xs={12} sm={6} md={3} lg={2}>
            <Form.Label className="text-muted fs-xxs text-uppercase mb-1">Đến ngày</Form.Label>
            <Form.Control
              type="date"
              value={value.toDate}
              onChange={(e) => patch({ toDate: e.target.value })}
            />
          </Col>
          {showGroupType && (
            <Col xs={12} sm={6} md={3} lg={2}>
              <Form.Label className="text-muted fs-xxs text-uppercase mb-1">Nhóm theo</Form.Label>
              <Form.Select
                value={value.groupType}
                onChange={(e) => patch({ groupType: parseReportGroupType(e.target.value) })}>
                {(Object.entries(REPORT_GROUP_TYPE_LABELS) as [string, string][]).map(([key, label]) => (
                  <option key={key} value={key}>
                    {label}
                  </option>
                ))}
              </Form.Select>
            </Col>
          )}
          <Col xs={12} md="auto">
            <Button variant="primary" className="px-4" disabled={isLoading} onClick={onApply}>
              {isLoading ? 'Đang tải...' : 'Áp dụng'}
            </Button>
          </Col>
        </Row>
      </Card.Body>
    </Card>
  )
}

export default ReportDateFilter
