import { Container, Spinner } from 'react-bootstrap'
import { Link, useNavigate } from 'react-router'

import PageBreadcrumb from '@/components/PageBreadcrumb'
import { useNotificationContext } from '@/context/useNotificationContext'
import ProductAttributesTab from '@/features/master-data/products/components/detail/ProductAttributesTab'
import ProductImagesTab from '@/features/master-data/products/components/detail/ProductImagesTab'
import ProductFormFooter from '@/features/master-data/products/components/ProductFormFooter'
import ProductWizard from '@/features/master-data/products/components/ProductWizard'
import ProductInfoTab from '@/features/master-data/products/components/ProductInfoTab'
import { useProductFormPage } from '@/features/master-data/products/useProductFormPage'

const ProductFormPage = () => {
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()
  const form = useProductFormPage()

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

    if (form.productId) void form.reloadProduct(form.productId)

    if (form.activeTab === 'product') {
      form.setSearchParams({ tab: 'images' })
    }
  }

  const handleContinue = () => {
    if (form.activeTab === 'images') {
      form.setSearchParams({ tab: 'attributes' })
      return
    }

    if (form.activeTab === 'attributes') {
      navigate('/settings/products')
    }
  }

  if (!form.isNew && form.isLoading) {
    return (
      <Container fluid className="text-center py-5">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải sản phẩm...
      </Container>
    )
  }

  if (!form.isNew && !form.isLoading && !form.product) {
    return (
      <Container fluid>
        <PageBreadcrumb title="Không tìm thấy" subtitle="Danh mục & dữ liệu" />
        <p className="text-muted">
          Sản phẩm không tồn tại. <Link to="/settings/products">Quay lại danh sách</Link>
        </p>
      </Container>
    )
  }

  return (
    <Container fluid>
      <PageBreadcrumb title={form.pageTitle} subtitle="Danh mục & dữ liệu" />

      <ProductWizard
        title={form.pageTitle}
        activeTab={form.activeTab}
        canAccessSubTabs={form.canAccessSubTabs}
        onStepChange={form.setActiveTab}
        productStep={
          <>
            <ProductInfoTab
              productId={form.productId}
              initialValues={form.product}
              categoryOptions={form.categoryOptions}
              countries={form.countries}
              isNew={form.isNew}
              onCreated={handleCreated}
              onSaved={handleSaved}
              onSavingChange={form.setIsSaving}
            />

            <div className="border-top mt-4 pt-3">
              <ProductFormFooter
                activeTab={form.activeTab}
                isSaving={form.isSaving}
                onContinue={handleContinue}
              />
            </div>
          </>
        }
        imagesStep={
          <>
            {form.productId && <ProductImagesTab productId={form.productId} />}

            <div className="border-top mt-4 pt-3">
              <ProductFormFooter
                activeTab={form.activeTab}
                isSaving={form.isSaving}
                onContinue={handleContinue}
              />
            </div>
          </>
        }
        attributesStep={
          <>
            {form.productId && <ProductAttributesTab productId={form.productId} />}

            <div className="border-top mt-4 pt-3">
              <ProductFormFooter
                activeTab={form.activeTab}
                isSaving={form.isSaving}
                onContinue={handleContinue}
              />
            </div>
          </>
        }
      />
    </Container>
  )
}

export default ProductFormPage
