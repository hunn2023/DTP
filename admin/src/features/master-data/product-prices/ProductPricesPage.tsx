import ProductPricesCrudTable from '@/features/master-data/product-prices/components/ProductPricesCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const ProductPricesPage = () => (
  <EntityPageLayout
    title="Giá sản phẩm (ProductPrices)"
    subtitle="Danh mục & dữ liệu"
    description="Bảng giá theo sản phẩm / biến thể — dùng cho eSIM, thẻ viễn thông và các loại sản phẩm khác.">
    <ProductPricesCrudTable />
  </EntityPageLayout>
)

export default ProductPricesPage
