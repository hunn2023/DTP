import { API_PATHS } from '@/shared/config/api'
import { normalizePaged, readString } from '@/shared/lib/dtoNormalize'
import { httpGet } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type AuditLogsQueryFilters = {
  keyword?: string
  module?: string
  action?: string
  actionType?: number
  status?: number
  userId?: string
  entityName?: string
  entityId?: string
  fromDate?: string
  toDate?: string
}

export type AuditLogRow = {
  id: string
  module: string
  action: string
  actionType: number
  status: number
  userId: string
  userName: string
  entityName: string
  entityId: string
  ipAddress: string
  requestPath: string
  requestMethod: string
  createdAt: string
}

export type AuditLogDetail = AuditLogRow & {
  description: string
  oldValues: string
  newValues: string
  userAgent: string
  correlationId: string
  errorMessage: string
}

function readEnum(raw: Raw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  if (typeof value === 'number') return value
  const parsed = Number(value)
  return Number.isNaN(parsed) ? 0 : parsed
}

function normalizeAuditLog(raw: Raw): AuditLogDetail {
  return {
    id: readString(raw, 'id', 'Id'),
    module: readString(raw, 'module', 'Module'),
    action: readString(raw, 'action', 'Action'),
    actionType: readEnum(raw, 'actionType', 'ActionType'),
    status: readEnum(raw, 'status', 'Status'),
    userId: readString(raw, 'userId', 'UserId'),
    userName: readString(raw, 'userName', 'UserName'),
    entityName: readString(raw, 'entityName', 'EntityName'),
    entityId: readString(raw, 'entityId', 'EntityId'),
    ipAddress: readString(raw, 'ipAddress', 'IpAddress'),
    requestPath: readString(raw, 'requestPath', 'RequestPath'),
    requestMethod: readString(raw, 'requestMethod', 'RequestMethod'),
    createdAt: readString(raw, 'createdAt', 'CreatedAt'),
    description: readString(raw, 'description', 'Description'),
    oldValues: readString(raw, 'oldValues', 'OldValues'),
    newValues: readString(raw, 'newValues', 'NewValues'),
    userAgent: readString(raw, 'userAgent', 'UserAgent'),
    correlationId: readString(raw, 'correlationId', 'CorrelationId'),
    errorMessage: readString(raw, 'errorMessage', 'ErrorMessage'),
  }
}

type AuditLogsPageResult = {
  items: AuditLogRow[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

const inflightAuditLogs = new Map<string, Promise<AuditLogsPageResult>>()

function buildAuditLogsRequestKey(
  pageIndex: number,
  pageSize: number,
  filters: AuditLogsQueryFilters,
): string {
  return JSON.stringify({ pageIndex, pageSize, filters })
}

export async function fetchAuditLogsPage(
  pageIndex: number,
  pageSize: number,
  filters: AuditLogsQueryFilters = {},
): Promise<AuditLogsPageResult> {
  const requestKey = buildAuditLogsRequestKey(pageIndex, pageSize, filters)
  const inflight = inflightAuditLogs.get(requestKey)
  if (inflight) return inflight

  const promise = httpGet<Raw>(API_PATHS.adminAuditLogs, {
    params: {
      pageIndex,
      pageSize,
      keyword: filters.keyword,
      module: filters.module,
      action: filters.action,
      actionType: filters.actionType,
      status: filters.status,
      userId: filters.userId,
      entityName: filters.entityName,
      entityId: filters.entityId,
      fromDate: filters.fromDate,
      toDate: filters.toDate,
    },
  })
    .then((raw) => normalizePaged(raw, normalizeAuditLog))
    .finally(() => {
      inflightAuditLogs.delete(requestKey)
    })

  inflightAuditLogs.set(requestKey, promise)
  return promise
}

export async function fetchAuditLogById(id: string): Promise<AuditLogDetail> {
  const raw = await httpGet<Raw>(`${API_PATHS.adminAuditLogs}/${id}`)
  return normalizeAuditLog(raw)
}
