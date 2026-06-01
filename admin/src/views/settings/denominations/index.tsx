import SettingsCrudTable from '@/views/settings/components/SettingsCrudTable'
import SettingsPageLayout from '@/views/settings/components/SettingsPageLayout'
import { buildDenominationColumns } from '@/views/settings/denominations/columns'
import { denominationsData, denominationsLabels } from '@/views/settings/denominations/data'
import { denominationFormConfig } from '@/views/settings/denominations/formConfig'

const DenominationsPage = () => {
  return (
    <SettingsPageLayout title="Mệnh giá" description="Dùng cho thẻ game và thẻ viễn thông (TelecomCards / GameCards).">
      <SettingsCrudTable
        initialData={denominationsData}
        buildColumns={buildDenominationColumns}
        formConfig={denominationFormConfig}
        labels={denominationsLabels}
      />
    </SettingsPageLayout>
  )
}

export default DenominationsPage
