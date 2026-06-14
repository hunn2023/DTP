import FaqsCrudTable from '@/features/content/faqs/components/FaqsCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const FaqsPage = () => (
  <EntityPageLayout
    title="FAQ"
    subtitle="Website & SEO"
    description="FAQ chung website — đồng bộ API admin/content/faqs.">
    <FaqsCrudTable />
  </EntityPageLayout>
)

export default FaqsPage
