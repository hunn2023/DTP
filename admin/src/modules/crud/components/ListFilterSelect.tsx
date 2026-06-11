import type { FormFieldOption } from '@/modules/crud/form/types'

type ListFilterSelectProps = {
  label: string
  value: string
  onChange: (value: string) => void
  options: FormFieldOption[]
  allLabel?: string
  className?: string
  minWidth?: string
  onFocus?: () => void
}

const ListFilterSelect = ({
  label,
  value,
  onChange,
  options,
  allLabel = 'Tất cả',
  className = '',
  minWidth = '9rem',
  onFocus,
}: ListFilterSelectProps) => (
  <div className="d-flex flex-column">
    <label className="form-label mb-1 small text-muted">{label}</label>
    <select
      className={`form-select form-select-sm ${className}`.trim()}
      style={{ minWidth, width: 'auto' }}
      aria-label={label}
      value={value}
      onFocus={onFocus}
      onChange={(e) => onChange(e.target.value)}>
      <option value="">{allLabel}</option>
      {options.map((opt) => (
        <option key={opt.value} value={opt.value}>
          {opt.label}
        </option>
      ))}
    </select>
  </div>
)

export default ListFilterSelect
