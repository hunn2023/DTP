import { useEffect, useState } from 'react'
import { Card, Container, Nav, Spinner, Tab } from 'react-bootstrap'
import { Link, useParams } from 'react-router'

import PageBreadcrumb from '@/components/PageBreadcrumb'
import ProductAttributesTab from '@/features/master-data/products/detail/ProductAttributesTab'
import ProductImagesTab from '@/features/master-data/products/detail/ProductImagesTab'
import ProductVariantsTab from '@/features/master-data/products/detail/ProductVariantsTab'
import { fetchProductDetail } from '@/features/master-data/products/products.api'
import type { CatalogProduct } from '@/features/master-data/products/types'

const ProductDetailPage = () => {
  const { productId = '' } = useParams()
  const [product, setProduct] = useState<CatalogProduct | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [activeTab, setActiveTab] = useState('variants')

  useEffect(() => {
    if (!productId) return
    setIsLoading(true)
    void fetchProductDetail(productId)
      .then(setProduct)
      .finally(() => setIsLoading(false))
  }, [productId])

  if (isLoading) {
    return (
      <Container fluid className="text-center py-5">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải sản phẩm...
      </Container>
    )
  }

  if (!product) {
    return (
      <Container fluid>
        <PageBreadcrumb title="Không tìm thấy" subtitle="Danh mục & dữ liệu" />
        <p className="text-muted">
          Sản phẩm không tồn tại.{' '}
          <Link to="/settings/products">Quay lại danh sách</Link>
        </p>
      </Container>
    )
  }

  return (
    <Container fluid>
      <PageBreadcrumb title={product.name} subtitle="Danh mục & dữ liệu" />
      <div className="d-flex align-items-center justify-content-between mb-3 flex-wrap gap-2">
        <div>
          <p className="text-muted mb-1">
            <code>{product.slug}</code>
            {product.code ? ` · ${product.code}` : ''}
          </p>
          <p className="text-muted mb-0 fs-sm">
            Danh mục: {product.categoryName || product.categoryId || '—'}
          </p>
        </div>
        <Link to="/settings/products" className="btn btn-sm btn-light">
          ← Danh sách sản phẩm
        </Link>
      </div>

      <Card>
        <Card.Body>
          <Tab.Container activeKey={activeTab} onSelect={(key) => key && setActiveTab(key)}>
            <Nav variant="tabs" className="mb-3">
              <Nav.Item>
                <Nav.Link eventKey="variants">Biến thể (ProductVariants)</Nav.Link>
              </Nav.Item>
              <Nav.Item>
                <Nav.Link eventKey="images">Hình ảnh (ProductImages)</Nav.Link>
              </Nav.Item>
              <Nav.Item>
                <Nav.Link eventKey="attributes">Thuộc tính (ProductAttributes)</Nav.Link>
              </Nav.Item>
            </Nav>
            <Tab.Content>
              <Tab.Pane eventKey="variants">
                <ProductVariantsTab productId={productId} />
              </Tab.Pane>
              <Tab.Pane eventKey="images">
                <ProductImagesTab productId={productId} />
              </Tab.Pane>
              <Tab.Pane eventKey="attributes">
                <ProductAttributesTab productId={productId} />
              </Tab.Pane>
            </Tab.Content>
          </Tab.Container>
        </Card.Body>
      </Card>
    </Container>
  )
}

export default ProductDetailPage
