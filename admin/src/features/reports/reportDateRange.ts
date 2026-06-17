import type { ReportFilters, ReportGroupType } from '@/features/reports/reportTypes'

export function getDefaultReportFilters(): ReportFilters {
  const now = new Date()
  const from = new Date(now.getFullYear(), 0, 1)
  return {
    fromDate: toDateInputValue(from),
    toDate: toDateInputValue(now),
    groupType: 1,
  }
}

export function toDateInputValue(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

export function toReportQueryParams(filters: ReportFilters): Record<string, string | number> {
  return {
    fromDate: filters.fromDate,
    toDate: filters.toDate,
    groupType: filters.groupType,
  }
}

export function parseReportGroupType(value: string): ReportGroupType {
  const parsed = Number(value)
  if (parsed === 2 || parsed === 3 || parsed === 4) return parsed
  return 1
}

/** @deprecated use getDefaultReportFilters */
export function getDefaultReportRange() {
  return getDefaultReportFilters()
}
