import { flexRender, type Table as TableType } from '@tanstack/react-table'
import clsx from 'clsx'
import { Table } from 'react-bootstrap'
import { TbArrowDown, TbArrowUp } from 'react-icons/tb'

import { isHiddenTableColumn } from '@/modules/crud/components/tableColumns'

type DataTableProps<TData> = {
  /**
   * The table instance from useReactTable
   */
  table: TableType<TData>
  /**
   * Optional class name for the table container
   */
  className?: string
  /**
   * Optional message to display when no data is available
   * @default 'Nothing found.'
   */
  emptyMessage?: React.ReactNode

  /**
   * Optional boolean to display headers
   * @default true
   */
  showHeaders?: boolean

  /** Click row to open detail (e.g. view modal). Ignored for checkbox/action clicks. */
  onRowClick?: (row: TData) => void
}

const DataTable = <TData,>({
  table,
  className = '',
  emptyMessage = 'Nothing found.',
  showHeaders = true,
  onRowClick,
}: DataTableProps<TData>) => {
  const headerGroup = table.getHeaderGroups()[0]
  const visibleHeaders =
    headerGroup?.headers.filter((header) => !isHiddenTableColumn(header.column.columnDef.meta)) ?? []
  const visibleColumnCount = visibleHeaders.length || table.getAllColumns().length

  return (
    <div className={clsx('table-responsive', className)}>
      <Table responsive hover className="table table-custom table-centered table-select w-100 mb-0">
        {showHeaders && (
          <thead className="bg-light align-middle bg-opacity-25 thead-sm">
            {table.getHeaderGroups().map((headerGroup) => (
              <tr key={headerGroup.id} className="text-uppercase fs-xxs">
                {headerGroup.headers
                  .filter((header) => !isHiddenTableColumn(header.column.columnDef.meta))
                  .map((header) => (
                  <th
                    key={header.id}
                    onClick={header.column.getToggleSortingHandler()}
                    style={{
                      cursor: header.column.getCanSort() ? 'pointer' : 'default',
                      userSelect: 'none',
                    }}>
                    <div className="d-flex align-items-center">
                      {flexRender(header.column.columnDef.header, header.getContext())}
                      {header.column.getCanSort() &&
                        ({
                          asc: <TbArrowUp className="ms-1" />,
                          desc: <TbArrowDown className="ms-1" />,
                        }[header.column.getIsSorted() as string] ??
                          null)}
                    </div>
                  </th>
                ))}
              </tr>
            ))}
          </thead>
        )}
        <tbody>
          {table.getRowModel().rows?.length ? (
            table.getRowModel().rows.map((row) => (
              <tr
                key={row.id}
                className={onRowClick ? 'cursor-pointer' : undefined}
                onClick={() => onRowClick?.(row.original)}>
                {row
                  .getVisibleCells()
                  .filter((cell) => !isHiddenTableColumn(cell.column.columnDef.meta))
                  .map((cell) => (
                    <td key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</td>
                  ))}
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan={visibleColumnCount} className="text-center py-3 text-muted">
                {emptyMessage}
              </td>
            </tr>
          )}
        </tbody>
      </Table>
    </div>
  )
}

export default DataTable
