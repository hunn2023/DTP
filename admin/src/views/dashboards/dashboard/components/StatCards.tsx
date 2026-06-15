import { Card, CardBody, Col, Row } from 'react-bootstrap'
import { TbArrowDown, TbArrowUp } from 'react-icons/tb'

import { statCards } from '../data'

const StatCards = () => (
  <Row className="row-cols-xxl-5 row-cols-md-3 row-cols-1 g-3 mb-3">
    {statCards.map((card) => {
      const Icon = card.icon
      const TrendIcon = card.trendUp ? TbArrowUp : TbArrowDown
      return (
        <Col key={card.id}>
          <Card className="h-100">
            <CardBody>
              <p className="text-muted mb-2 mb-xxl-3">{card.title}</p>
              <div className="d-flex align-items-center gap-3">
                <div className="avatar-md flex-shrink-0">
                  <span className={`avatar-title text-bg-${card.iconBg}-subtle text-${card.iconBg} rounded-circle fs-22`}>
                    <Icon />
                  </span>
                </div>
                <div className="flex-grow-1 min-w-0">
                  <h3 className="mb-1 fs-4 text-truncate">{card.value}</h3>
                  <span className={`fs-xs d-inline-flex align-items-center gap-1 ${card.trendUp ? 'text-success' : 'text-danger'}`}>
                    <TrendIcon size={14} />
                    {card.trendUp ? '+' : '-'}
                    {card.trend.toLocaleString('vi-VN')}% so với hôm qua
                  </span>
                </div>
              </div>
            </CardBody>
          </Card>
        </Col>
      )
    })}
  </Row>
)

export default StatCards
