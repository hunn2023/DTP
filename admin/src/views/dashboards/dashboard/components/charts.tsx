import { BarChart, LineChart, PieChart } from 'echarts/charts'
import { TooltipComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import { lazy, Suspense, useId } from 'react'
import { useIsClient } from 'usehooks-ts'

import Loader from '@/components/Loader'
import CustomEChart from '@/components/CustomEChart.tsx'

import {
  getOrderStatusOptions,
  getPaymentMethodsOptions,
  getRevenueSevenDaysOptions,
  getTopCountriesBarOptions,
  getTopRegionsMapOptions,
} from '../data'

const BaseVectorMap = lazy(() => import('@/components/maps/BaseVectorMap.tsx'))

export const RevenueSevenDaysChart = () => (
  <CustomEChart
    extensions={[LineChart, TooltipComponent, CanvasRenderer]}
    getOptions={getRevenueSevenDaysOptions}
    style={{ height: 280 }}
  />
)

export const OrderStatusChart = () => (
  <CustomEChart
    extensions={[PieChart, TooltipComponent, CanvasRenderer]}
    getOptions={getOrderStatusOptions}
    style={{ height: 280 }}
  />
)

export const TopCountriesBarChart = () => (
  <CustomEChart
    extensions={[BarChart, TooltipComponent, CanvasRenderer]}
    getOptions={getTopCountriesBarOptions}
    style={{ height: 280 }}
  />
)

export const PaymentMethodsChart = () => (
  <CustomEChart
    extensions={[PieChart, TooltipComponent, CanvasRenderer]}
    getOptions={getPaymentMethodsOptions}
    style={{ height: 220 }}
  />
)

export const TopRegionsMap = () => {
  const id = useId()
  const isClient = useIsClient()

  if (!isClient) return <Loader height="180px" />

  return (
    <Suspense fallback={<Loader height="180px" />}>
      <BaseVectorMap id={id} options={getTopRegionsMapOptions()} style={{ height: 180 }} />
    </Suspense>
  )
}
