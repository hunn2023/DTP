import CategoriesCrudTable from '@/features/master-data/categories/components/CategoriesCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const CategoriesPage = () => (
  <EntityPageLayout
    title="Danh mục"
    subtitle="Cấu hình hệ thống"
    description="Phân loại sản phẩm chính: eSIM, thẻ game, thẻ viễn thông, data. Dùng cho Products / EsimPackages / GameCards.">
    <CategoriesCrudTable />
  </EntityPageLayout>
)

export default CategoriesPage
