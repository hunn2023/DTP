import { useCallback, useEffect, useRef, useState } from 'react'
import { useLocation, useParams, useSearchParams } from 'react-router'

import { fetchCategoryOptions } from '@/apis/categoriesApi'
import { fetchCountries } from '@/apis/countriesApi'
import { fetchProductDetail } from '@/apis/productsApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { CatalogProduct, ProductFormTab } from '@/features/master-data/products/types'
import type { Country } from '@/features/master-data/types'
import type { FormFieldOption } from '@/modules/crud/form/types'

const TAB_KEYS: ProductFormTab[] = ['product', 'images', 'attributes', 'faqs', 'contents']

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

export function useProductFormPage() {
  const { productId: routeProductId } = useParams()
  const location = useLocation()
  const [searchParams, setSearchParams] = useSearchParams()
  const { showNotification } = useNotificationContext()

  const isNew = isNewProductRoute(routeProductId, location.pathname)
  const productId = isNew ? null : (routeProductId ?? null)
  const activeTab = parseTab(searchParams.get('tab'))
  const contentId = searchParams.get('contentId')
  const isContentFormOpen = activeTab === 'contents' && Boolean(contentId)

  const [product, setProduct] = useState<CatalogProduct | null>(null)
  const [isLoading, setIsLoading] = useState(!isNew && Boolean(routeProductId))
  const [isSaving, setIsSaving] = useState(false)
  const [categoryOptions, setCategoryOptions] = useState<FormFieldOption[]>([])
  const [countries, setCountries] = useState<Country[]>([])

  const showNotificationRef = useRef(showNotification)
  showNotificationRef.current = showNotification

  const canAccessSubTabs = Boolean(productId)

  const reloadProduct = useCallback(async (id: string) => {
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
        showNotificationRef.current({
          title: 'Lỗi',
          message: 'Không tải được danh mục hoặc quốc gia',
          variant: 'danger',
          delay: 4000,
        })
      })
  }, [])

  useEffect(() => {
    if (isNew || !productId) return
    void reloadProduct(productId)
  }, [isNew, productId, reloadProduct])

  useEffect(() => {
    if (isNew && activeTab !== 'product') {
      setSearchParams({ tab: 'product' }, { replace: true })
    }
  }, [isNew, activeTab, setSearchParams])

  const setActiveTab = useCallback(
    (tab: ProductFormTab) => {
      if (tab !== 'product' && !canAccessSubTabs) return
      setSearchParams({ tab })
    },
    [canAccessSubTabs, setSearchParams],
  )

  const openContentForm = useCallback(
    (id: string) => {
      setSearchParams({ tab: 'contents', contentId: id })
    },
    [setSearchParams],
  )

  const closeContentForm = useCallback(() => {
    setSearchParams({ tab: 'contents' })
  }, [setSearchParams])

  const pageTitle = isNew ? 'Tạo Product' : `Chỉnh sửa: ${product?.name ?? 'Product'}`

  return {
    isNew,
    productId,
    activeTab,
    setActiveTab,
    setSearchParams,
    product,
    isLoading,
    isSaving,
    setIsSaving,
    categoryOptions,
    countries,
    canAccessSubTabs,
    reloadProduct,
    pageTitle,
    contentId,
    isContentFormOpen,
    openContentForm,
    closeContentForm,
  }
}
