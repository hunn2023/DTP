import ApiSearchSelect from '@/components/form/ApiSearchSelect'
import {
  resolveCategorySelectOption,
  searchCategorySelectOptions,
} from '@/features/master-data/categories/categorySearchSelect'

type CategorySearchSelectProps = {
  value: string
  onChange: (value: string) => void
  placeholder?: string
  isDisabled?: boolean
  isClearable?: boolean
}

const CategorySearchSelect = ({
  value,
  onChange,
  placeholder = 'Tìm danh mục...',
  isDisabled = false,
  isClearable = false,
}: CategorySearchSelectProps) => {
  return (
    <ApiSearchSelect
      value={value}
      onChange={onChange}
      placeholder={placeholder}
      isDisabled={isDisabled}
      isClearable={isClearable}
      loadOptions={searchCategorySelectOptions}
      resolveValue={resolveCategorySelectOption}
      noOptionsMessage="Không tìm thấy danh mục"
    />
  )
}

export default CategorySearchSelect
