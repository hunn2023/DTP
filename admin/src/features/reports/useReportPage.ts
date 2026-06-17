import { useCallback, useEffect, useState } from 'react'

import { getDefaultReportFilters } from '@/features/reports/reportDateRange'
import type { ReportFilters } from '@/features/reports/reportTypes'
import { getErrorMessage } from '@/features/system/shared/getErrorMessage'

export function useReportPage<T>(fetcher: (filters: ReportFilters) => Promise<T>) {
  const [filters, setFilters] = useState<ReportFilters>(() => getDefaultReportFilters())
  const [appliedFilters, setAppliedFilters] = useState<ReportFilters>(() => getDefaultReportFilters())
  const [data, setData] = useState<T | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const loadData = useCallback(async () => {
    setIsLoading(true)
    setError(null)
    try {
      setData(await fetcher(appliedFilters))
    } catch (err) {
      setError(getErrorMessage(err, 'Không tải được báo cáo'))
      setData(null)
    } finally {
      setIsLoading(false)
    }
  }, [appliedFilters, fetcher])

  useEffect(() => {
    void loadData()
  }, [loadData])

  const applyFilters = useCallback(() => {
    setAppliedFilters(filters)
  }, [filters])

  return {
    filters,
    setFilters,
    appliedFilters,
    applyFilters,
    /** @deprecated use filters / setFilters */
    range: filters,
    setRange: setFilters,
    appliedRange: appliedFilters,
    applyRange: applyFilters,
    data,
    isLoading,
    error,
    reload: loadData,
  }
}
