import { fetchProductsPage } from '@/apis/productsApi'
import { fetchProductVariants } from '@/apis/productVariantsApi'
import type { ApiSearchSelectOption } from '@/components/form/ApiSearchSelect'

function toVariantOption(productName: string, variantId: string, variantName: string): ApiSearchSelectOption {
  return { value: variantId, label: `${productName} — ${variantName}` }
}

export async function searchProductVariantSelectOptions(
  keyword: string,
): Promise<ApiSearchSelectOption[]> {
  const q = keyword.trim().toLowerCase()
  const products = await fetchProductsPage(1, 30, {
    keyword: q || undefined,
    isActive: true,
  })

  const groups = await Promise.all(
    products.items.map(async (product) => {
      const variants = await fetchProductVariants(product.id)
      const matched = q
        ? variants.filter((variant) =>
            `${product.name} ${variant.name} ${variant.sku}`.toLowerCase().includes(q),
          )
        : variants
      return matched.map((variant) => toVariantOption(product.name, variant.id, variant.name))
    }),
  )

  return groups.flat()
}

export async function resolveProductVariantSelectOption(
  variantId: string,
): Promise<ApiSearchSelectOption | null> {
  if (!variantId) return null

  const products = await fetchProductsPage(1, 50, { isActive: true })
  for (const product of products.items) {
    const variants = await fetchProductVariants(product.id)
    const variant = variants.find((item) => item.id === variantId)
    if (variant) return toVariantOption(product.name, variant.id, variant.name)
  }

  return null
}
