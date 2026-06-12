import { Card, CardHeader, Nav, Tab } from 'react-bootstrap'

import { usePermissionsPage } from '@/features/system/permissions/usePermissionsPage'
import {
  PermissionsByModulePanel,
  PermissionsListPanel,
} from '@/features/system/permissions/components/PermissionsPanels'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const PermissionsPage = () => {
  const page = usePermissionsPage()

  return (
    <EntityPageLayout
      title="Phân quyền"
      subtitle="Hệ thống"
      description="Danh sách permission và nhóm theo module (read-only).">
      <Card className="border-0 shadow-none">
        <Tab.Container activeKey={page.activeTab} onSelect={(key) => page.setActiveTab(key as 'list' | 'module')}>
          <CardHeader className="border-light">
            <Nav variant="tabs" className="card-header-tabs">
              <Nav.Item>
                <Nav.Link eventKey="list">Danh sách</Nav.Link>
              </Nav.Item>
              <Nav.Item>
                <Nav.Link eventKey="module">Theo module</Nav.Link>
              </Nav.Item>
            </Nav>
          </CardHeader>
          <Tab.Content className="p-0">
            <Tab.Pane eventKey="list">
              <PermissionsListPanel
                searchPlaceholder="Tìm code, tên, module..."
                loadingLabel="Đang tải phân quyền..."
                emptyMessage="Chưa có quyền"
                list={page.list}
              />
            </Tab.Pane>
            <Tab.Pane eventKey="module" className="p-3">
              <PermissionsByModulePanel grouped={page.grouped} isLoading={page.moduleLoading} />
            </Tab.Pane>
          </Tab.Content>
        </Tab.Container>
      </Card>
    </EntityPageLayout>
  )
}

export default PermissionsPage
