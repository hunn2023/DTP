import PagesCrudTable from '@/features/content/pages/components/PagesCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const PagesPage = () => (
  <EntityPageLayout title="Trang tĩnh" subtitle="Website & SEO" description="Chính sách, điều khoản — API admin/content/pages.">
    <PagesCrudTable />
  </EntityPageLayout>
)

export default PagesPage
