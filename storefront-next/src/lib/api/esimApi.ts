import type {
  EsimCountryDetail,
  EsimCountryStat,
  EsimCountrySummary,
  EsimDataRange,
  EsimPackage,
  EsimPackageFilters,
  PackageQuickTag,
} from "@/types/esim";
import type { ApiCountryHome, ApiEsimPackage, ApiProduct } from "@/types/api";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

// ─── Helpers ──────────────────────────────────────────────────────────────────

/** Normalize slug: replace spaces with hyphens, trim */
function normalizeSlug(slug: string): string {
  return slug.trim().replace(/\s+/g, "-");
}

function mapApiCountryHomeToSummary(country: ApiCountryHome): EsimCountrySummary {
  return {
    slug: normalizeSlug(country.slug),
    flag: country.flagUrl || `https://flagcdn.com/w160/${country.code?.toLowerCase()}.png`,
    name: country.name,
    region: mapRegion(country.region),
    startingPrice: country.priceFrom ?? 0,
    bestseller: country.isHot,
    packageCount: country.packageCount ?? 0,
  };
}

function mapRegion(region: string | null): EsimCountryDetail["region"] {
  if (!region) return null;
  const r = region.toLowerCase();
  if (r.includes("á") || r.includes("asia")) return "Châu Á";
  if (r.includes("âu") || r.includes("europe")) return "Châu Âu";
  if (r.includes("mỹ") || r.includes("america")) return "Châu Mỹ";
  if (r.includes("đại dương") || r.includes("oceania")) return "Châu Đại Dương";
  return null;
}

function formatDataAmount(value: number): string {
  if (!Number.isFinite(value) || value <= 0) return "0";
  return Number.isInteger(value) ? String(value) : value.toFixed(2).replace(/\.?0+$/, "");
}

function getTrueBooleanFeatureLabels(pkg: ApiEsimPackage): string[] {
  const labels: string[] = [];
  if (pkg.isUnlimited) labels.push("Data không giới hạn");
  if (pkg.hotspotSupported) labels.push("Hỗ trợ chia sẻ Hotspot");
  if (pkg.phoneNumberSupported) labels.push("Hỗ trợ số điện thoại");
  if (pkg.smsSupported) labels.push("Hỗ trợ SMS");
  if (pkg.kycRequired) labels.push("Yêu cầu KYC");
  return labels;
}

function translateActivationPolicy(value: string | null): string | null {
  if (!value) return null;
  const labels: Record<string, string> = {
    ActivateWhenInstalled: "Kích hoạt khi cài đặt eSIM",
    ActivateOnFirstUse: "Kích hoạt khi sử dụng lần đầu",
    ActivateImmediately: "Kích hoạt ngay sau khi mua",
  };
  return labels[value] ?? value;
}

function getProductAttributeStats(
  product: ApiProduct | null | undefined,
  fallbackPackageCount?: number,
  fallbackPriceFrom?: number | null
): EsimCountryStat[] {
  const attributes = (product?.attributes ?? [])
    .filter((item) => item.isVisible !== false && item.key && item.value)
    .sort((a, b) => a.sortOrder - b.sortOrder)
    .map((item) => ({ label: item.key, value: item.value }));

  if (attributes.length > 0) return attributes;

  const stats: EsimCountryStat[] = [];
  if (fallbackPackageCount !== undefined) {
    stats.push({ label: "Số gói", value: String(fallbackPackageCount) });
  }
  if (fallbackPriceFrom !== undefined && fallbackPriceFrom !== null) {
    stats.push({ label: "Giá từ", value: fallbackPriceFrom > 0 ? `${fallbackPriceFrom.toLocaleString("vi-VN")}đ` : "—" });
  }
  return stats;
}

