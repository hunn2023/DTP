import type { EChartsOption } from 'echarts'
import type { IconType } from 'react-icons'
import {
  TbChartBar,
  TbPercentage,
  TbSend,
  TbShoppingCart,
  TbWallet,
} from 'react-icons/tb'

import { getColor } from '@/helpers/color'

import idFlag from '@/assets/images/flags/id.svg'
import jpFlag from '@/assets/images/flags/jp.svg'
import krFlag from '@/assets/images/flags/kr.svg'
import sgFlag from '@/assets/images/flags/sg.svg'
import thFlag from '@/assets/images/flags/th.svg'
import usFlag from '@/assets/images/flags/us.svg'

export type DashboardStatCard = {
  id: number
  title: string
  value: string
  trend: number
  trendUp: boolean
  icon: IconType
  iconBg: string
}

export type RecentOrderRow = {
  id: string
  customer: string
  packageName: string
  country: string
  flag: string
  payment: string
  status: string
  statusVariant: 'success' | 'warning' | 'danger'
  amount: string
}

export type BestSellingPackage = {
  id: number
  name: string
  sold: number
  percent: number
}

export type RegionShare = {
  id: number
  name: string
  percent: number
}

export const dashboardHeader = {
  title: 'Tổng quan hệ thống',
  subtitle: 'Theo dõi hiệu suất kinh doanh eSIM hôm nay',
} as const

export const statCards: DashboardStatCard[] = [
  {
    id: 1,
    title: 'Doanh thu hôm nay',
    value: '128.500.000 đ',
    trend: 18.6,
    trendUp: true,
    icon: TbChartBar,
    iconBg: 'primary',
  },
  {
    id: 2,
    title: 'Đơn hàng mới',
    value: '248',
    trend: 15.3,
    trendUp: true,
    icon: TbShoppingCart,
    iconBg: 'success',
  },
  {
    id: 3,
    title: 'Đơn chờ thanh toán',
    value: '36',
    trend: 5.3,
    trendUp: false,
    icon: TbWallet,
    iconBg: 'warning',
  },
  {
    id: 4,
    title: 'eSIM đã giao',
    value: '1.842',
    trend: 21.7,
    trendUp: true,
    icon: TbSend,
    iconBg: 'info',
  },
  {
    id: 5,
    title: 'Tỷ lệ thanh toán thành công',
    value: '94,8%',
    trend: 2.1,
    trendUp: true,
    icon: TbPercentage,
    iconBg: 'secondary',
  },
]

export const recentOrders: RecentOrderRow[] = [
  {
    id: '#DTP2505180012',
    customer: 'Nguyễn Minh Anh',
    packageName: 'Japan 10GB / 7 ngày',
    country: 'Nhật Bản',
    flag: jpFlag,
    payment: 'VNPT ePay',
    status: 'Đã thanh toán',
    statusVariant: 'success',
    amount: '380.000 đ',
  },
  {
    id: '#DTP2505180011',
    customer: 'Trần Hoàng Nam',
    packageName: 'Thailand Unlimited / 15 ngày',
    country: 'Thái Lan',
    flag: thFlag,
    payment: 'QR MoMo',
    status: 'Chờ thanh toán',
    statusVariant: 'warning',
    amount: '420.000 đ',
  },
  {
    id: '#DTP2505180010',
    customer: 'Lê Thu Hà',
    packageName: 'Singapore 5GB / 5 ngày',
    country: 'Singapore',
    flag: sgFlag,
    payment: 'Thẻ Visa',
    status: 'Đã thanh toán',
    statusVariant: 'success',
    amount: '290.000 đ',
  },
  {
    id: '#DTP2505170009',
    customer: 'Phạm Quốc Bảo',
    packageName: 'Korea 5GB / 7 ngày',
    country: 'Hàn Quốc',
    flag: krFlag,
    payment: 'VNPT ePay',
    status: 'Hoàn tiền',
    statusVariant: 'danger',
    amount: '350.000 đ',
  },
  {
    id: '#DTP2505170008',
    customer: 'Võ Thị Mai',
    packageName: 'USA 10GB / 10 ngày',
    country: 'Mỹ',
    flag: usFlag,
    payment: 'QR ZaloPay',
    status: 'Đã thanh toán',
    statusVariant: 'success',
    amount: '520.000 đ',
  },
  {
    id: '#DTP2505170007',
    customer: 'Đặng Văn Tú',
    packageName: 'Indonesia 8GB / 7 ngày',
    country: 'Indonesia',
    flag: idFlag,
    payment: 'Chuyển khoản',
    status: 'Chờ thanh toán',
    statusVariant: 'warning',
    amount: '310.000 đ',
  },
]

export const bestSellingPackages: BestSellingPackage[] = [
  { id: 1, name: 'Japan 10GB / 7 ngày', sold: 652, percent: 30 },
  { id: 2, name: 'Thailand Unlimited / 15 ngày', sold: 541, percent: 25 },
  { id: 3, name: 'Singapore 5GB / 5 ngày', sold: 398, percent: 18 },
  { id: 4, name: 'Korea 5GB / 7 ngày', sold: 312, percent: 14 },
  { id: 5, name: 'USA 10GB / 10 ngày', sold: 245, percent: 11 },
]

