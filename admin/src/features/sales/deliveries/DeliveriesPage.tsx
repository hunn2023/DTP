import { useCallback, useMemo } from 'react'

import { buildDeliveryColumns } from '@/features/sales/deliveries/columns'
import { fetchDeliveriesPage, type DeliveriesQueryFilters } from '@/features/sales/deliveries/deliveries.api'
import PagedListTable from '@/features/sales/shared/PagedListTable'
import { usePagedList } from '@/features/sales/shared/usePagedList'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

type DeliveriesPageProps = {
  title: string
  description: string
  filters?: DeliveriesQueryFilters
  searchPlaceholder?: string
}

const DeliveriesPage = ({
  title,
  description,
  filters = {},
  searchPlaceholder = 'Tìm mã đơn, khách...',
}: DeliveriesPageProps) => {
  const fetchPage = useCallback(
    (pageIndex: number, pageSize: number, keyword?: string) =>
      fetchDeliveriesPage(pageIndex + 1, pageSize, keyword, filters),
    [filters],
  )

  const buildColumns = useCallback(() => buildDeliveryColumns(), [])

  const filterKey = useMemo(() => String(filters.status ?? ''), [filters.status])

  const list = usePagedList({
    fetchPage,
    buildColumns,
    reloadKey: filterKey,
    emptyMessage: 'Chưa có giao hàng',
  })

  return (
    <EntityPageLayout title={title} subtitle="Bán hàng" description={description}>
      <PagedListTable
        searchPlaceholder={searchPlaceholder}
        loadingLabel="Đang tải giao hàng..."
        {...list}
      />
    </EntityPageLayout>
  )
}

export default DeliveriesPage
