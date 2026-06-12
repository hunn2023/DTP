import { useCallback, useMemo } from 'react'

import { buildOrderColumns } from '@/features/sales/orders/columns'
import type { OrderPageConfig } from '@/features/sales/orders/orderFilters'
import { ORDER_BREADCRUMB_SUBTITLE } from '@/features/sales/orders/orderFilters'
import { fetchOrdersPage } from '@/apis/ordersApi'
import PagedListTable from '@/features/sales/shared/PagedListTable'
import { usePagedList } from '@/features/sales/shared/usePagedList'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

type OrdersPageProps = {
  config: OrderPageConfig
}

const OrdersPage = ({ config }: OrdersPageProps) => {
  const fetchPage = useCallback(
    (pageIndex: number, pageSize: number, keyword?: string) =>
      fetchOrdersPage(pageIndex + 1, pageSize, keyword, config.filters),
    [config.filters],
  )

  const buildColumns = useCallback(() => buildOrderColumns(), [])

  const filterKey = useMemo(
    () => `${config.filters.status ?? ''}:${config.filters.paymentStatus ?? ''}`,
    [config.filters.status, config.filters.paymentStatus],
  )

  const list = usePagedList({
    fetchPage,
    buildColumns,
    reloadKey: filterKey,
    emptyMessage: 'Chưa có đơn hàng',
  })

  return (
    <EntityPageLayout
      title={config.title}
      subtitle={ORDER_BREADCRUMB_SUBTITLE}
      description={config.description}>
      <PagedListTable
        searchPlaceholder={config.searchPlaceholder}
        loadingLabel="Đang tải đơn hàng..."
        {...list}
      />
    </EntityPageLayout>
  )
}

export default OrdersPage
