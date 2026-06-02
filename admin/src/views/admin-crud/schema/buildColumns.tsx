import { type ReactNode } from 'react'
import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/views/settings/components/settingsTableColumns'
import type { SettingsTableHandlers } from '@/views/settings/hooks/useSettingsCrudTable'
import type { SettingsEntityBase } from '@/views/settings/types'
import { getFieldLabel } from '@/views/admin-crud/entities/fieldLabels'
import type { CrudCapabilities, EntityFieldDef } from '@/views/admin-crud/types'
import { defaultCrudCapabilities } from '@/views/admin-crud/types'
import { getTagsByIds } from '@/views/settings/tags/tagOptions'

function renderTagIdsCell(tagIds: unknown): ReactNode {
  const ids = Array.isArray(tagIds) ? (tagIds as number[]) : []
  if (ids.length === 0) return <span className="text-muted">—</span>
  return (
    <div className="d-flex flex-wrap gap-1">
      {getTagsByIds(ids).map((tag) => (
        <span
          key={tag.id}
          className="badge fs-xxs"
          style={{ backgroundColor: tag.color, color: '#fff' }}>
          {tag.icon} {tag.name}
        </span>
      ))}
    </div>
  )
}

type FieldCellMeta = {
  name: string
  type: EntityFieldDef<SettingsEntityBase>['type']
  table?: EntityFieldDef<SettingsEntityBase>['table']
}

function renderCellValue(value: unknown, field: FieldCellMeta): ReactNode {
  if (field.type === 'multiselect' || field.name === 'tagIds') {
    return renderTagIdsCell(value)
  }
  if (value === null || value === undefined) return '—'
  if (field.type === 'badge' || (typeof field.table === 'object' && field.table.variant === 'badge')) {
    const map = typeof field.table === 'object' ? field.table.badgeMap : undefined
    const key = String(value)
    const label = map?.[key] ?? key
    return <span className="badge badge-soft-info fs-xxs">{label}</span>
  }
  if (typeof value === 'boolean') {
    return (
      <span className={`badge ${value ? 'badge-soft-success' : 'badge-soft-secondary'} fs-xxs`}>
        {value ? 'Có' : 'Không'}
      </span>
    )
  }
  return String(value)
}

export function buildColumnsFromFields<T extends SettingsEntityBase>(
  fields: EntityFieldDef<T>[],
  handlers: SettingsTableHandlers<T>,
  capabilities: CrudCapabilities = defaultCrudCapabilities,
): ColumnDef<T>[] {
  const helper = createColumnHelper<T>()
  const caps = { ...defaultCrudCapabilities, ...capabilities }
  const cols: ColumnDef<T>[] = []

  if (caps.delete) {
    cols.push(createSelectColumn<T>() as ColumnDef<T>)
  }

  if (fields.some((f) => f.name === 'id' && f.table)) {
    cols.push(createIdColumn<T>() as ColumnDef<T>)
  }

  fields.forEach((field) => {
    if (!field.table) return
    if (field.name === 'id' || field.name === 'sortOrder' || field.name === 'isActive') return
    const variant = typeof field.table === 'object' ? field.table.variant : undefined

    cols.push(
      helper.accessor((row) => row[field.name as keyof T], {
        id: field.name,
        header: field.label ?? getFieldLabel(field.name),
        cell: ({ getValue, row }) => {
          const value = getValue()
          if (variant === 'primary' && field.name === 'name') {
            const slug = 'slug' in row.original ? String((row.original as T & { slug: string }).slug) : ''
            return (
              <div>
                <div className="fw-semibold">{String(value)}</div>
                {slug && <div className="text-muted fs-xxs">/{slug}</div>}
              </div>
            )
          }
          if (variant === 'code') {
            return <code className="fs-xs">{String(value)}</code>
          }
          return renderCellValue(value, {
            name: field.name,
            type: field.type,
            table: field.table,
          })
        },
      }) as ColumnDef<T>,
    )
  })

  if (fields.some((f) => f.name === 'sortOrder')) {
    cols.push(createSortOrderColumn<T & { sortOrder: number }>() as ColumnDef<T>)
  }
  if (fields.some((f) => f.name === 'isActive')) {
    cols.push(createIsActiveColumn<T>() as ColumnDef<T>)
  }

  const hasRowActions = caps.edit || caps.delete || caps.toggleActive !== false
  if (hasRowActions) {
    cols.push(createActionsColumn(handlers, caps) as ColumnDef<T>)
  }

  return cols as ColumnDef<T>[]
}
