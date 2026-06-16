import { fetchCategoriesPage, fetchCategoryById } from '@/apis/categoriesApi'
import type { ApiSearchSelectOption } from '@/components/form/ApiSearchSelect'
import type { Category } from '@/features/master-data/types'

export function mapCategoryToSelectOption(category: Category): ApiSearchSelectOption {
  const code = category.code.trim() || category.slug.trim()
  const label = code ? `${code} — ${category.name}` : category.name
  return { value: category.id, label }
}

export async function searchCategorySelectOptions(keyword: string): Promise<ApiSearchSelectOption[]> {
  const result = await fetchCategoriesPage(1, 30, keyword.trim() || undefined)
  return result.items.map(mapCategoryToSelectOption)
}

export async function resolveCategorySelectOption(
  categoryId: string,
): Promise<ApiSearchSelectOption | null> {
  if (!categoryId) return null

  const category = await fetchCategoryById(categoryId)
  if (category) return mapCategoryToSelectOption(category)

  const page = await fetchCategoriesPage(1, 100)
  const found = page.items.find((item) => item.id === categoryId)
  return found ? mapCategoryToSelectOption(found) : null
}
