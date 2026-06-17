import ApiSearchSelect from '@/components/form/ApiSearchSelect'
import type { ApiSearchSelectOption } from '@/components/form/ApiSearchSelect'

type ApiFilterSearchSelectProps = {
  label: string
  value: string
  onChange: (value: string, option?: ApiSearchSelectOption | null) => void
  allLabel: string
  loadOptions: (keyword: string) => Promise<ApiSearchSelectOption[]>
  resolveValue?: (value: string) => Promise<ApiSearchSelectOption | null>
  noOptionsMessage?: string
  isDisabled?: boolean
  className?: string
}

const ApiFilterSearchSelect = ({
  label,
  value,
  onChange,
  allLabel,
  loadOptions,
  resolveValue,
  noOptionsMessage,
  isDisabled = false,
  className = '',
}: ApiFilterSearchSelectProps) => (
  <div className={`d-flex flex-column ${className}`}>
    <label className="form-label mb-1 small text-muted">{label}</label>
    <ApiSearchSelect
      value={value}
      onChange={(next, option) => onChange(next, option)}
      placeholder={allLabel}
      loadOptions={loadOptions}
      resolveValue={resolveValue}
      noOptionsMessage={noOptionsMessage}
      isDisabled={isDisabled}
    />
  </div>
)

export default ApiFilterSearchSelect
