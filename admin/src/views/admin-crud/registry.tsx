import { Navigate, type RouteObject } from 'react-router'

import ReportPage from '@/views/admin-crud/components/ReportPage'
import { createCrudPage } from '@/views/admin-crud/createCrudPage'
import { customerEntities } from '@/views/admin-crud/entities/customers.entities'
import {
  marketingEntities,
  systemEntities,
} from '@/views/admin-crud/entities/marketing-system.entities'
import { productEntities } from '@/views/admin-crud/entities/products.entities'
import { providerEntities } from '@/views/admin-crud/entities/providers.entities'
import { reportEntities } from '@/views/admin-crud/entities/reports.entities'
import { salesEntities } from '@/views/admin-crud/entities/sales.entities'
import { websiteEntities } from '@/views/admin-crud/entities/website.entities'
import type { ResolvedAdminEntity } from '@/views/admin-crud/schema/defineEntity'
import type { SettingsEntityBase } from '@/views/settings/types'

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

export const allAdminEntities: ResolvedAdminEntity<SettingsEntityBase>[] = [
  ...productEntities,
  ...providerEntities,
  ...salesEntities,
  ...customerEntities,
  ...websiteEntities,
  ...marketingEntities,
  ...reportEntities,
  ...systemEntities,
] as unknown as ResolvedAdminEntity<SettingsEntityBase>[]

function entityRoute(entity: ResolvedAdminEntity<SettingsEntityBase>): RouteObject {
  const kpis = reportKpiMap[entity.path]
  if (kpis) {
    const Report = () => <ReportPage entity={entity} kpis={kpis} />
    return { path: entity.path, element: <Report /> }
  }
  const Page = createCrudPage(entity)
  return { path: entity.path, element: <Page /> }
}

const redirectRoutes: RouteObject[] = [
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

export const dtpAdminRoutes: RouteObject[] = [
  ...redirectRoutes,
  ...allAdminEntities.map(entityRoute),
]
