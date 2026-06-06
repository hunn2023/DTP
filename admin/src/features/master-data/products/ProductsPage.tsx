import ProductsCrudTable from '@/features/master-data/products/ProductsCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const ProductsPage = () => (
  <EntityPageLayout
    title="Sản phẩm (master catalog)"
    subtitle="Danh mục & dữ liệu"
    description="Catalog master: biến thể, hình ảnh và thuộc tính quản lý trong trang chi tiết từng sản phẩm.">
    <ProductsCrudTable />
  </EntityPageLayout>
)

export default ProductsPage
