import { Card, Col, Row } from 'react-bootstrap'

import EntityCrudTable from '@/modules/crud/components/EntityCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'
import type { ResolvedAdminEntity } from '@/modules/crud/schema/defineEntity'
import { readonlyCrudCapabilities, type CrudEntityBase } from '@/modules/crud/types'

type ReportKpi = {
  label: string
  value: string
  hint?: string
}

type ReportPageProps<T extends CrudEntityBase> = {
  entity: ResolvedAdminEntity<T>
  kpis: ReportKpi[]
}

const ReportPage = <T extends CrudEntityBase>({ entity, kpis }: ReportPageProps<T>) => {
  return (
    <EntityPageLayout title={entity.title} subtitle={entity.breadcrumbSubtitle} description={entity.description}>
      <Row className="g-3 mb-3">
        {kpis.map((kpi) => (
          <Col key={kpi.label} md={4} sm={6}>
            <Card className="border-light shadow-none">
              <Card.Body>
                <div className="text-muted fs-xs text-uppercase">{kpi.label}</div>
                <div className="fs-4 fw-bold mt-1">{kpi.value}</div>
                {kpi.hint && <div className="text-muted fs-xs mt-1">{kpi.hint}</div>}
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
      <EntityCrudTable
        initialData={entity.initialData}
        buildColumns={entity.buildColumns!}
        formConfig={entity.formConfig}
        labels={{ ...entity.labels, addButton: 'Export Excel' }}
        capabilities={{ ...readonlyCrudCapabilities, create: false, view: true }}
      />
    </EntityPageLayout>
  )
}

export default ReportPage
