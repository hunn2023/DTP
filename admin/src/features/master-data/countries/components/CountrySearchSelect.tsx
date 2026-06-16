import ApiSearchSelect from '@/components/form/ApiSearchSelect'
import {
  resolveCountrySelectOption,
  searchCountrySelectOptions,
} from '@/features/master-data/countries/countrySearchSelect'

type CountrySearchSelectProps = {
  value: string
  onChange: (value: string) => void
  placeholder?: string
  isDisabled?: boolean
}

const CountrySearchSelect = ({
  value,
  onChange,
  placeholder = 'Tìm quốc gia...',
  isDisabled = false,
}: CountrySearchSelectProps) => {
  return (
    <ApiSearchSelect
      value={value}
      onChange={onChange}
      placeholder={placeholder}
      isDisabled={isDisabled}
      loadOptions={searchCountrySelectOptions}
      resolveValue={resolveCountrySelectOption}
      noOptionsMessage="Không tìm thấy quốc gia"
    />
  )
}

export default CountrySearchSelect