function mapApiPackageToEsim(pkg: ApiEsimPackage): EsimPackage {
  const dataGB = pkg.isUnlimited
    ? null
    : pkg.dataUnit?.toUpperCase() === "MB" && pkg.dataAmount
      ? pkg.dataAmount / 1024
      : pkg.dataAmount ?? 0;
  const dataStr = pkg.isUnlimited ? "∞" : formatDataAmount(pkg.dataAmount ?? 0);
  const displayDataUnit = pkg.isUnlimited ? "Không giới hạn" : (pkg.dataUnit || "GB");

  const quickTags: PackageQuickTag[] = [];
  if (pkg.isUnlimited) quickTags.push("unlimited");
  if (pkg.hotspotSupported) quickTags.push("hotspot");
  if (pkg.phoneNumberSupported) quickTags.push("phone");
  if (pkg.salePrice && pkg.price && pkg.salePrice < pkg.price * 0.85) quickTags.push("cheap");

  // Bullet-list features: descriptive text (activation/speed policy).
  const features: string[] = [];
  const activationPolicy = translateActivationPolicy(pkg.activationPolicy);
  if (activationPolicy) features.push(activationPolicy);
  if (pkg.speedPolicy) features.push(pkg.speedPolicy);

  // Capability pills: only the boolean flags that are true.
  const booleanFeatures = getTrueBooleanFeatureLabels(pkg);

  const price = pkg.salePrice ?? pkg.price ?? 0;
  const oldPrice = pkg.originalPrice ?? pkg.price ?? undefined;
  const discount = oldPrice && oldPrice > price
    ? `-${Math.round((1 - price / oldPrice) * 100)}%`
    : undefined;

  return {
    id: pkg.id,
    slug: pkg.slug,
    name: pkg.name,
    image: `https://picsum.photos/seed/${pkg.slug}/640/480`,
    productId: pkg.productId,
    productVariantId: pkg.productVariantId,
    esimPackageId: pkg.id,
    data: dataStr,
    dataUnit: displayDataUnit,
    subtitle: pkg.coverageType || "",
    tag: `${pkg.validityDays} NGÀY`,
    tagType: pkg.isUnlimited ? "unlimited" : undefined,
    features,
    booleanFeatures,
    price,
    oldPrice: oldPrice !== price ? oldPrice : undefined,
    discount,
    featured: pkg.sortOrder <= 2,
    days: pkg.validityDays,
    dataGB,
    quickTags,
    stock: 999,
    rating: 4.5,
    salesCount: 100 - pkg.sortOrder,
  };
}

// ─── API Calls ────────────────────────────────────────────────────────────────

export interface CatalogThumbnailMaps {
  byVariant: Map<string, string>;
  byProduct: Map<string, string>;
}

let catalogThumbnailsPromise: Promise<CatalogThumbnailMaps> | null = null;

/**
 * Fetches all catalog variant thumbnails and indexes them by productVariantId
 * and productId. Cached at module level so repeated lookups (e.g. opening
 * several order detail popups) hit the network only once.
 */
export async function getCatalogThumbnails(): Promise<CatalogThumbnailMaps> {
  if (!catalogThumbnailsPromise) {
    catalogThumbnailsPromise = (async () => {
      const byVariant = new Map<string, string>();
      const byProduct = new Map<string, string>();
      try {
        const response = await fetchWithAuth(
          "/api/catalog/products/variants?pageSize=500"
        );
        if (response.ok) {
          const json = await response.json();
          const payload = json?.data ?? json;
          const items: ApiProductVariant[] = Array.isArray(payload) ? payload : payload?.items ?? [];
          for (const item of items) {
            if (!item.thumbnailUrl) continue;
            if (item.productVariantId) byVariant.set(item.productVariantId, item.thumbnailUrl);
            if (item.productId && !byProduct.has(item.productId)) {
              byProduct.set(item.productId, item.thumbnailUrl);
            }
          }
        }
      } catch {
        // Network failures fall back to no thumbnails — caller handles gracefully.
      }
      return { byVariant, byProduct };
    })();
  }
  return catalogThumbnailsPromise;
}

export interface HomeEsimProduct {
  id: string;
  name: string;
  slug: string;
  locationText: string | null;
  thumbnailUrl: string | null;
  flagUrl: string | null;
  region?: string | null;
  priceFrom: number;
  currency: string;
  isHot: boolean;
  isFeatured: boolean;
  packageCount?: number;
}

const ESIM_PACKAGES_PAGE_SIZE = 500;

