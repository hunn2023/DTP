export const AUDIT_ACTION_TYPE_LABELS: Record<number, string> = {
  0: 'Không xác định',
  1: 'Tạo',
  2: 'Cập nhật',
  3: 'Xóa',
  4: 'Xem',
  5: 'Đăng nhập',
  6: 'Đăng xuất',
  10: 'Đổi trạng thái',
  11: 'Upload',
  12: 'Download',
  20: 'Thanh toán',
  21: 'Hoàn tiền',
  30: 'Provider request',
  31: 'Provider response',
  100: 'Hệ thống',
}

export const AUDIT_ACTION_TYPE_OPTIONS = Object.entries(AUDIT_ACTION_TYPE_LABELS).map(
  ([value, label]) => ({ value: Number(value), label }),
)

export const AUDIT_STATUS_OPTIONS = [
  { value: 1, label: 'Thành công' },
  { value: 2, label: 'Thất bại' },
  { value: 3, label: 'Cảnh báo' },
]

export const AUDIT_MODULE_OPTIONS = [
  'Auth',
  'Content',
  'Catalog',
  'Ordering',
  'Payment',
  'Delivery',
  'Provider',
  'Customer',
  'Audit',
  'System',
]

export const AUDIT_STATUS_LABELS: Record<number, string> = {
  1: 'Thành công',
  2: 'Thất bại',
  3: 'Cảnh báo',
}

export function enumLabel(
  value: number | string,
  labels: Record<number, string>,
): string {
  if (typeof value === 'number') return labels[value] ?? String(value)
  const parsed = Number(value)
  return Number.isNaN(parsed) ? value : labels[parsed] ?? value
}

export function formatDateTime(value: string): string {
  if (!value) return '—'
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? value : date.toLocaleString('vi-VN')
}
