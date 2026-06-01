import { Container } from 'react-bootstrap'
import type { ReactNode } from 'react'

import PageBreadcrumb from '@/components/PageBreadcrumb'

type SettingsPageLayoutProps = {
  title: string
  subtitle?: string
  description?: string
  children: ReactNode
}

const SettingsPageLayout = ({ title, subtitle = 'Cấu hình hệ thống', description, children }: SettingsPageLayoutProps) => {
  return (
    <Container fluid>
      <PageBreadcrumb title={title} subtitle={subtitle} />
      {description && <p className="text-muted mb-3">{description}</p>}
      {children}
    </Container>
  )
}

export default SettingsPageLayout