function unwrapItems<T>(json: unknown): { items: T[]; hasNext: boolean; totalPages: number } {
  const root = json as { data?: unknown; totalPages?: number; hasNextPage?: boolean } | null;
  const payload = (root?.data ?? root) as {
    items?: T[];
    totalPages?: number;
    hasNextPage?: boolean;
  } | T[] | null;
  const items = Array.isArray(payload) ? payload : payload?.items ?? [];
  return {
    items,
    hasNext: Array.isArray(payload)
      ? items.length === ESIM_PACKAGES_PAGE_SIZE
      : Boolean(payload?.hasNextPage),
    totalPages: Array.isArray(payload) ? 1 : payload?.totalPages ?? 1,
  };
}

async function getCatalogVariants(): Promise<ApiProductVariant[]> {
  const all: ApiProductVariant[] = [];
  let pageIndex = 1;

  while (true) {
    const response = await fetchWithAuth(
      `/api/catalog/products/variants?PageIndex=${pageIndex}&PageSize=100`
    );

    if (!response.ok) break;

    const json = await response.json();
    const payload = json?.data ?? json;
    const items: ApiProductVariant[] = Array.isArray(payload)
      ? payload
      : payload?.items ?? [];

    all.push(...items);

    const hasNext =
      payload?.hasNextPage ??
      pageIndex < (payload?.totalPages ?? 1);

    if (!hasNext || items.length === 0) break;
    pageIndex++;
  }

  return all;
}

async function getCatalogEsimPackages(): Promise<ApiEsimPackage[]> {
  const all: ApiEsimPackage[] = [];
  let pageIndex = 1;

  while (true) {
    try {
      const response = await fetchWithAuth(
        `/api/catalog/esim-packages?PageIndex=${pageIndex}&PageSize=${ESIM_PACKAGES_PAGE_SIZE}`
      );
      if (!response.ok) break;
      const json = await response.json();
      if (json?.isSuccess === false) break;
      const { items, hasNext, totalPages } = unwrapItems<ApiEsimPackage>(json);
      all.push(...items);
      if (!hasNext || pageIndex >= totalPages || items.length === 0) break;
      pageIndex += 1;
    } catch {
      break;
    }
  }

  return all.filter((pkg) => pkg.isActive);
}

async function getCatalogEsimPackagesBySlug(slug: string): Promise<ApiEsimPackage[]> {
  try {
    const response = await fetchWithAuth(`/api/catalog/esim-packages/${encodeURIComponent(slug)}`);
    if (!response.ok) return [];
    const json = await response.json();
    if (json?.isSuccess === false) return [];
    const data = json?.data ?? json;
    const items = Array.isArray(data) ? data : data ? [data] : [];
    return (items as ApiEsimPackage[]).filter((pkg) => pkg.isActive);
  } catch {
    return [];
  }
}

async function getCatalogProductBySlug(slug: string): Promise<ApiProduct | null> {
  try {
    const response = await fetchWithAuth(`/api/catalog/products/${encodeURIComponent(slug)}`);
    if (!response.ok) return null;
    const json = await response.json();
    if (json?.isSuccess === false) return null;
    return (json?.data ?? json) as ApiProduct | null;
  } catch {
    return null;
  }
}

function matchesProductSlug(productSlug: string | null | undefined, slug: string): boolean {
  const normalizedProductSlug = productSlug?.trim().toLowerCase();
  const normalizedRouteSlug = slug.trim().toLowerCase();
  return (
    normalizedProductSlug === normalizedRouteSlug ||
    normalizedProductSlug === `esim-${normalizedRouteSlug}` ||
    `esim-${normalizedProductSlug}` === normalizedRouteSlug
  );
}

/** Featured eSIM products for the homepage "Điểm đến nổi bật" section. */
export async function getHomeEsimProducts(): Promise<HomeEsimProduct[]> {
  try {
    const response = await fetchWithAuth("/api/catalog/products/home/esim-products");
    if (!response.ok) return [];
    const json = await response.json();
    const data = json?.data ?? json ?? [];
    return Array.isArray(data) ? (data as HomeEsimProduct[]) : [];
  } catch {
    return [];
  }
}

