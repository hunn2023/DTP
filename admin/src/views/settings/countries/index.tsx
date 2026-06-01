import SettingsCrudTable from '@/views/settings/components/SettingsCrudTable'
import SettingsPageLayout from '@/views/settings/components/SettingsPageLayout'
import { buildCountryColumns } from '@/views/settings/countries/columns'
import { countriesData, countriesLabels } from '@/views/settings/countries/data'
import { countryFormConfig } from '@/views/settings/countries/formConfig'

const CountriesPage = () => {
  return (
    <SettingsPageLayout
      title="Quốc gia"
      description="Danh mục quốc gia cho eSIM và nhà mạng. RegionCode dùng enum (ASIA, EU, ...) — không cần bảng Regions riêng ở MVP.">
      <SettingsCrudTable
        initialData={countriesData}
        buildColumns={buildCountryColumns}
        formConfig={countryFormConfig}
        labels={countriesLabels}
      />
    </SettingsPageLayout>
  )
}

export default CountriesPage
