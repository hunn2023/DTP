import type { DeliveriesQueryFilters } from '@/apis/deliveriesApi'
import DeliveriesTable from '@/features/sales/deliveries/components/DeliveriesTable'
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
  searchPlaceholder,
}: DeliveriesPageProps) => (
  <EntityPageLayout title={title} subtitle="Bán hàng" description={description}>
    <DeliveriesTable filters={filters} searchPlaceholder={searchPlaceholder} />
  </EntityPageLayout>
)

export default DeliveriesPage