async function getHomeEsimProductBySlug(slug: string): Promise<HomeEsimProduct | null> {
  const products = await getHomeEsimProducts();
  return products.find((product) => matchesProductSlug(product.slug, slug)) ?? null;
}


// Backend caps PageSize at 100; loop pages so every country is returned.
const COUNTRIES_PAGE_SIZE = 100;

/**
 * Maps each countryId to its canonical product slug (e.g. "esim-nhat-ban").
 * The variants catalog is the only endpoint that carries both countryId and
 * productSlug, so it's the reliable join between countries/home and products.
 */
async function getProductSlugByCountryId(): Promise<Map<string, string>> {
  const map = new Map<string, string>();
  try {
    const response = await fetchWithAuth("/api/catalog/products/variants?pageSize=500");
    if (!response.ok) return map;
    const json = await response.json();
    const payload = json?.data ?? json;
    const items: ApiProductVariant[] = Array.isArray(payload) ? payload : payload?.items ?? [];
    for (const item of items) {
      const productSlug = item.productSlug?.trim();
      if (item.countryId && productSlug && !map.has(item.countryId)) {
        map.set(item.countryId, productSlug);
      }
    }
  } catch {
    /* ignore — countries without a resolved product are dropped by the caller */
  }
  return map;
}

export async function getEsimCountries(): Promise<EsimCountrySummary[]> {
  try {
    const homeProducts = await getHomeEsimProducts();
    if (homeProducts.length > 0) {
      return homeProducts.map((product) => ({
        slug: product.slug,
        flag: product.flagUrl || product.thumbnailUrl || "",
        name: product.name,
        region: mapRegion(product.region ?? null),
        startingPrice: product.priceFrom ?? 0,
        bestseller: product.isHot,
        packageCount: product.packageCount ?? 0,
      }));
    }

    const all: ApiCountryHome[] = [];
    let pageIndex = 1;

    while (true) {
      try {
        const response = await fetchWithAuth(
          `/api/catalog/countries/home?PageIndex=${pageIndex}&PageSize=${COUNTRIES_PAGE_SIZE}`
        );
        if (!response.ok) break;
        const json = await response.json();
        const payload = json.data ?? json;
        const items: ApiCountryHome[] =
          Array.isArray(payload) ? payload : payload.items ?? [];
        all.push(...items);

        const hasNext = Array.isArray(payload)
          ? items.length === COUNTRIES_PAGE_SIZE
          : payload.hasNextPage ?? pageIndex < (payload.totalPages ?? 1);
        if (!hasNext || items.length === 0) break;
        pageIndex += 1;
      } catch {
        break;
      }
    }

    if (all.length === 0) return [];

    // Listing links must use the canonical product slug ("esim-nhat-ban"), not the
    // country slug ("nhat-ban"), so every card points at the one prerendered detail
    // page. Countries with no resolved product have no detail page and are dropped.
    const productSlugByCountryId = await getProductSlugByCountryId();
    return all.reduce<EsimCountrySummary[]>((acc, country) => {
      const productSlug = productSlugByCountryId.get(country.countryId);
      if (!productSlug) return acc;
      acc.push({ ...mapApiCountryHomeToSummary(country), slug: productSlug });
      return acc;
    }, []);
  } catch {
    return [];
  }
}

/** Build a placeholder country (no packages) so the detail page always renders. */
function buildEmptyEsimCountry(slug: string): EsimCountryDetail {
  const displayName = slug
    .split("-")
    .filter(Boolean)
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
    .join(" ");
  return {
    slug,
    flag: "",
    name: displayName ? `eSIM ${displayName}` : "eSIM",
    nameEn: slug,
    region: mapRegion(null),
    gradient: "from-blue-500 to-purple-600",
    textColor: "text-white",
    tagBg: "bg-white/20",
    tags: [],
    stats: [],
    features: [],
    packages: [],
  };
}

