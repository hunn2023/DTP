import { useCallback, useEffect, useRef, useState } from 'react'
import { useLocation, useParams, useSearchParams } from 'react-router'

import { fetchProductDetail } from '@/apis/productsApi'
import type { CatalogProduct, ProductFormTab } from '@/features/master-data/products/types'

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

  const isNew = isNewProductRoute(routeProductId, location.pathname)
  const productId = isNew ? null : (routeProductId ?? null)
  const activeTab = parseTab(searchParams.get('tab'))
  const contentId = searchParams.get('contentId')
  const isContentFormOpen = activeTab === 'contents' && Boolean(contentId)

  const [product, setProduct] = useState<CatalogProduct | null>(null)
  const [isLoading, setIsLoading] = useState(!isNew && Boolean(routeProductId))
  const [isSaving, setIsSaving] = useState(false)
  const [dirtyTabs, setDirtyTabs] = useState<Set<ProductFormTab>>(new Set())
  const [pendingTab, setPendingTab] = useState<ProductFormTab | null>(null)
  const [showUnsavedModal, setShowUnsavedModal] = useState(false)
  const [formResetNonce, setFormResetNonce] = useState(0)

  const productSaveRef = useRef<(() => Promise<boolean>) | null>(null)

  const canAccessSubTabs = Boolean(productId)

  const markTabDirty = useCallback((tab: ProductFormTab, dirty: boolean) => {
    setDirtyTabs((prev) => {
      if (prev.has(tab) === dirty) return prev
      const next = new Set(prev)
      if (dirty) next.add(tab)
      else next.delete(tab)
      return next
    })
  }, [])

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

  const requestTabChange = useCallback(
    (tab: ProductFormTab) => {
      if (tab === activeTab) return
      if (tab !== 'product' && !canAccessSubTabs) return
      if (dirtyTabs.has(activeTab)) {
        setPendingTab(tab)
        setShowUnsavedModal(true)
        return
      }
      setActiveTab(tab)
    },
    [activeTab, canAccessSubTabs, dirtyTabs, setActiveTab],
  )

  const registerProductSave = useCallback((save: (() => Promise<boolean>) | null) => {
    productSaveRef.current = save
  }, [])

  const discardPendingTabChange = useCallback(() => {
    if (!pendingTab) return
    setFormResetNonce((n) => n + 1)
    markTabDirty(activeTab, false)
    setActiveTab(pendingTab)
    setPendingTab(null)
    setShowUnsavedModal(false)
  }, [pendingTab, activeTab, markTabDirty, setActiveTab])

  const confirmUnsavedSave = useCallback(async () => {
    if (activeTab === 'product') {
      const ok = (await productSaveRef.current?.()) ?? false
      if (!ok) return
    }
    if (pendingTab) {
      setActiveTab(pendingTab)
      setPendingTab(null)
    }
    setShowUnsavedModal(false)
  }, [activeTab, pendingTab, setActiveTab])

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
    requestTabChange,
    setSearchParams,
    product,
    isLoading,
    isSaving,
    setIsSaving,
    canAccessSubTabs,
    reloadProduct,
    pageTitle,
    contentId,
    isContentFormOpen,
    openContentForm,
    closeContentForm,
    markTabDirty,
    showUnsavedModal,
    setShowUnsavedModal,
    confirmUnsavedSave,
    discardPendingTabChange,
    registerProductSave,
    formResetNonce,
  }
}
