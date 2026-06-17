import type { EChartsOption } from 'echarts'

import { formatPaymentMethod } from '@/features/sales/shared/format'
import { formatOrderStatusLabel, formatReportDateLabel } from '@/features/reports/reportFormat'
import type { OrdersReport, PaymentsReport, ReportTimeSeriesPoint, ReportTopItem } from '@/features/reports/reportTypes'
import { getColor } from '@/helpers/color'

const CHART_COLORS = ['primary', 'success', 'warning', 'danger', 'info', 'secondary'] as const

function chartFont(): string {
  return getComputedStyle(document.body).fontFamily
}

function formatAxisMoney(value: number): string {
  if (value >= 1_000_000) return `${(value / 1_000_000).toFixed(1)}M`
  if (value >= 1_000) return `${(value / 1_000).toFixed(0)}K`
  return String(value)
}

function buildSharePercent(items: ReportTopItem[], pick: (item: ReportTopItem) => number): number[] {
  const values = items.map(pick)
  const total = values.reduce((sum, value) => sum + value, 0)
  if (total <= 0) return values.map(() => 0)
  return values.map((value) => Math.round((value / total) * 1000) / 10)
}

export function buildRevenueChartOptions(points: ReportTimeSeriesPoint[]): EChartsOption {
  const labels = points.map((point) => formatReportDateLabel(point.label, point.date))
  const values = points.map((point) => point.value)
  const maxValue = Math.max(...values, 0)

  return {
    textStyle: { fontFamily: chartFont() },
    tooltip: {
      trigger: 'axis',
      valueFormatter: (value) => `${Number(value).toLocaleString('vi-VN')} VND`,
    },
    grid: { left: 8, right: 16, top: 16, bottom: 8, containLabel: true },
    xAxis: {
      type: 'category',
      data: labels.length > 0 ? labels : ['—'],
      axisLine: { show: false },
      axisTick: { show: false },
    },
    yAxis: {
      type: 'value',
      max: maxValue > 0 ? undefined : 100,
      axisLabel: { formatter: (value) => formatAxisMoney(Number(value)) },
      splitLine: { lineStyle: { type: 'dashed', color: '#676b891f' } },
    },
    series: [
      {
        type: 'line',
        smooth: true,
        data: values.length > 0 ? values : [0],
        itemStyle: { color: getColor('primary') },
        areaStyle: { opacity: 0.12, color: getColor('primary') },
        symbolSize: 6,
      },
    ],
  }
}

export function buildOrderStatusChartOptions(ordersReport: OrdersReport | null): EChartsOption {
  const items = ordersReport?.ordersByStatus ?? []
  const total = items.reduce((sum, item) => sum + item.count, 0)
  const pieData = items.map((item, index) => ({
    value: item.count,
    name: formatOrderStatusLabel(item.code, item.name),
    itemStyle: { color: getColor(CHART_COLORS[index % CHART_COLORS.length]) },
  }))

  return {
    tooltip: { trigger: 'item', formatter: '{b}: {c} ({d}%)' },
    legend: {
      orient: 'vertical',
      right: 0,
      top: 'middle',
      textStyle: { fontSize: 11 },
    },
    series: [
      {
        type: 'pie',
        radius: ['58%', '78%'],
        center: ['38%', '50%'],
        label: {
          show: true,
          position: 'center',
          formatter: `Tổng số\n${total.toLocaleString('vi-VN')}`,
          fontSize: 14,
          fontWeight: 600,
          lineHeight: 20,
        },
        data: pieData.length > 0 ? pieData : [{ value: 1, name: 'Chưa có dữ liệu', itemStyle: { color: '#dee2e6' } }],
      },
    ],
  }
}

export function buildTopItemsBarOptions(items: ReportTopItem[]): EChartsOption {
  const names = items.map((item) => item.name || item.code || '—')
  const values = items.map((item) => item.value)
  const maxValue = Math.max(...values, 0)

  return {
    textStyle: { fontFamily: chartFont() },
    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'shadow' },
      valueFormatter: (value) => `${Number(value).toLocaleString('vi-VN')} VND`,
    },
    grid: { left: 8, right: 8, top: 16, bottom: 8, containLabel: true },
    xAxis: {
      type: 'category',
      data: names.length > 0 ? names : ['—'],
      axisTick: { show: false },
      axisLabel: { interval: 0, width: 72, overflow: 'truncate' },
    },
    yAxis: {
      type: 'value',
      max: maxValue > 0 ? undefined : 100,
      axisLabel: { formatter: (value) => formatAxisMoney(Number(value)) },
      splitLine: { lineStyle: { type: 'dashed', color: '#676b891f' } },
    },
    series: [
      {
        type: 'bar',
        barWidth: 18,
        data: values.length > 0 ? values : [0],
        itemStyle: {
          borderRadius: [4, 4, 0, 0],
          color: getColor('primary'),
        },
      },
    ],
  }
}

export function buildPaymentMethodsChartOptions(paymentsReport: PaymentsReport | null): EChartsOption {
  const items = paymentsReport?.paymentsByMethod ?? []
  const total = items.reduce((sum, item) => sum + item.count, 0)
  const pieData = items.map((item, index) => ({
    value: item.count,
    name: formatPaymentMethod(item.name || item.code),
    itemStyle: { color: getColor(CHART_COLORS[index % CHART_COLORS.length]) },
  }))

  return {
    tooltip: { trigger: 'item', formatter: '{b}: {c} ({d}%)' },
    legend: {
      orient: 'vertical',
      right: 0,
      top: 'middle',
      textStyle: { fontSize: 11 },
    },
    series: [
      {
        type: 'pie',
        radius: ['58%', '78%'],
        center: ['38%', '50%'],
        label: {
          show: true,
          position: 'center',
          formatter: `Tổng giao dịch\n${total.toLocaleString('vi-VN')}`,
          fontSize: 13,
          fontWeight: 600,
          lineHeight: 18,
        },
        data: pieData.length > 0 ? pieData : [{ value: 1, name: 'Chưa có dữ liệu', itemStyle: { color: '#dee2e6' } }],
      },
    ],
  }
}

export function buildTopProductShares(items: ReportTopItem[]): Array<{ name: string; sold: number; percent: number }> {
  const percents = buildSharePercent(items, (item) => item.count)
  return items.map((item, index) => ({
    name: item.name || item.code || '—',
    sold: item.count,
    percent: percents[index] ?? 0,
  }))
}

export function buildTopItemShares(
  items: ReportTopItem[],
): Array<{ name: string; value: number; percent: number }> {
  const percents = buildSharePercent(items, (item) => item.value)
  return items.map((item, index) => ({
    name: item.name || item.code || '—',
    value: item.value,
    percent: percents[index] ?? 0,
  }))
}

/** @deprecated use buildTopItemShares */
export function buildTopProviderShares(items: ReportTopItem[]): Array<{ name: string; percent: number }> {
  return buildTopItemShares(items).map(({ name, percent }) => ({ name, percent }))
}
