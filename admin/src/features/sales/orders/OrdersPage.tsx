import type { OrderPageConfig } from '@/features/sales/orders/orderFilters'
import { ORDER_BREADCRUMB_SUBTITLE } from '@/features/sales/orders/orderFilters'
import { useOrdersPage } from '@/features/sales/orders/useOrdersPage'
import PagedListTable from '@/features/sales/shared/PagedListTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

type OrdersPageProps = {
  config: OrderPageConfig
}

const OrdersPage = ({ config }: OrdersPageProps) => {
  const list = useOrdersPage(config)

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
