import { Navigate, type RouteObject } from 'react-router'

import CarriersPage from '@/features/master-data/carriers/CarriersPage'
import CategoriesPage from '@/features/master-data/categories/CategoriesPage'
import CountriesPage from '@/features/master-data/countries/CountriesPage'
import EsimPackagesPage from '@/features/products/esim-packages/EsimPackagesPage'
import PhoneCardsPage from '@/features/products/phone-cards/PhoneCardsPage'
import ProductDetailPage from '@/features/master-data/products/ProductDetailPage'
import ProductsPage from '@/features/master-data/products/ProductsPage'
import ProductPricesPage from '@/features/master-data/product-prices/ProductPricesPage'
import ProvidersPage from '@/features/providers/ProvidersPage'
import ReportPage from '@/modules/crud/components/ReportPage'
import { createCrudPage } from '@/modules/crud/createCrudPage'
import { customerEntities } from '@/modules/crud/entities/customers.entities'
import {
  marketingEntities,
  systemEntities,
} from '@/modules/crud/entities/marketing-system.entities'
import { productEntities } from '@/modules/crud/entities/products.entities'
import { providerEntities } from '@/modules/crud/entities/providers.entities'
import { reportEntities } from '@/modules/crud/entities/reports.entities'
import { salesEntities } from '@/modules/crud/entities/sales.entities'
import { settingsEntities } from '@/modules/crud/entities/settings.entities'
import { websiteEntities } from '@/modules/crud/entities/website.entities'
import type { ResolvedAdminEntity } from '@/modules/crud/schema/defineEntity'
import type { CrudEntityBase } from '@/modules/crud/types'

const reportKpiMap: Record<string, { label: string; value: string; hint?: string }[]> = {
  '/reports/revenue': [
    { label: 'Today', value: '10.000.000đ', hint: 'Net after refunds' },
    { label: 'Paid orders', value: '85' },
    { label: 'Refunds', value: '500.000đ' },
  ],
  '/reports/providers': [
    { label: 'Airalo success', value: '98%' },
    { label: 'Errors (24h)', value: '4' },
    { label: 'Revenue', value: '5.000.000đ' },
  ],
}

export const allAdminEntities: ResolvedAdminEntity<CrudEntityBase>[] = [
  ...settingsEntities,
  ...productEntities,
  ...providerEntities,
  ...salesEntities,
  ...customerEntities,
  ...websiteEntities,
  ...marketingEntities,
  ...reportEntities,
  ...systemEntities,
] as unknown as ResolvedAdminEntity<CrudEntityBase>[]

function entityRoute(entity: ResolvedAdminEntity<CrudEntityBase>): RouteObject {
  const kpis = reportKpiMap[entity.path]
  if (kpis) {
    const Report = () => <ReportPage entity={entity} kpis={kpis} />
    return { path: entity.path, element: <Report /> }
  }
  const Page = createCrudPage(entity)
  return { path: entity.path, element: <Page /> }
}

const redirectRoutes: RouteObject[] = [
  { path: '/settings', element: <Navigate to="/settings/categories" replace /> },
  { path: '/products/esim', element: <Navigate to="/products/esim/packages" replace /> },
  { path: '/products/cards-data', element: <Navigate to="/products/cards-data/telecom-cards" replace /> },
  { path: '/providers', element: <Navigate to="/providers/list" replace /> },
  { path: '/orders', element: <Navigate to="/orders/all" replace /> },
  { path: '/payments', element: <Navigate to="/payments/transactions" replace /> },
  { path: '/deliveries', element: <Navigate to="/deliveries/list" replace /> },
  { path: '/customers', element: <Navigate to="/customers/list" replace /> },
  { path: '/website/content', element: <Navigate to="/website/content/posts" replace /> },
  { path: '/website/seo', element: <Navigate to="/website/seo/homepage" replace /> },
  { path: '/marketing', element: <Navigate to="/marketing/promotions" replace /> },
  { path: '/reports', element: <Navigate to="/reports/revenue" replace /> },
  { path: '/system', element: <Navigate to="/system/admin-users" replace /> },
]

const categoriesRoute: RouteObject = {
  path: '/settings/categories',
  element: <CategoriesPage />,
}

const countriesRoute: RouteObject = {
  path: '/settings/countries',
  element: <CountriesPage />,
}

const esimPackagesRoute: RouteObject = {
  path: '/products/esim/packages',
  element: <EsimPackagesPage />,
}

const carriersRoute: RouteObject = {
  path: '/settings/carriers',
  element: <CarriersPage />,
}

const phoneCardsRoute: RouteObject = {
  path: '/products/cards-data/telecom-cards',
  element: <PhoneCardsPage />,
}

const providersListRoute: RouteObject = {
  path: '/providers/list',
  element: <ProvidersPage />,
}

const productsRoute: RouteObject = {
  path: '/settings/products',
  element: <ProductsPage />,
}

const productDetailRoute: RouteObject = {
  path: '/settings/products/:productId',
  element: <ProductDetailPage />,
}

const productPricesRoute: RouteObject = {
  path: '/settings/product-prices',
  element: <ProductPricesPage />,
}

export const dtpAdminRoutes: RouteObject[] = [
  ...redirectRoutes,
  categoriesRoute,
  countriesRoute,
  carriersRoute,
  productsRoute,
  productDetailRoute,
  productPricesRoute,
  esimPackagesRoute,
  phoneCardsRoute,
  providersListRoute,
  { path: '/products/esim/prices', element: <Navigate to="/settings/product-prices" replace /> },
  { path: '/products/cards-data/prices', element: <Navigate to="/settings/product-prices" replace /> },
  ...allAdminEntities.map(entityRoute),
]
