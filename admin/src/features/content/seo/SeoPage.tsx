import SeoCrudTable from '@/features/content/seo/components/SeoCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const SeoPage = () => (
  <EntityPageLayout title="Cấu hình SEO" subtitle="Website & SEO" description="Quản lý meta SEO — API admin/content/seo.">
    <SeoCrudTable />
  </EntityPageLayout>
)

export default SeoPage
