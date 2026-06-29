import { Product, ProductDetail } from "@/types/product";
import type { ApiProduct, PaginatedResponse } from "@/types/api";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

// ─── Mappers ──────────────────────────────────────────────────────────────────

function mapApiProductToDetail(p: ApiProduct): ProductDetail {
  return {
    id: p.id,
    name: p.name,
    slug: p.slug,
    image: p.thumbnailUrl || `https://picsum.photos/seed/${p.slug}/600/600`,
    images: p.thumbnailUrl ? [p.thumbnailUrl] : [`https://picsum.photos/seed/${p.slug}/600/600`],
    sku: p.code || p.slug,
    price: p.salePrice ?? p.price ?? 0,
    salePrice: p.salePrice ?? undefined,
    originalPrice: p.originalPrice ?? p.price ?? undefined,
    stock: p.isActive ? 99 : 0,
    category: p.category?.name || "",
    categoryId: p.category?.slug || p.categoryId,
    inStock: p.isActive,
    description: p.shortDescription || undefined,
    longDescription: p.description || undefined,
    badge: p.isHot ? "Hot" : p.isFeatured ? "Nổi bật" : undefined,
  };
}

function mapApiProductToCard(p: ApiProduct): Product {
  return {
    id: p.id,
    name: p.name,
    slug: p.slug,
    image: p.thumbnailUrl || `https://picsum.photos/seed/${p.slug}/400/300`,
    price: p.salePrice ?? p.price ?? 0,
    originalPrice: p.originalPrice ?? p.price ?? undefined,
    category: p.category?.slug || "",
    inStock: p.isActive,
    badge: p.isHot ? "Hot" : p.isFeatured ? "Nổi bật" : undefined,
  };
}

// ─── API Calls ────────────────────────────────────────────────────────────────

export async function getProductBySlug(slug: string): Promise<ProductDetail | null> {
  try {
    const response = await fetchWithAuth(`/api/catalog/products/${slug}`);
    if (!response.ok) return null;
    const json = await response.json();
    const product: ApiProduct = json.data ?? json;
    return mapApiProductToDetail(product);
  } catch {
    return null;
  }
}

export async function getRelatedProducts(
  categoryId: string,
  excludeId: string
): Promise<Product[]> {
  try {
    const response = await fetchWithAuth(
      `/api/catalog/products?CategoryId=${categoryId}&PageIndex=1&PageSize=6`
    );
    if (!response.ok) return [];
    const json = await response.json();
    const payload = json.data ?? json;
    const items: ApiProduct[] = Array.isArray(payload) ? payload : payload.items ?? [];
    return items
      .filter((p) => p.id !== excludeId)
      .slice(0, 4)
      .map(mapApiProductToCard);
  } catch {
    return [];
  }
}

export async function getHomeEsimProducts(): Promise<Product[]> {
  try {
    const response = await fetchWithAuth(
      "/api/catalog/products?IsFeatured=true&PageIndex=1&PageSize=10"
    );
    if (!response.ok) return [];
    const json = await response.json();
    const payload = json.data ?? json;
    const items: ApiProduct[] = Array.isArray(payload) ? payload : payload.items ?? [];
    return items.map(mapApiProductToCard);
  } catch {
    return [];
  }
}
