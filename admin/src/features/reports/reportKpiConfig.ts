import type { IconType } from 'react-icons'
import {
  LuBadgePercent,
  LuBox,
  LuCircleCheck,
  LuClock,
  LuCreditCard,
  LuPackage,
  LuRefreshCw,
  LuTrendingUp,
  LuUsers,
  LuWallet,
  LuCircleX,
} from 'react-icons/lu'

import type { ReportKpiVariant, ReportTableAccent } from '@/features/reports/reportTypes'

export const KPI_VARIANT_CYCLE: ReportKpiVariant[] = [
  'primary',
  'success',
  'info',
  'warning',
  'danger',
  'secondary',
]

export const KPI_ICON_CYCLE: IconType[] = [
  LuWallet,
  LuTrendingUp,
  LuBox,
  LuCircleCheck,
  LuBadgePercent,
  LuRefreshCw,
]

export const TABLE_ACCENT_ICONS: Record<ReportTableAccent, IconType> = {
  primary: LuTrendingUp,
  success: LuCircleCheck,
  info: LuPackage,
  warning: LuClock,
  danger: LuCircleX,
  secondary: LuUsers,
}

export function resolveKpiVariant(index: number, override?: ReportKpiVariant): ReportKpiVariant {
  return override ?? KPI_VARIANT_CYCLE[index % KPI_VARIANT_CYCLE.length]
}

export function resolveKpiIcon(index: number): IconType {
  return KPI_ICON_CYCLE[index % KPI_ICON_CYCLE.length]
}

export const PAYMENT_KPI_ICONS: IconType[] = [
  LuCreditCard,
  LuCircleCheck,
  LuClock,
  LuCircleX,
  LuRefreshCw,
  LuWallet,
  LuBadgePercent,
]
