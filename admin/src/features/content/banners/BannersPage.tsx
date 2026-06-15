import BannersCrudTable from '@/features/content/banners/components/BannersCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const BannersPage = () => (
  <EntityPageLayout
    title="Banner"
    subtitle="Website & SEO"
    description="Quản lý banner homepage / landing — đồng bộ API admin/content/banners.">
    <BannersCrudTable />
  </EntityPageLayout>
)

export default BannersPage
