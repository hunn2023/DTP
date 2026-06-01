import SettingsCrudTable from '@/views/settings/components/SettingsCrudTable'
import SettingsPageLayout from '@/views/settings/components/SettingsPageLayout'
import { buildSupportedDeviceColumns } from '@/views/settings/supported-devices/columns'
import { supportedDevicesData, supportedDevicesLabels } from '@/views/settings/supported-devices/data'
import { supportedDeviceFormConfig } from '@/views/settings/supported-devices/formConfig'

const SupportedDevicesPage = () => {
  return (
    <SettingsPageLayout
      title="Thiết bị hỗ trợ"
      description="Danh sách thiết bị hỗ trợ eSIM. Phase 2: Import Excel. MVP có thể dùng trang hướng dẫn tĩnh.">
      <SettingsCrudTable
        initialData={supportedDevicesData}
        buildColumns={buildSupportedDeviceColumns}
        formConfig={supportedDeviceFormConfig}
        labels={supportedDevicesLabels}
      />
    </SettingsPageLayout>
  )
}

export default SupportedDevicesPage
