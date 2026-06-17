import type { EChartsOption } from 'echarts'

import { getColor } from '@/helpers/color'

export const dashboardHeader = {
  title: 'Tổng quan hệ thống',
} as const

/** Giữ cho trang Metrics (template cũ). */
export const getPieEchartOptions = (): EChartsOption => ({
  tooltip: { show: false },
  series: [
    {
      type: 'pie',
      radius: ['60%', '100%'],
      label: { show: false },
      labelLine: { show: false },
      data: [
        { value: 45, itemStyle: { color: getColor('primary') } },
        { value: 30, itemStyle: { color: getColor('secondary') } },
        { value: 25, itemStyle: { color: '#bbcae14d' } },
      ],
    },
  ],
})
