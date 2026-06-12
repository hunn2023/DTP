import { useCallback, useMemo } from 'react'

import { fetchOrdersPage } from '@/apis/ordersApi'
import { buildOrderColumns } from '@/features/sales/orders/columns'
import type { OrderPageConfig } from '@/features/sales/orders/orderFilters'
import { usePagedList } from '@/features/sales/shared/usePagedList'

export function useOrdersPage(config: OrderPageConfig) {
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

  return usePagedList({
    fetchPage,
    buildColumns,
    reloadKey: filterKey,
    emptyMessage: 'Chưa có đơn hàng',
  })
}
