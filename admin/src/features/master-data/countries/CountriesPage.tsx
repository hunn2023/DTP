import CountriesCrudTable from '@/features/master-data/countries/components/CountriesCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const CountriesPage = () => (
  <EntityPageLayout
    title="Quốc gia"
    subtitle="Cấu hình hệ thống"
    description="Danh mục quốc gia cho eSIM và nhà mạng. Liên kết với Carriers và gói eSIM.">
    <CountriesCrudTable />
  </EntityPageLayout>
)

export default CountriesPage