export async function getEsimCountryBySlug(slug: string): Promise<EsimCountryDetail> {
  try {
    const [allVariants, routePackages] = await Promise.all([
      getCatalogVariants(),
      getCatalogEsimPackagesBySlug(slug),
    ]);
    const variantsById = new Map(allVariants.map((v) => [v.productVariantId, v]));
    const routePackageVariantIds = new Set(routePackages.map((pkg) => pkg.productVariantId));
    const routePackageProductIds = new Set(routePackages.map((pkg) => pkg.productId));
    const items = allVariants.filter(
      (v) =>
        matchesProductSlug(v.productSlug, slug) ||
        routePackageVariantIds.has(v.productVariantId) ||
        routePackageProductIds.has(v.productId)
    );
    const [productDetail, product] = await Promise.all([
      getCatalogProductBySlug(slug),
      getHomeEsimProductBySlug(slug),
    ]);
    const esimPackages = routePackages;
    const productId = routePackages[0]?.productId || items[0]?.productId || productDetail?.id || product?.id || "";

    if (esimPackages.length === 0) {
      if (!product) return buildEmptyEsimCountry(slug);
      return {
        productId: product.id,
        slug,
        flag: productDetail?.thumbnailUrl || product.flagUrl || product.thumbnailUrl || "",
        name: productDetail?.name || product.name,
        nameEn: productDetail?.code || product.slug || slug,
        region: mapRegion(null),
        gradient: "from-blue-500 to-purple-600",
        textColor: "text-white",
        tagBg: "bg-white/20",
        tags: [productDetail?.locationText, product.locationText].filter(Boolean) as string[],
        stats: getProductAttributeStats(productDetail, product.packageCount, product.priceFrom),
        description: productDetail?.description || productDetail?.shortDescription || null,
        features: [],
        packages: [],
      };
    }

    const first = items[0];
    const firstPackage = esimPackages[0];
    const countryFeatures = Array.from(
      new Set(
        [
          ...items.flatMap((v) => v.features || []),
          ...esimPackages.flatMap((pkg) => pkg.productVariantFeatures || []),
        ]
          .sort((a, b) => a.sortOrder - b.sortOrder)
          .map((f) => f.text)
      )
    );

    const packages: EsimPackage[] = esimPackages.map((apiPackage) => {
      const variant = variantsById.get(apiPackage.productVariantId);
      const price = variant?.salePrice ?? variant?.originalPrice ?? apiPackage.salePrice ?? apiPackage.price ?? 0;
      const oldPrice =
        variant?.originalPrice && variant.originalPrice > price
          ? variant.originalPrice
          : apiPackage.originalPrice && apiPackage.originalPrice > price
            ? apiPackage.originalPrice
            : undefined;
      const discount = oldPrice ? `-${Math.round((1 - price / oldPrice) * 100)}%` : undefined;
      const mapped = mapApiPackageToEsim({ ...apiPackage, price, originalPrice: oldPrice });
      const variantFeatures = [...(variant?.features || []), ...(apiPackage.productVariantFeatures || [])]
        .sort((a: { sortOrder: number }, b: { sortOrder: number }) => a.sortOrder - b.sortOrder)
        .map((f: { text: string }) => f.text);

      return {
        ...mapped,
        name: apiPackage.name || apiPackage.productVariantName || variant?.variantShortName || variant?.variantName || mapped.name,
        image: variant?.thumbnailUrl || `https://picsum.photos/seed/${apiPackage.slug}/640/480`,
        productId: apiPackage.productId,
        productVariantId: apiPackage.productVariantId,
        esimPackageId: apiPackage.id,
        subtitle:
          apiPackage.productVariantName ||
          variant?.variantShortName ||
          variant?.variantName ||
          apiPackage.coverageType ||
          mapped.subtitle,
        price,
        oldPrice,
        discount,
        featured: Boolean(variant?.isFeatured) || mapped.featured,
        quickTags: variant?.isHot
          ? Array.from(new Set([...(mapped.quickTags ?? []), "bestseller" as PackageQuickTag]))
          : mapped.quickTags,
        features: Array.from(new Set([...variantFeatures, ...mapped.features])),
        salesCount: variant?.soldCount ?? mapped.salesCount,
      };
    });

    return {
      productId,
      slug,
      flag: productDetail?.thumbnailUrl || first?.thumbnailUrl || product?.flagUrl || product?.thumbnailUrl || "",
      name: productDetail?.name || first?.productName || firstPackage?.productName || product?.name || `eSIM ${firstPackage?.countryName ?? ""}`.trim(),
      nameEn: productDetail?.code || first?.productCode || first?.productSlug || product?.slug || slug,
      region: mapRegion(null),
      gradient: "from-blue-500 to-purple-600",
      textColor: "text-white",
      tagBg: "bg-white/20",
      tags: [productDetail?.locationText, product?.locationText].filter(Boolean) as string[],
      stats: getProductAttributeStats(productDetail, packages.length, packages.length > 0 ? Math.min(...packages.map((p) => p.price)) : product?.priceFrom),
      description: productDetail?.description || productDetail?.shortDescription || null,
      features: countryFeatures,
      packages,
    };
  } catch {
    return buildEmptyEsimCountry(slug);
  }
}

