import type { Country } from '@/features/master-data/types'
import { countriesData } from '@/features/master-data/countries/data'

/** In-memory store — thay bằng httpGet khi có backend. */
let store: Country[] = [...countriesData]

export async function fetchCountries(): Promise<Country[]> {
  return [...store]
}

export async function createCountry(payload: Omit<Country, 'id'>): Promise<Country> {
  const id = Math.max(0, ...store.map((c) => c.id)) + 1
  const row = { ...payload, id } as Country
  store = [...store, row]
  return row
}

export async function updateCountry(id: number, payload: Partial<Country>): Promise<Country> {
  const index = store.findIndex((c) => c.id === id)
  if (index < 0) throw new Error(`Country id=${id} not found`)
  const updated = { ...store[index], ...payload, id }
  store = store.map((c) => (c.id === id ? updated : c))
  return updated
}

export async function deleteCountry(id: number): Promise<void> {
  store = store.filter((c) => c.id !== id)
}
