import SettingsCrudTable from '@/views/settings/components/SettingsCrudTable'
import SettingsPageLayout from '@/views/settings/components/SettingsPageLayout'
import { buildBrandColumns } from '@/views/settings/brands/columns'
import { brandsData, brandsLabels } from '@/views/settings/brands/data'
import { brandFormConfig } from '@/views/settings/brands/formConfig'

const BrandsPage = () => {
  return (
    <SettingsPageLayout
      title="Brand / Nhà phát hành"
      description="Nhà phát hành, nhà cung cấp thương hiệu. Products và Carriers có thể tham chiếu BrandId.">
      <SettingsCrudTable
        initialData={brandsData}
        buildColumns={buildBrandColumns}
        formConfig={brandFormConfig}
        labels={brandsLabels}
      />
    </SettingsPageLayout>
  )
}

export default BrandsPage
