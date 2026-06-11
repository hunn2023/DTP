import { tagsData } from '@/features/master-data/tags/data'
import type { Tag } from '@/features/master-data/types'

export const activeTagOptions = tagsData
  .filter((t) => t.isActive)
  .map((t) => ({
    value: String(t.id),
    label: [t.icon, t.name].filter(Boolean).join(' '),
  }))

export function getTagsByIds(ids: number[]): Tag[] {
  return ids
    .map((id) => tagsData.find((t) => t.id === id))
    .filter((t): t is Tag => t !== undefined)
}
