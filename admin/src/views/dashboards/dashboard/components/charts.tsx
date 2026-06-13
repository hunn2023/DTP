import { BarChart, LineChart, PieChart } from 'echarts/charts'
import { TooltipComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import { useId } from 'react'
import { useIsClient } from 'usehooks-ts'

import CustomEChart from '@/components/CustomEChart.tsx'
import BaseVectorMap from '@/components/maps/BaseVectorMap.tsx'

import {
  getOrderStatusOptions,
  getPaymentMethodsOptions,
  getRevenueSevenDaysOptions,
  getTopCountriesBarOptions,
  getTopRegionsMapOptions,
} from '../data'

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
  return isClient ? <BaseVectorMap id={id} options={getTopRegionsMapOptions()} style={{ height: 180 }} /> : null
}