interface ApiProductVariant {
  productId: string;
  productCode: string;
  productName: string;
  productSlug: string;
  productVariantId: string;
  sku: string;
  variantName: string;
  variantShortName: string;
  variantDescription: string | null;
  categoryId: string;
  categoryName: string;
  countryId: string;
  countryName: string;
  shortDescription: string;
  locationText: string | null;
  thumbnailUrl: string | null;
  originalPrice: number;
  salePrice: number | null;
  currency: string;
  isFeatured: boolean;
  isHot: boolean;
  soldCount: number;
  productSortOrder: number;
  variantSortOrder: number;
  features: { productVariantId: string; text: string; icon: string | null; sortOrder: number }[];
}

// ─── Pure Utilities (no API call) ─────────────────────────────────────────────

export function getDataRangeForPackage(dataGB: number | null): EsimDataRange {
  if (dataGB === null) return "unlimited";
  if (dataGB <= 3) return "1-3";
  if (dataGB <= 5) return "5";
  if (dataGB <= 10) return "10";
  return "20";
}

export function getPackageCountByQuickTag(packages: EsimPackage[]): Record<PackageQuickTag, number> {
  return packages.reduce<Record<PackageQuickTag, number>>(
    (acc, pkg) => {
      for (const tag of pkg.quickTags ?? []) {
        acc[tag] += 1;
      }
      return acc;
    },
    {
      bestseller: 0,
      cheap: 0,
      "5g": 0,
      unlimited: 0,
      phone: 0,
      hotspot: 0,
    }
  );
}

export function filterEsimPackages(packages: EsimPackage[], filters: EsimPackageFilters): EsimPackage[] {
  const filtered = packages.filter((pkg) => {
    const matchesDay = filters.days.length === 0 || filters.days.includes(pkg.days);
    const matchesDataRange =
      filters.dataRanges.length === 0 || filters.dataRanges.includes(getDataRangeForPackage(pkg.dataGB));
    const matchesFeatureTags =
      filters.featureTags.length === 0 || filters.featureTags.every((tag) => pkg.features?.includes(tag));
    const matchesQuickTag = filters.quickTag === "all" || pkg.quickTags?.includes(filters.quickTag);
    const matchesMin = filters.minPrice === undefined || pkg.price >= filters.minPrice;
    const matchesMax = filters.maxPrice === undefined || pkg.price <= filters.maxPrice;
    return matchesDay && matchesDataRange && matchesFeatureTags && matchesQuickTag && matchesMin && matchesMax;
  });

  switch (filters.sort) {
    case "price_asc":
      return filtered.sort((a, b) => a.price - b.price);
    case "price_desc":
      return filtered.sort((a, b) => b.price - a.price);
    case "bestseller":
      return filtered.sort((a, b) => b.salesCount - a.salesCount);
    case "rating":
      return filtered.sort((a, b) => b.rating - a.rating || b.salesCount - a.salesCount);
    case "recommended":
    default:
      return filtered.sort((a, b) => {
        const featuredDiff = Number(Boolean(b.featured)) - Number(Boolean(a.featured));
        if (featuredDiff !== 0) return featuredDiff;
        return b.salesCount - a.salesCount;
      });
  }
}

