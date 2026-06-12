import type { DeliveriesQueryFilters } from '@/apis/deliveriesApi'
import { useDeliveriesPage } from '@/features/sales/deliveries/useDeliveriesPage'
import PagedListTable from '@/features/sales/shared/PagedListTable'
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
  const list = useDeliveriesPage({ filters })

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
