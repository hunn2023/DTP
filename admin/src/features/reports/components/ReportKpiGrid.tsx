import { Card, Col, Row } from 'react-bootstrap'

import { resolveKpiIcon, resolveKpiVariant } from '@/features/reports/reportKpiConfig'
import type { ReportKpi } from '@/features/reports/reportTypes'

type ReportKpiGridProps = {
  items: ReportKpi[]
}

const ReportKpiGrid = ({ items }: ReportKpiGridProps) => (
  <Row className="g-3 mb-4">
    {items.map((kpi, index) => {
      const variant = resolveKpiVariant(index, kpi.variant)
      const Icon = resolveKpiIcon(index)

      return (
        <Col key={kpi.label} xs={12} sm={6} xl={items.length >= 6 ? 4 : items.length >= 4 ? 3 : 4}>
          <Card className={`report-kpi-card report-kpi-card--${variant} h-100`}>
            <Card.Body className="d-flex align-items-start gap-3 py-3">
              <span className={`avatar-sm flex-shrink-0`}>
                <span
                  className={`avatar-title text-bg-${variant}-subtle text-${variant} rounded-circle d-flex align-items-center justify-content-center`}>
                  <Icon size={18} />
                </span>
              </span>
              <div className="min-w-0 flex-grow-1">
                <div className="text-muted fs-xxs text-uppercase text-truncate">{kpi.label}</div>
                <div className="report-kpi-card__value fs-4 fw-bold mt-1 text-truncate">{kpi.value}</div>
                {kpi.hint && <div className="text-muted fs-xs mt-1">{kpi.hint}</div>}
              </div>
            </Card.Body>
          </Card>
        </Col>
      )
    })}
  </Row>
)

export default ReportKpiGrid
