type Raw = Record<string, unknown>

export function readString(raw: Raw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  return value == null ? '' : String(value)
}

export function readBool(raw: Raw, camel: string, pascal: string): boolean {
  const value = raw[camel] ?? raw[pascal]
  return Boolean(value)
}

export function readNumber(raw: Raw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  return typeof value === 'number' ? value : Number(value ?? 0)
}

export function readOptionalNumber(raw: Raw, camel: string, pascal: string): number | null {
  const value = raw[camel] ?? raw[pascal]
  if (value == null || value === '') return null
  return typeof value === 'number' ? value : Number(value)
}

export function readDateString(raw: Raw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  if (value == null || value === '') return ''
  const text = String(value)
  return text.length >= 10 ? text.slice(0, 10) : text
}

export function normalizePaged<T>(
  raw: Raw,
  mapItem: (item: Raw) => T,
): { items: T[]; totalCount: number; pageIndex: number; pageSize: number } {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as Raw[]
  return {
    items: Array.isArray(itemsRaw) ? itemsRaw.map(mapItem) : [],
    totalCount: readNumber(raw, 'totalCount', 'TotalCount'),
    pageIndex: readNumber(raw, 'pageIndex', 'PageIndex'),
    pageSize: readNumber(raw, 'pageSize', 'PageSize'),
  }
}
