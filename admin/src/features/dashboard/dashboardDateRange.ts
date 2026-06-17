import type { ReportDateRange, ReportFilters } from '@/features/reports/reportTypes'
import { getDefaultReportFilters, toDateInputValue } from '@/features/reports/reportDateRange'

export function getDefaultDashboardRange(): ReportDateRange {
  const defaults = getDefaultReportFilters()
  return {
    fromDate: defaults.fromDate,
    toDate: defaults.toDate,
  }
}

export function getLast7DaysRange(): ReportDateRange {
  const to = new Date()
  const from = new Date()
  from.setDate(from.getDate() - 6)
  return {
    fromDate: toDateInputValue(from),
    toDate: toDateInputValue(to),
  }
}

export function toDashboardFilters(range: ReportDateRange): ReportFilters {
  return { ...range, groupType: 1 }
}

export function isValidDateRange(range: ReportDateRange): boolean {
  if (!range.fromDate || !range.toDate) return false
  return range.fromDate <= range.toDate
}
