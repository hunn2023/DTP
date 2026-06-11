import { useCallback, useEffect, useRef, useState } from 'react'

import { fetchProductPrices } from '@/features/master-data/product-prices/product-prices.api'
import { fetchProductVariants } from '@/features/master-data/products/product-variants.api'
import { fetchProductDetail, fetchProductsPage } from '@/features/master-data/products/products.api'
import type { ProductPriceRow, ProductVariant } from '@/features/master-data/products/types'
import { fetchCountries } from '@/features/master-data/countries/countries.api'
import { fetchEsimPackageDetail } from '@/features/products/esim-packages/esim-packages.api'
import type { EsimPackage } from '@/features/products/esim-packages/types'
import { mapPackageToForm } from '@/features/products/esim-wizard/mapPackageForm'
import { fetchVariantFeatures } from '@/features/products/esim-wizard/product-variant-features.api'
import type { EsimPackageForm, EsimWizardTab } from '@/features/products/esim-wizard/types'
import { getDefaultPackageValues } from '@/features/products/esim-wizard/wizardDefaults'
import { fetchProviderOptions } from '@/features/providers/providers.api'
import type { FormFieldOption } from '@/modules/crud/form/types'

type LoaderParams = {
  isNew: boolean
  variantId: string | null
  productIdParam: string | null
  packageIdParam: string | null
  activeTab: EsimWizardTab
  onBootstrapError: () => void
}

type WizardBootstrapSnapshot = {
  productId: string
  variant: ProductVariant | null
  productName: string
  price: ProductPriceRow | null
  esimPackage: EsimPackage | null
  packageForm: EsimPackageForm | null
  selectedCarrierIds: string[]
  providerName: string
  defaultCountryId: string
  featureCount: number
  loadedTabs: EsimWizardTab[]
}

const bootstrapSnapshotCache = new Map<string, WizardBootstrapSnapshot>()
const bootstrapInflight = new Map<string, Promise<WizardBootstrapSnapshot>>()
const wizardLoadedTabsByVariant = new Map<string, Set<EsimWizardTab>>()

function buildBootstrapKey(
  variantId: string,
  productIdParam: string | null,
  packageIdParam: string | null,
  mode: 'review' | 'core',
): string {
  return `${variantId}:${productIdParam ?? ''}:${packageIdParam ?? ''}:${mode}`
}

function getWizardLoadedTabs(variantId: string): Set<EsimWizardTab> {
  let tabs = wizardLoadedTabsByVariant.get(variantId)
  if (!tabs) {
    tabs = new Set()
    wizardLoadedTabsByVariant.set(variantId, tabs)
  }
  return tabs
}

function clearBootstrapCacheForVariant(variantId: string): void {
  const prefix = `${variantId}:`
  for (const key of bootstrapSnapshotCache.keys()) {
    if (key.startsWith(prefix)) bootstrapSnapshotCache.delete(key)
  }
  for (const key of bootstrapInflight.keys()) {
    if (key.startsWith(prefix)) bootstrapInflight.delete(key)
  }
}

async function loadBootstrapSnapshot(
  key: string,
  fetchSnapshot: () => Promise<WizardBootstrapSnapshot>,
): Promise<WizardBootstrapSnapshot> {
  const cached = bootstrapSnapshotCache.get(key)
  if (cached) return cached

  const inflight = bootstrapInflight.get(key)
  if (inflight) return inflight

  const promise = fetchSnapshot()
    .then((snapshot) => {
      bootstrapSnapshotCache.set(key, snapshot)
      bootstrapInflight.delete(key)
      return snapshot
    })
    .catch((error) => {
      bootstrapInflight.delete(key)
      throw error
    })

  bootstrapInflight.set(key, promise)
  return promise
}

function mapProductOptions(items: { id: string; name: string }[]): FormFieldOption[] {
  return items.map((p) => ({ value: p.id, label: p.name }))
}

function mapCountryOptions(
  countries: Awaited<ReturnType<typeof fetchCountries>>,
): FormFieldOption[] {
  return countries.map((c) => ({ value: c.id, label: `${c.isoCode} ${c.name}` }))
}

