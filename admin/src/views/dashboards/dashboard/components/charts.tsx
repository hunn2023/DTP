import type { EChartsOption } from 'echarts'
import { BarChart, LineChart, PieChart } from 'echarts/charts'
import { TooltipComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'

import CustomEChart from '@/components/CustomEChart.tsx'

type ChartProps = {
  getOptions: () => EChartsOption
  height?: number
}

export const RevenueSevenDaysChart = ({ getOptions, height = 280 }: ChartProps) => (
  <CustomEChart
    extensions={[LineChart, TooltipComponent, CanvasRenderer]}
    getOptions={getOptions}
    style={{ height }}
  />
)

export const OrderStatusChart = ({ getOptions, height = 280 }: ChartProps) => (
  <CustomEChart
    extensions={[PieChart, TooltipComponent, CanvasRenderer]}
    getOptions={getOptions}
    style={{ height }}
  />
)

export const TopItemsBarChart = ({ getOptions, height = 280 }: ChartProps) => (
  <CustomEChart
    extensions={[BarChart, TooltipComponent, CanvasRenderer]}
    getOptions={getOptions}
    style={{ height }}
  />
)

/** @deprecated use TopItemsBarChart */
export const TopProvidersBarChart = TopItemsBarChart

export const PaymentMethodsChart = ({ getOptions, height = 220 }: ChartProps) => (
  <CustomEChart
    extensions={[PieChart, TooltipComponent, CanvasRenderer]}
    getOptions={getOptions}
    style={{ height }}
  />
)
