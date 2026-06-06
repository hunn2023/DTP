type ActiveFilterValue = 'all' | 'true' | 'false'

type ActiveFilterSelectProps = {
  value: ActiveFilterValue
  onChange: (value: ActiveFilterValue) => void
}

const ActiveFilterSelect = ({ value, onChange }: ActiveFilterSelectProps) => (
  <select
    className="form-select form-select-sm"
    style={{ minWidth: '9.75rem', width: 'auto' }}
    aria-label="Lọc theo trạng thái"
    value={value}
    onChange={(e) => onChange(e.target.value as ActiveFilterValue)}>
    <option value="all">Tất cả</option>
    <option value="true">Đang hiển thị</option>
    <option value="false">Đang ẩn</option>
  </select>
)

export type { ActiveFilterValue }
export default ActiveFilterSelect

export function activeFilterToBool(value: ActiveFilterValue): boolean | undefined {
  if (value === 'true') return true
  if (value === 'false') return false
  return undefined
}
