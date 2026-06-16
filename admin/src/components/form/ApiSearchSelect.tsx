import { useCallback, useEffect, useRef, useState } from 'react'
import AsyncSelect from 'react-select/async'
import type { GroupBase, OptionsOrGroups } from 'react-select'

export type ApiSearchSelectOption = {
  value: string
  label: string
}

type ApiSearchSelectProps = {
  value: string
  onChange: (value: string, option?: ApiSearchSelectOption | null) => void
  placeholder?: string
  isDisabled?: boolean
  isClearable?: boolean
  menuMaxHeight?: number
  debounceMs?: number
  loadOptions: (keyword: string) => Promise<ApiSearchSelectOption[]>
  resolveValue?: (value: string) => Promise<ApiSearchSelectOption | null>
  noOptionsMessage?: string
}

const DEFAULT_MENU_HEIGHT = 240
const DEFAULT_DEBOUNCE_MS = 300
const MENU_PORTAL_Z_INDEX = 1060

function getMenuPortalTarget(): HTMLElement | null {
  if (typeof document === 'undefined') return null
  return document.body
}

function useDebouncedLoader(
  loader: (keyword: string) => Promise<ApiSearchSelectOption[]>,
  debounceMs: number,
) {
  const loaderRef = useRef(loader)
  loaderRef.current = loader

  const timerRef = useRef<ReturnType<typeof setTimeout> | undefined>(undefined)
  const requestIdRef = useRef(0)

  return useCallback(
    (keyword: string) =>
      new Promise<ApiSearchSelectOption[]>((resolve) => {
        clearTimeout(timerRef.current)
        const requestId = ++requestIdRef.current

        timerRef.current = setTimeout(() => {
          void loaderRef.current(keyword).then((options) => {
            if (requestId === requestIdRef.current) resolve(options)
          })
        }, debounceMs)
      }),
    [debounceMs],
  )
}

const ApiSearchSelect = ({
  value,
  onChange,
  placeholder = 'Tìm kiếm...',
  isDisabled = false,
  isClearable = true,
  menuMaxHeight = DEFAULT_MENU_HEIGHT,
  debounceMs = DEFAULT_DEBOUNCE_MS,
  loadOptions,
  resolveValue,
  noOptionsMessage = 'Không có kết quả',
}: ApiSearchSelectProps) => {
  const [selected, setSelected] = useState<ApiSearchSelectOption | null>(null)
  const debouncedLoad = useDebouncedLoader(loadOptions, debounceMs)

  useEffect(() => {
    if (!value) {
      setSelected(null)
      return
    }
    if (selected?.value === value) return
    if (!resolveValue) return

    let active = true
    void resolveValue(value).then((option) => {
      if (active && option) setSelected(option)
    })
    return () => {
      active = false
    }
  }, [value, resolveValue, selected?.value])

  const handleLoadOptions = useCallback(
  async (inputValue: string): Promise<OptionsOrGroups<ApiSearchSelectOption, GroupBase<ApiSearchSelectOption>>> => {
      return debouncedLoad(inputValue)
    },
    [debouncedLoad],
  )

  return (
    <AsyncSelect<ApiSearchSelectOption>
      className="react-select"
      classNamePrefix="react-select"
      cacheOptions
      defaultOptions
      value={selected}
      loadOptions={handleLoadOptions}
      onChange={(option) => {
        const next = option as ApiSearchSelectOption | null
        setSelected(next)
        onChange(next?.value ?? '', next)
      }}
      placeholder={placeholder}
      isDisabled={isDisabled}
      isClearable={isClearable}
      maxMenuHeight={menuMaxHeight}
      menuPortalTarget={getMenuPortalTarget()}
      menuPosition="fixed"
      menuShouldScrollIntoView={false}
      styles={{
        menuPortal: (base) => ({ ...base, zIndex: MENU_PORTAL_Z_INDEX }),
      }}
      noOptionsMessage={() => noOptionsMessage}
      loadingMessage={() => 'Đang tải...'}
    />
  )
}

export default ApiSearchSelect
