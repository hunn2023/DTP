import type { ContentPublishStatus } from '@/features/content/types'

export const CONTENT_STATUS_OPTIONS = [
  { value: '0', label: 'Nháp' },
  { value: '1', label: 'Đã đăng' },
  { value: '2', label: 'Ẩn' },
] as const

const statusLabelMap: Record<ContentPublishStatus, string> = {
  0: 'Nháp',
  1: 'Đã đăng',
  2: 'Ẩn',
}

const statusBadgeMap: Record<ContentPublishStatus, string> = {
  0: 'badge-soft-secondary',
  1: 'badge-soft-success',
  2: 'badge-soft-warning',
}

export function getContentStatusLabel(status: ContentPublishStatus): string {
  return statusLabelMap[status] ?? String(status)
}

export function getContentStatusBadgeClass(status: ContentPublishStatus): string {
  return statusBadgeMap[status] ?? 'badge-soft-secondary'
}
