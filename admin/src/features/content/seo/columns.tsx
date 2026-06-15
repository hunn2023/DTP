import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'
import { Button } from 'react-bootstrap'
import { TbEdit, TbTrash } from 'react-icons/tb'

import type { SeoMetadata } from '@/features/content/types'
import { createIdColumn, createSelectColumn } from '@/modules/crud/components/tableColumns'

export type SeoTableHandlers = {
  onEdit: (row: SeoMetadata) => void
  onDeleteRequest: (id: string) => void
}

const helper = createColumnHelper<SeoMetadata & { isActive: boolean }>()

export function buildSeoColumns(handlers: SeoTableHandlers) {
  return [
    createSelectColumn<SeoMetadata & { isActive: boolean }>(),
    createIdColumn<SeoMetadata & { isActive: boolean }>(),
    helper.accessor('metaTitle', {
      header: 'Meta title',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.metaTitle}</div>
          <div className="text-muted fs-xxs text-truncate" style={{ maxWidth: 280 }}>
            {row.original.routePath || row.original.entityType}
          </div>
        </div>
      ),
    }),
    helper.accessor('entityType', { header: 'Entity', cell: ({ getValue }) => <span className="badge badge-soft-info fs-xxs">{getValue()}</span> }),
    {
      id: 'actions',
      header: 'Thao tác',
      cell: ({ row }: { row: { original: SeoMetadata } }) => (
        <div className="d-flex gap-1" onClick={(e) => e.stopPropagation()}>
          <Button variant="light" size="sm" className="btn-icon rounded-circle" onClick={() => handlers.onEdit(row.original)}>
            <TbEdit className="fs-lg" />
          </Button>
          <Button variant="light" size="sm" className="btn-icon rounded-circle" onClick={() => handlers.onDeleteRequest(row.original.id)}>
            <TbTrash className="fs-lg" />
          </Button>
        </div>
      ),
    },
  ] as ColumnDef<SeoMetadata & { isActive: boolean }>[]
}

export function toSeoRow(item: SeoMetadata): SeoMetadata & { isActive: boolean } {
  return { ...item, isActive: true }
}
