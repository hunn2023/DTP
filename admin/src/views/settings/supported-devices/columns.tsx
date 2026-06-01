import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
} from '@/views/settings/components/settingsTableColumns'
import type { SettingsTableHandlers } from '@/views/settings/hooks/useSettingsCrudTable'
import type { SupportedDevice } from '@/views/settings/types'

const helper = createColumnHelper<SupportedDevice>()

export function buildSupportedDeviceColumns(
  handlers: SettingsTableHandlers<SupportedDevice>,
) {
  return [
    createSelectColumn<SupportedDevice>(),
    createIdColumn<SupportedDevice>(),
    helper.accessor('brandName', { header: 'Brand' }),
    helper.accessor('deviceName', { header: 'Device', cell: ({ getValue }) => <span className="fw-semibold">{getValue()}</span> }),
    helper.accessor('modelCode', { header: 'Model code', cell: ({ getValue }) => <code className="fs-xs">{getValue()}</code> }),
    helper.accessor('supportEsim', {
      header: 'eSIM',
      cell: ({ getValue }) => (
        <span className={`badge ${getValue() ? 'badge-soft-success' : 'badge-soft-danger'} fs-xxs`}>
          {getValue() ? 'Yes' : 'No'}
        </span>
      ),
    }),
    helper.accessor('note', {
      header: 'Note',
      cell: ({ getValue }) => <span className="text-muted fs-xs">{getValue()}</span>,
    }),
    createIsActiveColumn<SupportedDevice>(),
    createActionsColumn(handlers),
  ] as ColumnDef<SupportedDevice>[]
}
