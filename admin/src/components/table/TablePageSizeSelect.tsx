export type TablePageSizeSelectProps = {
  pageSize: number
  pageSizeOptions: readonly number[]
  onPageSizeChange: (size: number) => void
}

const TablePageSizeSelect = ({
  pageSize,
  pageSizeOptions,
  onPageSizeChange,
}: TablePageSizeSelectProps) => (
  <select
    className="form-select form-select-sm w-auto"
    aria-label="Số dòng mỗi trang"
    value={pageSize}
    onChange={(e) => onPageSizeChange(Number(e.target.value))}>
    {pageSizeOptions.map((size) => (
      <option key={size} value={size}>
        {size}
      </option>
    ))}
  </select>
)

export default TablePageSizeSelect
