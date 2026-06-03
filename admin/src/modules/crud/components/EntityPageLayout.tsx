import { Container } from 'react-bootstrap'
import type { ReactNode } from 'react'

import PageBreadcrumb from '@/components/PageBreadcrumb'

type EntityPageLayoutProps = {
  title: string
  subtitle?: string
  description?: string
  children: ReactNode
}

const EntityPageLayout = ({
  title,
  subtitle = 'Cấu hình hệ thống',
  description,
  children,
}: EntityPageLayoutProps) => {
  return (
    <Container fluid>
      <PageBreadcrumb title={title} subtitle={subtitle} />
      {description && <p className="text-muted mb-3">{description}</p>}
      {children}
    </Container>
  )
}

export default EntityPageLayout
