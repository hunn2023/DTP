import SettingsCrudTable from '@/views/settings/components/SettingsCrudTable'
import SettingsPageLayout from '@/views/settings/components/SettingsPageLayout'
import { buildCategoryColumns } from '@/views/settings/categories/columns'
import { categoriesData, categoriesLabels } from '@/views/settings/categories/data'
import { categoryFormConfig } from '@/views/settings/categories/formConfig'

const CategoriesPage = () => {
  return (
    <SettingsPageLayout
      title="Danh mục"
      description="Phân loại sản phẩm chính: eSIM, thẻ game, thẻ viễn thông, data. Dùng cho Products / EsimPackages / GameCards.">
      <SettingsCrudTable
        initialData={categoriesData}
        buildColumns={buildCategoryColumns}
        formConfig={categoryFormConfig}
        labels={categoriesLabels}
      />
    </SettingsPageLayout>
  )
}

export default CategoriesPage