export const regionShares: RegionShare[] = [
  { id: 1, name: 'Châu Á', percent: 78.6 },
  { id: 2, name: 'Bắc Mỹ', percent: 9.8 },
  { id: 3, name: 'Châu Âu', percent: 6.1 },
  { id: 4, name: 'Khác', percent: 5.5 },
]

const chartFont = () => getComputedStyle(document.body).fontFamily

export const getRevenueSevenDaysOptions = (): EChartsOption => ({
  textStyle: { fontFamily: chartFont() },
  tooltip: {
    trigger: 'axis',
    valueFormatter: (value) => `${Number(value).toLocaleString('vi-VN')} đ`,
  },
  grid: { left: 8, right: 16, top: 16, bottom: 8, containLabel: true },
  xAxis: {
    type: 'category',
    data: ['12/05', '13/05', '14/05', '15/05', '16/05', '17/05', '18/05'],
    axisLine: { show: false },
    axisTick: { show: false },
  },
  yAxis: {
    type: 'value',
    max: 160,
    axisLabel: { formatter: (v) => `${v}M` },
    splitLine: { lineStyle: { type: 'dashed', color: '#676b891f' } },
  },
  series: [
    {
      type: 'line',
      smooth: true,
      data: [72, 84, 91, 98, 112, 125, 128.5],
      itemStyle: { color: getColor('primary') },
      areaStyle: { opacity: 0.12, color: getColor('primary') },
      symbolSize: 6,
    },
  ],
})

export const getOrderStatusOptions = (): EChartsOption => ({
  tooltip: { trigger: 'item', formatter: '{b}: {d}%' },
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
        formatter: 'Tổng số\n2.126',
        fontSize: 14,
        fontWeight: 600,
        lineHeight: 20,
      },
      data: [
        { value: 71.1, name: 'Đã thanh toán', itemStyle: { color: getColor('primary') } },
        { value: 1.6, name: 'Chờ thanh toán', itemStyle: { color: getColor('warning') } },
        { value: 22.6, name: 'Đã giao eSIM', itemStyle: { color: getColor('success') } },
        { value: 4.6, name: 'Hoàn tiền', itemStyle: { color: getColor('danger') } },
      ],
    },
  ],
})

export const getTopCountriesBarOptions = (): EChartsOption => ({
  textStyle: { fontFamily: chartFont() },
  tooltip: {
    trigger: 'axis',
    axisPointer: { type: 'shadow' },
    valueFormatter: (value) => `${Number(value).toLocaleString('vi-VN')}M`,
  },
  grid: { left: 8, right: 8, top: 16, bottom: 8, containLabel: true },
  xAxis: {
    type: 'category',
    data: ['Thái Lan', 'Nhật Bản', 'Hàn Quốc', 'Singapore', 'Mỹ', 'Indonesia'],
    axisTick: { show: false },
  },
  yAxis: {
    type: 'value',
    max: 120,
    axisLabel: { formatter: (v) => `${v}M` },
    splitLine: { lineStyle: { type: 'dashed', color: '#676b891f' } },
  },
  series: [
    {
      type: 'bar',
      barWidth: 18,
      data: [104.2, 86.7, 71.5, 58.6, 42.1, 33.8],
      itemStyle: {
        borderRadius: [4, 4, 0, 0],
        color: getColor('primary'),
      },
    },
  ],
})

export const getPaymentMethodsOptions = (): EChartsOption => ({
  tooltip: { trigger: 'item', formatter: '{b}: {d}%' },
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
        formatter: 'Tổng giao dịch\n2.126',
        fontSize: 13,
        fontWeight: 600,
        lineHeight: 18,
      },
      data: [
        { value: 58.4, name: 'QR (MoMo, ZaloPay)', itemStyle: { color: getColor('primary') } },
        { value: 32.7, name: 'VNPT ePay', itemStyle: { color: getColor('info') } },
        { value: 6.1, name: 'Thẻ quốc tế', itemStyle: { color: getColor('warning') } },
        { value: 2.8, name: 'Chuyển khoản', itemStyle: { color: getColor('secondary') } },
      ],
    },
  ],
})

export const getTopRegionsMapOptions = () => ({
  type: 'world',
  zoomOnScroll: false,
  zoomButtons: false,
  markersSelectable: false,
  regionStyle: {
    initial: { fill: '#e9ecef', stroke: '#ced4da', strokeWidth: 0.4, fillOpacity: 1 },
  },
  markerStyle: {
    initial: { fill: getColor('primary'), stroke: getColor('primary'), fillOpacity: 0.35, r: 6 },
  },
  markers: [
    { name: 'Asia', coords: [15, 105] },
    { name: 'North America', coords: [45, -100] },
    { name: 'Europe', coords: [50, 10] },
  ],
  labels: { markers: { render: () => '' } },
})

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