function variantFromPackage(pkg: EsimPackage, variantId: string): ProductVariant {
  return {
    id: variantId,
    productId: pkg.productId,
    sku: '',
    name: pkg.productVariantName,
    shortName: '',
    description: '',
    sortOrder: pkg.sortOrder,
    isActive: pkg.isActive,
  }
}

export function useEsimWizardLoader({
  isNew,
  variantId,
  productIdParam,
  packageIdParam,
  activeTab,
  onBootstrapError,
}: LoaderParams) {
  const [isLoading, setIsLoading] = useState(!isNew)
  const [isTabLoading, setIsTabLoading] = useState(false)
  const [bootstrapReady, setBootstrapReady] = useState(isNew)
  const [productOptions, setProductOptions] = useState<FormFieldOption[]>([])
  const [providerOptions, setProviderOptions] = useState<FormFieldOption[]>([])
  const [countryOptions, setCountryOptions] = useState<FormFieldOption[]>([])

  const [productId, setProductId] = useState('')
  const [variant, setVariant] = useState<ProductVariant | null>(null)
  const [price, setPrice] = useState<ProductPriceRow | null>(null)
  const [packageForm, setPackageForm] = useState<EsimPackageForm | null>(null)
  const [esimPackage, setEsimPackage] = useState<EsimPackage | null>(null)
  const [selectedCarrierIds, setSelectedCarrierIds] = useState<string[]>([])
  const [featureCount, setFeatureCount] = useState(0)
  const [productName, setProductName] = useState('')
  const [providerName, setProviderName] = useState('')
  const [defaultCountryId, setDefaultCountryId] = useState('')

  const loadedTabsRef = useRef<Set<EsimWizardTab>>(getWizardLoadedTabs(variantId ?? ''))
  const onBootstrapErrorRef = useRef(onBootstrapError)
  onBootstrapErrorRef.current = onBootstrapError

  const productIdRef = useRef(productId)
  productIdRef.current = productId

  const packageIdRef = useRef(packageIdParam)
  packageIdRef.current = packageIdParam

  const esimPackageRef = useRef(esimPackage)
  esimPackageRef.current = esimPackage

  const applyPackageState = useCallback((pkg: EsimPackage) => {
    const form = mapPackageToForm(pkg)
    setEsimPackage(pkg)
    setPackageForm(form)
    setSelectedCarrierIds(form.carrierIds)
    setProviderName(pkg.providerName)
    setDefaultCountryId(pkg.countryId)
  }, [])

  const resolvePackageId = useCallback((): string | null => {
    return packageIdRef.current || esimPackageRef.current?.id || null
  }, [])

  const loadPackageDetail = useCallback(async (): Promise<EsimPackage | null> => {
    const pkgId = resolvePackageId()
    if (!pkgId) return null

    const pkg = await fetchEsimPackageDetail(pkgId)
    if (pkg) applyPackageState(pkg)
    return pkg
  }, [resolvePackageId, applyPackageState])

  const loadProductOptions = useCallback(async () => {
    const products = await fetchProductsPage(1, 200, { isActive: true })
    setProductOptions(mapProductOptions(products.items))
  }, [])

  const loadVariantsTab = useCallback(async () => {
    const pId = productIdRef.current
    const vId = variantId
    if (!pId || !vId) return

    const [variants] = await Promise.all([fetchProductVariants(pId), loadProductOptions()])

    const found = variants.find((v) => v.id === vId) ?? null
    if (found) setVariant(found)

    const product = await fetchProductDetail(pId)
    if (product?.name) setProductName(product.name)
    if (product?.countryId) setDefaultCountryId(product.countryId)

    loadedTabsRef.current.add('variants')
  }, [variantId, loadProductOptions])

  const applyBootstrapSnapshot = useCallback((snapshot: WizardBootstrapSnapshot) => {
    setProductId(snapshot.productId)
    setVariant(snapshot.variant)
    setProductName(snapshot.productName)
    setPrice(snapshot.price)
    setEsimPackage(snapshot.esimPackage)
    setPackageForm(snapshot.packageForm)
    setSelectedCarrierIds(snapshot.selectedCarrierIds)
    setProviderName(snapshot.providerName)
    setDefaultCountryId(snapshot.defaultCountryId)
    setFeatureCount(snapshot.featureCount)

    const tabs = getWizardLoadedTabs(variantId ?? '')
    tabs.clear()
    snapshot.loadedTabs.forEach((tab) => tabs.add(tab))
    loadedTabsRef.current = tabs
  }, [variantId])

  const fetchReviewBootstrapSnapshot = useCallback(
    async (vId: string): Promise<WizardBootstrapSnapshot> => {
      const pkgId = packageIdRef.current
      const emptySnapshot = (): WizardBootstrapSnapshot => ({
        productId: '',
        variant: null,
        productName: '',
        price: null,
        esimPackage: null,
        packageForm: null,
        selectedCarrierIds: [],
        providerName: '',
        defaultCountryId: '',
        featureCount: 0,
        loadedTabs: [],
      })

      if (!pkgId) return emptySnapshot()

      const [pkg, prices, features] = await Promise.all([
        fetchEsimPackageDetail(pkgId),
        fetchProductPrices({ productVariantId: vId }),
        fetchVariantFeatures(vId),
      ])

      if (!pkg) return emptySnapshot()

      const form = mapPackageToForm(pkg)
      return {
        productId: pkg.productId || productIdParam || '',
        variant: variantFromPackage(pkg, vId),
        productName: pkg.productName,
        price: prices[0] ?? null,
        esimPackage: pkg,
        packageForm: form,
        selectedCarrierIds: form.carrierIds,
        providerName: pkg.providerName,
        defaultCountryId: pkg.countryId,
        featureCount: features.length,
        loadedTabs: ['review', 'prices'],
      }
    },
    [productIdParam],
  )

  const fetchVariantBootstrapSnapshot = useCallback(
    async (vId: string): Promise<WizardBootstrapSnapshot> => {
      const emptySnapshot = (): WizardBootstrapSnapshot => ({
        productId: '',
        variant: null,
        productName: '',
        price: null,
        esimPackage: null,
        packageForm: null,
        selectedCarrierIds: [],
        providerName: '',
        defaultCountryId: '',
        featureCount: 0,
        loadedTabs: [],
      })

      let pId = productIdParam ?? ''
      if (!pId) {
        const pkgId = packageIdRef.current
        if (pkgId) {
          const pkg = await fetchEsimPackageDetail(pkgId)
          pId = pkg?.productId ?? ''
        }
      }
      if (!pId) return emptySnapshot()

      const variants = await fetchProductVariants(pId)
      const found = variants.find((v) => v.id === vId) ?? null
      if (!found) return emptySnapshot()

      return {
        ...emptySnapshot(),
        productId: pId,
        variant: found,
      }
    },
    [productIdParam],
  )

  const loadPriceTab = useCallback(async () => {
    if (!variantId) return
    const prices = await fetchProductPrices({ productVariantId: variantId })
    setPrice(prices[0] ?? null)
    loadedTabsRef.current.add('prices')
  }, [variantId])

  const loadPackageTab = useCallback(async () => {
    const vId = variantId
    const pId = productIdRef.current
    if (!vId || !pId) return

    const packageTask = esimPackageRef.current
      ? Promise.resolve(esimPackageRef.current)
      : loadPackageDetail()

    const [providers, countries, pkg] = await Promise.all([
      fetchProviderOptions(),
      fetchCountries(),
      packageTask,
    ])

    setProviderOptions(providers)
    setCountryOptions(mapCountryOptions(countries))

    if (!pkg) {
      setPackageForm(getDefaultPackageValues(pId, vId, defaultCountryId))
    }
    loadedTabsRef.current.add('packages')
  }, [variantId, defaultCountryId, loadPackageDetail])

  const loadReviewTab = useCallback(async () => {
    if (!variantId || !productIdRef.current) return

    const tasks: Promise<unknown>[] = []
    if (!loadedTabsRef.current.has('prices')) tasks.push(loadPriceTab())
    if (!esimPackageRef.current) tasks.push(loadPackageDetail())
    await Promise.all(tasks)

    if (!loadedTabsRef.current.has('review')) {
      const features = await fetchVariantFeatures(variantId)
      setFeatureCount(features.length)
    }
    loadedTabsRef.current.add('review')
  }, [variantId, loadPriceTab, loadPackageDetail])

  const loadActiveTab = useCallback(async () => {
    if (!variantId || isNew || !productIdRef.current) return
    if (loadedTabsRef.current.has(activeTab)) return

    setIsTabLoading(true)
    try {
      if (activeTab === 'variants') await loadVariantsTab()
      if (activeTab === 'prices') await loadPriceTab()
      if (activeTab === 'packages' || activeTab === 'carriers') await loadPackageTab()
      if (activeTab === 'review') await loadReviewTab()
      if (activeTab !== 'review') loadedTabsRef.current.add(activeTab)
    } finally {
      setIsTabLoading(false)
    }
  }, [activeTab, variantId, isNew, loadVariantsTab, loadPriceTab, loadPackageTab, loadReviewTab])

  useEffect(() => {
    if (variantId) loadedTabsRef.current = getWizardLoadedTabs(variantId)
  }, [variantId])

  const invalidateTab = useCallback((tab: EsimWizardTab) => {
    loadedTabsRef.current.delete(tab)
    if (tab === 'packages') loadedTabsRef.current.delete('review')
    if (tab === 'prices') loadedTabsRef.current.delete('review')
    if (variantId) clearBootstrapCacheForVariant(variantId)
  }, [variantId])

  useEffect(() => {
    if (isNew) {
      let active = true
      void loadProductOptions()
        .then(() => {
          if (active) setBootstrapReady(true)
        })
        .catch(() => {
          if (active) onBootstrapErrorRef.current()
        })
      return () => {
        active = false
      }
    }
    if (!variantId) return

    let active = true
    const isActive = () => active

    const mode = activeTab === 'review' ? 'review' : 'core'
    const cacheKey = buildBootstrapKey(variantId, productIdParam, packageIdParam, mode)

    const fetchSnapshot = () =>
      mode === 'review'
        ? fetchReviewBootstrapSnapshot(variantId)
        : fetchVariantBootstrapSnapshot(variantId)

    const runBootstrap = async () => {
      const cached = bootstrapSnapshotCache.get(cacheKey)
      if (!cached) {
        setIsLoading(true)
        setBootstrapReady(false)
      }

      try {
        const snapshot = cached ?? (await loadBootstrapSnapshot(cacheKey, fetchSnapshot))
        if (!isActive()) return
        applyBootstrapSnapshot(snapshot)
      } catch {
        if (active) onBootstrapErrorRef.current()
      } finally {
        if (active) {
          setIsLoading(false)
          setBootstrapReady(true)
        }
      }
    }

    void runBootstrap()
    return () => {
      active = false
    }
  }, [
    isNew,
    variantId,
    productIdParam,
    packageIdParam,
    activeTab,
    loadProductOptions,
    applyBootstrapSnapshot,
    fetchReviewBootstrapSnapshot,
    fetchVariantBootstrapSnapshot,
  ])

  useEffect(() => {
    if (!bootstrapReady) return
    void loadActiveTab()
  }, [bootstrapReady, activeTab, productId, loadActiveTab])

  return {
    isLoading,
    isTabLoading,
    productOptions,
    providerOptions,
    countryOptions,
    productId,
    variant,
    price,
    setPrice,
    packageForm,
    setPackageForm,
    esimPackage,
    setEsimPackage,
    selectedCarrierIds,
    setSelectedCarrierIds,
    featureCount,
    setFeatureCount,
    productName,
    setProductName,
    providerName,
    setProviderName,
    defaultCountryId,
    setDefaultCountryId,
    invalidateTab,
    refreshPrice: loadPriceTab,
    refreshPackage: loadPackageDetail,
  }
}
