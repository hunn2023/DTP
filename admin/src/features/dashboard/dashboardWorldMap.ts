import { getColor } from '@/helpers/color'
import type { ReportTopItem } from '@/features/reports/reportTypes'

const COUNTRY_ISO_ALIASES: Record<string, string> = {
  japan: 'JP',
  'nhật bản': 'JP',
  'nhat ban': 'JP',
  thailand: 'TH',
  'thái lan': 'TH',
  'thai lan': 'TH',
  singapore: 'SG',
  korea: 'KR',
  'south korea': 'KR',
  'hàn quốc': 'KR',
  'han quoc': 'KR',
  china: 'CN',
  'trung quốc': 'CN',
  usa: 'US',
  'united states': 'US',
  'mỹ': 'US',
  vietnam: 'VN',
  'việt nam': 'VN',
  indonesia: 'ID',
  malaysia: 'MY',
  philippines: 'PH',
  taiwan: 'TW',
  france: 'FR',
  germany: 'DE',
  'united kingdom': 'GB',
  uk: 'GB',
  australia: 'AU',
  india: 'IN',
}

export function resolveCountryIsoCode(item: ReportTopItem): string | null {
  const code = item.code?.trim().toUpperCase()
  if (code && code.length === 2) return code

  const name = item.name?.trim() ?? ''
  if (name.length === 2) return name.toUpperCase()

  const alias = COUNTRY_ISO_ALIASES[name.toLowerCase()]
  if (alias) return alias

  return null
}

export function buildTopCountriesWorldMapOptions(items: ReportTopItem[]) {
  const values: Record<string, number> = {}

  for (const item of items) {
    const iso = resolveCountryIsoCode(item)
    if (!iso) continue
    values[iso] = Math.max(values[iso] ?? 0, item.value)
  }

  const regionCodes = Object.keys(values)
  const primary = getColor('primary')
  const primaryRgb = `rgba(var(--bs-primary-rgb), 0.85)`

  return {
    map: 'world',
    zoomOnScroll: false,
    zoomButtons: false,
    markersSelectable: false,
    selectedRegions: regionCodes,
    regionStyle: {
      initial: {
        fill: '#e9ecef',
        stroke: '#ced4da',
        strokeWidth: 0.35,
        fillOpacity: 1,
      },
      selected: {
        fill: primaryRgb,
      },
    },
    series: {
      regions: [
        {
          values,
          scale: ['#dfe6ee', primary],
          normalizeFunction: 'polynomial',
        },
      ],
    },
    labels: {
      markers: {
        render: () => '',
      },
    },
  }
}

export function buildTopCountriesLegend(
  items: ReportTopItem[],
): Array<{ label: string; value: number; mapped: boolean }> {
  return items.map((item) => ({
    label: item.name || item.code || '—',
    value: item.value,
    mapped: resolveCountryIsoCode(item) != null,
  }))
}
