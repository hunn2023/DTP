import { Category, Product, PaginatedProducts, ProductQueryParams } from "@/types/product";
import type { ApiCategory, ApiProduct, PaginatedResponse } from "@/types/api";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

const PAGE_SIZE = 9;

// ─── Mappers ──────────────────────────────────────────────────────────────────

function mapApiCategory(c: ApiCategory): Category {
  return {
    id: c.id,
    name: c.name,
    slug: c.slug,
    description: "",
  };
}

function mapApiProduct(p: ApiProduct): Product {
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

export async function getCategoryBySlug(slug: string): Promise<Category | null> {
  try {
    const response = await fetchWithAuth(
      "/api/catalog/categories?PageIndex=1&PageSize=50"
    );
    if (!response.ok) return null;
    const json = await response.json();
    const payload = json.data ?? json;
    const items: ApiCategory[] = Array.isArray(payload) ? payload : payload.items ?? [];
    const found = items.find((c) => c.slug === slug);
    return found ? mapApiCategory(found) : null;
  } catch {
    return null;
  }
}

export async function getProductsByCategory(
  categorySlug: string,
  params: ProductQueryParams = {}
): Promise<PaginatedProducts> {
  try {
    // First resolve categoryId from slug
    const catRes = await fetchWithAuth(
      "/api/catalog/categories?PageIndex=1&PageSize=50"
    );
    let categoryId = "";
    if (catRes.ok) {
      const catJson = await catRes.json();
      const catPayload = catJson.data ?? catJson;
      const cats: ApiCategory[] = Array.isArray(catPayload) ? catPayload : catPayload.items ?? [];
      const cat = cats.find((c) => c.slug === categorySlug);
      if (cat) categoryId = cat.id;
    }

    const pageIndex = params.pageIndex ? Number(params.pageIndex) : 1;
    const queryParams = new URLSearchParams({
      PageIndex: String(pageIndex),
      PageSize: String(PAGE_SIZE),
    });
    if (categoryId) queryParams.set("CategoryId", categoryId);

    const response = await fetchWithAuth(
      `/api/catalog/products?${queryParams.toString()}`
    );
    if (!response.ok) return { products: [], totalPages: 0 };

    const json = await response.json();
    const payload = json.data ?? json;
    const items: ApiProduct[] = Array.isArray(payload) ? payload : payload.items ?? [];
    const totalCount = Array.isArray(payload) ? payload.length : payload.totalCount ?? 0;

    let products = items.map(mapApiProduct);

    // Client-side price filtering
    if (params.minPrice) {
      products = products.filter((p) => p.price >= Number(params.minPrice));
    }
    if (params.maxPrice) {
      products = products.filter((p) => p.price <= Number(params.maxPrice));
    }

    // Client-side sorting
    switch (params.sort) {
      case "price_asc":
        products.sort((a, b) => a.price - b.price);
        break;
      case "price_desc":
        products.sort((a, b) => b.price - a.price);
        break;
      case "newest":
        break;
      default:
        break;
    }

    const totalPages = Math.ceil(totalCount / PAGE_SIZE);
    return { products, totalPages };
  } catch {
    return { products: [], totalPages: 0 };
  }
}
