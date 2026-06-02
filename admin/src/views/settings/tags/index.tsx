import SettingsCrudTable from '@/views/settings/components/SettingsCrudTable'
import SettingsPageLayout from '@/views/settings/components/SettingsPageLayout'
import { buildTagColumns } from '@/views/settings/tags/columns'
import { tagsData, tagsLabels } from '@/views/settings/tags/data'
import { tagFormConfig } from '@/views/settings/tags/formConfig'

const TagsPage = () => {
  return (
    <SettingsPageLayout
      title="Tag"
      description="Nhãn gắn sản phẩm qua bảng ProductTags. Có thể hardcode ở MVP nếu chưa cần CRUD đầy đủ.">
      <SettingsCrudTable initialData={tagsData} buildColumns={buildTagColumns} formConfig={tagFormConfig} labels={tagsLabels} />
    </SettingsPageLayout>
  )
}

export default TagsPage
