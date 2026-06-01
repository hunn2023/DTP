import { Card, Col, Row } from 'react-bootstrap'

import SettingsCrudTable from '@/views/settings/components/SettingsCrudTable'
import SettingsPageLayout from '@/views/settings/components/SettingsPageLayout'
import { readonlyCrudCapabilities } from '@/views/admin-crud/types'
import type { ResolvedAdminEntity } from '@/views/admin-crud/schema/defineEntity'
import type { SettingsEntityBase } from '@/views/settings/types'

type ReportKpi = {
  label: string
  value: string
  hint?: string
}

type ReportPageProps<T extends SettingsEntityBase> = {
  entity: ResolvedAdminEntity<T>
  kpis: ReportKpi[]
}

const ReportPage = <T extends SettingsEntityBase>({ entity, kpis }: ReportPageProps<T>) => {
  return (
    <SettingsPageLayout title={entity.title} subtitle={entity.breadcrumbSubtitle} description={entity.description}>
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
      <SettingsCrudTable
        initialData={entity.initialData}
        buildColumns={entity.buildColumns!}
        formConfig={entity.formConfig}
        labels={{ ...entity.labels, addButton: 'Export Excel' }}
        capabilities={{ ...readonlyCrudCapabilities, create: false, view: true }}
      />
    </SettingsPageLayout>
  )
}

export default ReportPage
