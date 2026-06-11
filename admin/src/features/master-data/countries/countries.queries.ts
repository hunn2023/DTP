import { useCallback, useEffect, useState } from 'react'

import type { Country } from '@/features/master-data/types'
import * as countriesApi from '@/features/master-data/countries/countries.api'

type CountriesQueryState = {
  data: Country[]
  isLoading: boolean
  error: string | null
  refetch: () => Promise<void>
}

export function useCountriesQuery(): CountriesQueryState {
  const [data, setData] = useState<Country[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const refetch = useCallback(async () => {
    setIsLoading(true)
    setError(null)
    try {
      setData(await countriesApi.fetchCountries())
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Không tải được danh sách quốc gia')
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    void refetch()
  }, [refetch])

  return { data, isLoading, error, refetch }
}
