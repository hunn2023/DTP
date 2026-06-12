import PhoneCardsCrudTable from '@/features/products/phone-cards/components/PhoneCardsCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const PhoneCardsPage = () => (
  <EntityPageLayout
    title="Thẻ viễn thông"
    subtitle="Sản phẩm bán"
    description="Quản lý thẻ viễn thông (PhoneCards). Liên kết Product Variant và Provider API.">
    <PhoneCardsCrudTable />
  </EntityPageLayout>
)

export default PhoneCardsPage
