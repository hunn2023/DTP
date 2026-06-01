import SettingsCrudTable from '@/views/settings/components/SettingsCrudTable'
import SettingsPageLayout from '@/views/settings/components/SettingsPageLayout'
import { buildCarrierColumns } from '@/views/settings/carriers/columns'
import { carriersData, carriersLabels } from '@/views/settings/carriers/data'
import { carrierFormConfig } from '@/views/settings/carriers/formConfig'

const CarriersPage = () => {
  return (
    <SettingsPageLayout
      title="Nhà mạng"
      description="Hiển thị, lọc và SEO — không phải provider API. Liên kết CountryId.">
      <SettingsCrudTable
        initialData={carriersData}
        buildColumns={buildCarrierColumns}
        formConfig={carrierFormConfig}
        labels={carriersLabels}
      />
    </SettingsPageLayout>
  )
}

export default CarriersPage
