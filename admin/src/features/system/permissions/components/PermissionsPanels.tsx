import { Card, Spinner } from 'react-bootstrap'

import type { Table } from '@tanstack/react-table'

import type { PermissionRow } from '@/apis/permissionsApi'
import SystemListTable from '@/features/system/shared/SystemListTable'

type PermissionsListPanelProps = {
  searchPlaceholder: string
  loadingLabel: string
  emptyMessage: string
  list: {
    table: Table<PermissionRow>
    globalFilter: string
    setGlobalFilter: (value: string) => void
    paginationInfo: { start: number; end: number; total: number }
    pageCount: number
    pageSize: number
    setPageSize: (size: number) => void
    isLoading: boolean
  }
}

const PermissionsListPanel = ({
  searchPlaceholder,
  loadingLabel,
  emptyMessage,
  list,
}: PermissionsListPanelProps) => (
  <SystemListTable
    searchPlaceholder={searchPlaceholder}
    loadingLabel={loadingLabel}
    emptyMessage={emptyMessage}
    {...list}
  />
)

type PermissionsByModulePanelProps = {
  grouped: Record<string, PermissionRow[]>
  isLoading: boolean
}

const PermissionsByModulePanel = ({ grouped, isLoading }: PermissionsByModulePanelProps) => {
  if (isLoading) {
    return (
      <Card>
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải phân quyền theo module...
        </div>
      </Card>
    )
  }

  const modules = Object.entries(grouped)
  if (modules.length === 0) {
    return (
      <Card>
        <div className="text-center py-5 text-muted">Chưa có quyền theo module</div>
      </Card>
    )
  }

  return (
    <div className="d-flex flex-column gap-3">
      {modules.map(([module, items]) => (
        <Card key={module}>
          <Card.Header className="border-light fw-semibold">{module}</Card.Header>
          <Card.Body className="p-0">
            <div className="table-responsive">
              <table className="table table-sm table-hover mb-0">
                <thead>
                  <tr>
                    <th>Mã</th>
                    <th>Tên</th>
                    <th>Mô tả</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((item) => (
                    <tr key={item.id}>
                      <td>
                        <code>{item.code}</code>
                      </td>
                      <td>{item.name}</td>
                      <td className="text-muted">{item.description || '—'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </Card.Body>
        </Card>
      ))}
    </div>
  )
}

export { PermissionsByModulePanel, PermissionsListPanel }
