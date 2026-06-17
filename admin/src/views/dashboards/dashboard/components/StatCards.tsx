import { useMemo } from 'react'
import { Card, CardBody, Col, Row } from 'react-bootstrap'

import { buildStatCards } from '@/features/dashboard/dashboardStatCards'
import { useDashboard } from '@/features/dashboard/DashboardContext'

const StatCards = () => {
  const { dashboard, paymentsReport } = useDashboard()
  const cards = useMemo(() => buildStatCards(dashboard, paymentsReport), [dashboard, paymentsReport])

  return (
    <Row className="row-cols-xxl-4 row-cols-md-2 row-cols-1 g-3 mb-3">
      {cards.map((card) => {
        const Icon = card.icon
        return (
          <Col key={card.id}>
            <Card className="h-100 dashboard-stat-card">
              <CardBody>
                <div className="d-flex align-items-center gap-2 mb-3">
                  <div className="avatar-sm flex-shrink-0">
                    <span
                      className={`avatar-title text-bg-${card.iconBg}-subtle text-${card.iconBg} rounded-circle fs-18 d-flex align-items-center justify-content-center`}>
                      <Icon size={18} />
                    </span>
                  </div>
                  <p className="text-muted mb-0 fw-medium">{card.title}</p>
                </div>
                <div className="dashboard-stat-card__metrics">
                  {card.metrics.length === 0 ? (
                    <div className="text-muted fs-sm">—</div>
                  ) : (
                    card.metrics.map((metric) => (
                      <div key={metric.label} className="dashboard-stat-card__row">
                        <span className="text-muted fs-sm">{metric.label}</span>
                        <span className="fw-semibold fs-sm text-end">{metric.value}</span>
                      </div>
                    ))
                  )}
                </div>
              </CardBody>
            </Card>
          </Col>
        )
      })}
    </Row>
  )
}

export default StatCards
