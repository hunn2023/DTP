import type { ProductContent, ProductFaq } from "@/types/productContent";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

function unwrapItems<T>(json: unknown): T[] {
  const payload = (json as { data?: unknown })?.data ?? json;
  if (Array.isArray(payload)) return payload as T[];
  const items = (payload as { items?: unknown })?.items;
  return Array.isArray(items) ? (items as T[]) : [];
}

export async function getProductContents(productId: string): Promise<ProductContent[]> {
  try {
    const response = await fetchWithAuth(
      `/api/public/product-contents/by-product/${productId}`
    );
    if (!response.ok) return [];
    const items = unwrapItems<ProductContent>(await response.json());
    return items
      .filter((item) => item.isActive)
      .sort((a, b) => a.sortOrder - b.sortOrder || a.contentType - b.contentType);
  } catch {
    return [];
  }
}

export async function getProductContentsByType(
  productId: string,
  contentType: number
): Promise<ProductContent[]> {
  try {
    const response = await fetchWithAuth(
      `/api/public/product-contents/by-product/${productId}/type/${contentType}`
    );
    if (!response.ok) return [];
    const items = unwrapItems<ProductContent>(await response.json());
    return items
      .filter((item) => item.isActive)
      .sort((a, b) => a.sortOrder - b.sortOrder);
  } catch {
    return [];
  }
}

export async function getProductFaqs(productId: string): Promise<ProductFaq[]> {
  try {
    const response = await fetchWithAuth(
      `/api/public/product-faqs/by-product/${productId}`
    );
    if (!response.ok) return [];
    const items = unwrapItems<ProductFaq>(await response.json());
    return items
      .filter((item) => item.isActive && item.question && item.answer)
      .sort((a, b) => a.sortOrder - b.sortOrder);
  } catch {
    return [];
  }
}
