import { Navigate, type RouteObject } from 'react-router'

import ArticlesPage from '@/features/content/articles/ArticlesPage'
import ArticleFormPage from '@/features/content/articles/ArticleFormPage'
import BannersPage from '@/features/content/banners/BannersPage'
import BannerFormPage from '@/features/content/banners/BannerFormPage'
import FaqsPage from '@/features/content/faqs/FaqsPage'
import PagesPage from '@/features/content/pages/PagesPage'
import PageFormPage from '@/features/content/pages/PageFormPage'
import SeoPage from '@/features/content/seo/SeoPage'
import SeoFormPage from '@/features/content/seo/SeoFormPage'
import CarriersPage from '@/features/master-data/carriers/CarriersPage'
import CategoriesPage from '@/features/master-data/categories/CategoriesPage'
import CountriesPage from '@/features/master-data/countries/CountriesPage'
import EsimPackagesPage from '@/features/products/esim-packages/EsimPackagesPage'
import EsimWizardPage from '@/features/products/esim-wizard/EsimWizardPage'
import PhoneCardsPage from '@/features/products/phone-cards/PhoneCardsPage'
import ProductFormPage from '@/features/master-data/products/ProductFormPage'
import ProductsPage from '@/features/master-data/products/ProductsPage'
import ProductPricesPage from '@/features/master-data/product-prices/ProductPricesPage'
import ProvidersPage from '@/features/providers/ProvidersPage'
import ReportPage from '@/modules/crud/components/ReportPage'
import { createCrudPage } from '@/modules/crud/createCrudPage'
import { customerEntities } from '@/modules/crud/entities/customers.entities'
import AuditLogsPage from '@/features/system/audit/AuditLogsPage'
import PermissionsPage from '@/features/system/permissions/PermissionsPage'
import RolesPage from '@/features/system/roles/RolesPage'
import UsersPage from '@/features/system/users/UsersPage'
import {
  marketingEntities,
  systemSeedEntities,
} from '@/modules/crud/entities/marketing-system.entities'
import { productEntities } from '@/modules/crud/entities/products.entities'
import { providerEntities } from '@/modules/crud/entities/providers.entities'
import { reportEntities } from '@/modules/crud/entities/reports.entities'
import { ORDER_PAGE_CONFIGS } from '@/features/sales/orders/orderFilters'
import OrdersPage from '@/features/sales/orders/OrdersPage'
import DeliveriesPage from '@/features/sales/deliveries/DeliveriesPage'
import PaymentTransactionsPage from '@/features/sales/payments/PaymentTransactionsPage'
import { salesSeedEntities } from '@/modules/crud/entities/sales.entities'
import { settingsEntities } from '@/modules/crud/entities/settings.entities'
import { websiteEntities } from '@/modules/crud/entities/website.entities'
import type { ResolvedAdminEntity } from '@/modules/crud/schema/defineEntity'
import type { CrudEntityBase } from '@/modules/crud/types'

const API_BACKED_WEBSITE_PATHS = new Set([
  '/website/banners',
  '/website/faqs',
  '/website/articles',
  '/website/pages',
  '/website/seo',
  '/website/content/banners',
  '/website/content/faqs',
  '/website/content/posts',
  '/website/content/pages',
  '/website/seo/homepage',
  '/website/seo/categories',
  '/website/seo/countries',
  '/website/seo/products',
])

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
  ...salesSeedEntities,
  ...customerEntities,
  ...websiteEntities,
  ...marketingEntities,
  ...reportEntities,
  ...systemSeedEntities,
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
  { path: '/website/content/banners', element: <Navigate to="/website/banners" replace /> },
  { path: '/website/content/faqs', element: <Navigate to="/website/faqs" replace /> },
  { path: '/website/content/posts', element: <Navigate to="/website/articles" replace /> },
  { path: '/website/content/pages', element: <Navigate to="/website/pages" replace /> },
  { path: '/website/seo/homepage', element: <Navigate to="/website/seo" replace /> },
  { path: '/website/seo/categories', element: <Navigate to="/website/seo" replace /> },
  { path: '/website/seo/countries', element: <Navigate to="/website/seo" replace /> },
  { path: '/website/seo/products', element: <Navigate to="/website/seo" replace /> },
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

const esimWizardRoute: RouteObject = {
  path: '/products/esim/wizard/:wizardId',
  element: <EsimWizardPage />,
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

const productFormRoute: RouteObject = {
  path: '/settings/products/:productId',
  element: <ProductFormPage />,
}

const productPricesRoute: RouteObject = {
  path: '/settings/product-prices',
  element: <ProductPricesPage />,
}

const systemApiRoutes: RouteObject[] = [
  { path: '/system/admin-users', element: <UsersPage /> },
  { path: '/system/roles', element: <RolesPage /> },
  { path: '/system/permissions', element: <PermissionsPage /> },
  {
    path: '/system/audit-logs',
    element: (
      <AuditLogsPage
        title="Nhật ký thao tác"
        description="Audit log từ API admin/audit/logs — chỉ xem."
      />
    ),
  },
]

const contentApiRoutes: RouteObject[] = [
  { path: '/website/banners/new', element: <BannerFormPage /> },
  { path: '/website/banners/:bannerId', element: <BannerFormPage /> },
  { path: '/website/banners', element: <BannersPage /> },
  { path: '/website/faqs', element: <FaqsPage /> },
  { path: '/website/articles/new', element: <ArticleFormPage /> },
  { path: '/website/articles/:articleId', element: <ArticleFormPage /> },
  { path: '/website/articles', element: <ArticlesPage /> },
  { path: '/website/pages/new', element: <PageFormPage /> },
  { path: '/website/pages/:pageId', element: <PageFormPage /> },
  { path: '/website/pages', element: <PagesPage /> },
  { path: '/website/seo/new', element: <SeoFormPage /> },
  { path: '/website/seo/:seoId', element: <SeoFormPage /> },
  { path: '/website/seo', element: <SeoPage /> },
]

const salesApiRoutes: RouteObject[] = [
  ...ORDER_PAGE_CONFIGS.map((config) => ({
    path: config.path,
    element: <OrdersPage config={config} />,
  })),
  {
    path: '/payments/transactions',
    element: <PaymentTransactionsPage />,
  },
  {
    path: '/deliveries/list',
    element: (
      <DeliveriesPage
        title="Danh sách giao hàng"
        description="Digital delivery: eSIM QR, mã thẻ."
      />
    ),
  },
  {
    path: '/deliveries/retry',
    element: (
      <DeliveriesPage
        title="Retry giao hàng"
        description="Đơn giao lỗi cần retry."
        filters={{ status: 4 }}
        searchPlaceholder="Tìm đơn lỗi giao hàng..."
      />
    ),
  },
]

export const dtpAdminRoutes: RouteObject[] = [
  ...redirectRoutes,
  ...systemApiRoutes,
  ...salesApiRoutes,
  ...contentApiRoutes,
  categoriesRoute,
  countriesRoute,
  carriersRoute,
  productsRoute,
  productFormRoute,
  productPricesRoute,
  esimPackagesRoute,
  esimWizardRoute,
  phoneCardsRoute,
  providersListRoute,
  { path: '/products/esim/prices', element: <Navigate to="/settings/product-prices" replace /> },
  { path: '/products/cards-data/prices', element: <Navigate to="/settings/product-prices" replace /> },
  ...allAdminEntities.filter((entity) => !API_BACKED_WEBSITE_PATHS.has(entity.path)).map(entityRoute),
]
