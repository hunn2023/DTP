import { useCallback, useMemo } from 'react'

import { fetchDeliveriesPage, type DeliveriesQueryFilters } from '@/apis/deliveriesApi'
import { buildDeliveryColumns } from '@/features/sales/deliveries/columns'
import { usePagedList } from '@/features/sales/shared/usePagedList'

type UseDeliveriesPageParams = {
  filters?: DeliveriesQueryFilters
}

export function useDeliveriesPage({ filters = {} }: UseDeliveriesPageParams = {}) {
  const fetchPage = useCallback(
    (pageIndex: number, pageSize: number, keyword?: string) =>
      fetchDeliveriesPage(pageIndex + 1, pageSize, keyword, filters),
    [filters],
  )

  const buildColumns = useCallback(() => buildDeliveryColumns(), [])

  const filterKey = useMemo(() => String(filters.status ?? ''), [filters.status])

  return usePagedList({
    fetchPage,
    buildColumns,
    reloadKey: filterKey,
    emptyMessage: 'Chưa có giao hàng',
  })
}
