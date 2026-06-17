import { lazy, Suspense, useId, useMemo } from 'react'
import { useIsClient } from 'usehooks-ts'

import Loader from '@/components/Loader'
import {
  buildTopCountriesWorldMapOptions,
} from '@/features/dashboard/dashboardWorldMap'
import type { ReportTopItem } from '@/features/reports/reportTypes'

const BaseVectorMap = lazy(() => import('@/components/maps/BaseVectorMap'))

type TopCountriesWorldMapProps = {
  items: ReportTopItem[]
  height?: number
}

const TopCountriesWorldMap = ({ items, height = 280 }: TopCountriesWorldMapProps) => {
  const isClient = useIsClient()
  const reactId = useId().replace(/:/g, '')
  const mapSeed = useMemo(
    () => items.map((item) => `${item.name}-${item.value}-${item.count}`).join('|'),
    [items],
  )
  const options = useMemo(() => buildTopCountriesWorldMapOptions(items), [items])

  if (!isClient) return <Loader height={`${height}px`} />

  return (
    <Suspense fallback={<Loader height={`${height}px`} />}>
      <BaseVectorMap
        key={mapSeed}
        id={`dashboard-top-countries-${reactId}`}
        options={options}
        style={{ height, width: '100%' }}
      />
    </Suspense>
  )
}

export default TopCountriesWorldMap
