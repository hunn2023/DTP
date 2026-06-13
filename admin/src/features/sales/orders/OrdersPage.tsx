import OrdersTable from '@/features/sales/orders/components/OrdersTable'
import type { OrderPageConfig } from '@/features/sales/orders/orderFilters'
import { ORDER_BREADCRUMB_SUBTITLE } from '@/features/sales/orders/orderFilters'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

type OrdersPageProps = {
  config: OrderPageConfig
}

const OrdersPage = ({ config }: OrdersPageProps) => (
  <EntityPageLayout
    title={config.title}
    subtitle={ORDER_BREADCRUMB_SUBTITLE}
    description={config.description}>
    <OrdersTable config={config} />
  </EntityPageLayout>
)

export default OrdersPage
