import { useCallback, useEffect, useState } from 'react'
import { Card, Container, Spinner, Tab } from 'react-bootstrap'
import { Link, useLocation, useNavigate, useParams, useSearchParams } from 'react-router'

import PageBreadcrumb from '@/components/PageBreadcrumb'
import { useNotificationContext } from '@/context/useNotificationContext'
import { fetchCategoryOptions } from '@/features/master-data/categories/categories.api'
import { fetchCountries } from '@/features/master-data/countries/countries.api'
import ProductAttributesTab from '@/features/master-data/products/detail/ProductAttributesTab'
import ProductImagesTab from '@/features/master-data/products/detail/ProductImagesTab'
import ProductFormFooter from '@/features/master-data/products/ProductFormFooter'
import ProductFormStepper from '@/features/master-data/products/ProductFormStepper'
import ProductInfoTab from '@/features/master-data/products/ProductInfoTab'
import { getStepByTab } from '@/features/master-data/products/productFormSteps'
import { fetchProductDetail } from '@/features/master-data/products/products.api'
import type { CatalogProduct, ProductFormTab } from '@/features/master-data/products/types'
import type { Country } from '@/features/master-data/types'
import type { FormFieldOption } from '@/modules/crud/form/types'

const TAB_KEYS: ProductFormTab[] = ['product', 'images', 'attributes']

function parseTab(value: string | null): ProductFormTab {
  if (value && TAB_KEYS.includes(value as ProductFormTab)) {
    return value as ProductFormTab
  }
  return 'product'
}

function isNewProductRoute(routeProductId: string | undefined, pathname: string): boolean {
  if (routeProductId === 'new') return true
  return pathname.replace(/\/$/, '').endsWith('/products/new')
}

const ProductFormPage = () => {
  const { productId: routeProductId } = useParams()
  const location = useLocation()
  const [searchParams, setSearchParams] = useSearchParams()
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()

  const isNew = isNewProductRoute(routeProductId, location.pathname)
  const productId = isNew ? null : (routeProductId ?? null)
  const activeTab = parseTab(searchParams.get('tab'))
  const currentStep = getStepByTab(activeTab)

  const [product, setProduct] = useState<CatalogProduct | null>(null)
  const [isLoading, setIsLoading] = useState(!isNew && Boolean(routeProductId))
  const [isSaving, setIsSaving] = useState(false)
  const [categoryOptions, setCategoryOptions] = useState<FormFieldOption[]>([])
  const [countries, setCountries] = useState<Country[]>([])

  const canAccessSubTabs = Boolean(productId)

  const loadProduct = useCallback(async (id: string) => {
    setIsLoading(true)
    try {
      const detail = await fetchProductDetail(id)
      setProduct(detail)
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    void Promise.all([fetchCategoryOptions(), fetchCountries()])
      .then(([categories, countryList]) => {
        setCategoryOptions(categories)
        setCountries(countryList)
      })
      .catch(() => {
        showNotification({
          title: 'Lỗi',
          message: 'Không tải được danh mục hoặc quốc gia',
          variant: 'danger',
          delay: 4000,
        })
      })
  }, [showNotification])

  useEffect(() => {
    if (isNew || !productId) return
    void loadProduct(productId)
  }, [isNew, productId, loadProduct])

  useEffect(() => {
    if (isNew && activeTab !== 'product') {
      setSearchParams({ tab: 'product' }, { replace: true })
    }
  }, [isNew, activeTab, setSearchParams])

  const setActiveTab = (tab: ProductFormTab) => {
    if (tab !== 'product' && !canAccessSubTabs) return
    setSearchParams({ tab })
  }

  const handleCreated = (id: string) => {
    showNotification({
      title: 'Thành công',
      message: 'Đã tạo sản phẩm. Tiếp tục thêm hình ảnh.',
      variant: 'success',
      delay: 2500,
    })
    navigate(`/settings/products/${id}?tab=images`, { replace: true })
  }

  const handleSaved = () => {
    showNotification({
      title: 'Thành công',
      message: 'Đã cập nhật sản phẩm',
      variant: 'success',
      delay: 2500,
    })
    if (productId) void loadProduct(productId)
    if (activeTab === 'product') {
      setSearchParams({ tab: 'images' })
    }
  }

  const handleContinue = () => {
    if (activeTab === 'images') {
      setSearchParams({ tab: 'attributes' })
      return
    }
    if (activeTab === 'attributes') {
      navigate('/settings/products')
    }
  }

  if (!isNew && isLoading) {
    return (
      <Container fluid className="text-center py-5">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải sản phẩm...
      </Container>
    )
  }

  if (!isNew && !isLoading && !product) {
    return (
      <Container fluid>
        <PageBreadcrumb title="Không tìm thấy" subtitle="Danh mục & dữ liệu" />
        <p className="text-muted">
          Sản phẩm không tồn tại. <Link to="/settings/products">Quay lại danh sách</Link>
        </p>
      </Container>
    )
  }

  const pageTitle = isNew ? 'Tạo Product' : `Chỉnh sửa: ${product?.name ?? 'Product'}`

  return (
    <Container fluid>
      <PageBreadcrumb title={pageTitle} subtitle="Danh mục & dữ liệu" />

      <Card>
        <Card.Body className="p-4">
          <div className="mb-2">
            <h4 className="mb-1 fw-semibold">
              {currentStep.step}. {isNew ? 'Tạo Product' : currentStep.title}
            </h4>
            {!isNew && product && (
              <p className="text-muted mb-0 fs-sm">
                <code>{product.slug}</code>
                {product.code ? ` · ${product.code}` : ''}
              </p>
            )}
          </div>

          <ProductFormStepper
            activeTab={activeTab}
            canAccessSubTabs={canAccessSubTabs}
            onStepClick={setActiveTab}
          />

          <Tab.Container activeKey={activeTab}>
            <Tab.Content>
              <Tab.Pane eventKey="product" mountOnEnter>
                <ProductInfoTab
                  productId={productId}
                  initialValues={product}
                  categoryOptions={categoryOptions}
                  countries={countries}
                  isNew={isNew}
                  onCreated={handleCreated}
                  onSaved={handleSaved}
                  onSavingChange={setIsSaving}
                />
              </Tab.Pane>
              <Tab.Pane eventKey="images" mountOnEnter>
                {productId && <ProductImagesTab productId={productId} />}
              </Tab.Pane>
              <Tab.Pane eventKey="attributes" mountOnEnter>
                {productId && <ProductAttributesTab productId={productId} />}
              </Tab.Pane>
            </Tab.Content>
          </Tab.Container>

          <ProductFormFooter activeTab={activeTab} isSaving={isSaving} onContinue={handleContinue} />
        </Card.Body>
      </Card>
    </Container>
  )
}

export default ProductFormPage
