import type { AuditLogsQueryFilters } from '@/apis/auditLogsApi'

export type AuditLogFilterForm = {
  keyword: string
  module: string
  action: string
  actionType: string
  status: string
  userId: string
  entityName: string
  entityId: string
  fromDate: string
  toDate: string
}

export const defaultAuditLogFilterForm: AuditLogFilterForm = {
  keyword: '',
  module: '',
  action: '',
  actionType: '',
  status: '',
  userId: '',
  entityName: '',
  entityId: '',
  fromDate: '',
  toDate: '',
}

function toIsoDateStart(date: string): string | undefined {
  if (!date) return undefined
  return `${date}T00:00:00`
}

function toIsoDateEnd(date: string): string | undefined {
  if (!date) return undefined
  return `${date}T23:59:59`
}

function parseOptionalInt(value: string): number | undefined {
  if (!value) return undefined
  const parsed = Number(value)
  return Number.isNaN(parsed) ? undefined : parsed
}

export function toAuditLogsQueryFilters(form: AuditLogFilterForm): AuditLogsQueryFilters {
  return {
    keyword: form.keyword.trim() || undefined,
    module: form.module.trim() || undefined,
    action: form.action.trim() || undefined,
    actionType: parseOptionalInt(form.actionType),
    status: parseOptionalInt(form.status),
    userId: form.userId.trim() || undefined,
    entityName: form.entityName.trim() || undefined,
    entityId: form.entityId.trim() || undefined,
    fromDate: toIsoDateStart(form.fromDate),
    toDate: toIsoDateEnd(form.toDate),
  }
}

export function filterFormKey(form: AuditLogFilterForm): string {
  return JSON.stringify(form)
}

export function hasAdvancedAuditFilters(form: AuditLogFilterForm): boolean {
  return [form.userId, form.entityName, form.entityId].some((value) => value.trim() !== '')
}
