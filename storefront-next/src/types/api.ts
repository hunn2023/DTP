// ─── API Response Wrapper ─────────────────────────────────────────────────────
export interface ApiResponse<T> {
  isSuccess: boolean;
  data: T;
  error: string | null;
}

// ─── Paginated Response Wrapper ───────────────────────────────────────────────
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
}

// ─── Auth ─────────────────────────────────────────────────────────────────────
export interface ApiLoginRequest {
  email: string;
  password: string;
}

export interface ApiLoginResponse {
  accessToken: string;
  refreshToken: string;
  user?: ApiUserProfile;
}

export interface ApiUserProfile {
  id: string;
  email: string;
  phone: string | null;
  fullName: string | null;
  avatarUrl: string | null;
}

export interface ApiRegisterRequest {
  email: string;
  phone: string;
  fullName: string;
  password: string;
}

export interface ApiVerifyOtpRequest {
  email: string;
  otpCode: string;
  ipAddress?: string;
  userAgent?: string;
}

export interface ApiResendOtpRequest {
  email: string;
}

export interface ApiRefreshTokenRequest {
  refreshToken: string;
}

// ─── Catalog: Country ─────────────────────────────────────────────────────────
export interface ApiCountry {
  id: string;
  code: string;
  name: string;
  slug: string;
  flagUrl: string | null;
  region: string | null;
  description: string | null;
  sortOrder: number;
  isActive: boolean;
}

/** Shape returned by /api/catalog/countries/home */
export interface ApiCountryHome {
  countryId: string;
  code: string;
  name: string;
  slug: string;
  flagUrl: string | null;
  region: string | null;
  priceFrom: number;
  currency: string;
  packageCount: number;
  isHot: boolean;
}

// ─── Catalog: Category ────────────────────────────────────────────────────────
export interface ApiCategory {
  id: string;
  name: string;
  code: string;
  slug: string;
  isActive: boolean;
  sortOrder: number;
}

// ─── Catalog: Carrier ─────────────────────────────────────────────────────────
export interface ApiCarrier {
  id: string;
  code: string;
  name: string;
  slug: string;
  countryId: string;
  logoUrl: string | null;
  sortOrder: number;
  isActive?: boolean;
}

// ─── Catalog: EsimPackage ─────────────────────────────────────────────────────
export interface ApiEsimPackage {
  id: string;
  productId: string;
  productName?: string | null;
  productVariantId: string;
  productVariantName?: string | null;
  providerId: string;
  providerName?: string | null;
  countryId: string;
  countryName?: string | null;
  name: string;
  slug: string;
  providerPackageCode: string | null;
  dataAmount: number | null;
  dataUnit: string | null;
  validityDays: number;
  isUnlimited: boolean;
  coverageType: string | null;
  coverageDescription: string | null;
  activationPolicy: string | null;
  speedPolicy: string | null;
  hotspotSupported: boolean;
  phoneNumberSupported: boolean;
  smsSupported: boolean;
  kycRequired: boolean;
  qrDeliveryType: string | null;
  sortOrder: number;
  isActive: boolean;
  // Price fields (expected from API response, not in Swagger command schemas)
  price?: number;
  salePrice?: number;
  originalPrice?: number;
  costPrice?: number;
  currency?: string;
  // Relations
  country?: ApiCountry;
  carriers?: ApiCarrier[];
  productVariantFeatures?: {
    productVariantId?: string;
    text: string;
    icon: string | null;
    sortOrder: number;
  }[];
}

// ─── Catalog: Product ─────────────────────────────────────────────────────────
export interface ApiProduct {
  id: string;
  code: string;
  name: string;
  slug: string;
  categoryId: string;
  countryId: string | null;
  shortDescription: string | null;
  description: string | null;
  locationText: string | null;
  thumbnailUrl: string | null;
  isFeatured: boolean;
  isHot: boolean;
  sortOrder: number;
  isActive: boolean;
  soldCount?: number;
  minPrice?: number | null;
  variantCount?: number;
  attributes?: {
    id: string;
    productId: string | null;
    key: string;
    value: string;
    sortOrder: number;
    isVisible: boolean | null;
  }[];
  // Relations
  category?: ApiCategory;
  country?: ApiCountry;
  // Price (from product-prices relation)
  price?: number;
  salePrice?: number;
  originalPrice?: number;
}

// ─── Catalog: Product Variant ─────────────────────────────────────────────────
export interface ApiProductVariant {
  id: string;
  productId: string;
  sku: string | null;
  name: string | null;
  shortName: string | null;
  description: string | null;
  sortOrder: number;
  isActive: boolean;
}

// ─── Catalog: PhoneCard ───────────────────────────────────────────────────────
export interface ApiPhoneCard {
  id: string;
  productVariantId: string;
  providerId: string;
  name: string;
  slug: string;
  faceValue: number;
  price: number;
  currency: string | null;
  sortOrder: number;
  isActive: boolean;
  // Relations
  provider?: ApiProvider;
}

// ─── Catalog: Provider ────────────────────────────────────────────────────────
export interface ApiProvider {
  id: string;
  code: string;
  name: string;
  logoUrl?: string | null;
  isActive?: boolean;
}

// ─── Orders ───────────────────────────────────────────────────────────────────
export enum OrderItemType {
  EsimPackage = 1,
  PhoneCard = 2,
}

export interface ApiCreateOrderItem {
  itemType: OrderItemType;
  productId: string;
  productVariantId?: string | null;
  esimPackageId?: string | null;
  phoneCardId?: string | null;
  productName: string;
  variantName?: string | null;
  sku?: string | null;
  quantity: number;
  unitPrice: number;
}

export interface ApiCreateOrderCommand {
  customerId?: string;
  customerEmail?: string;
  customerPhone?: string;
  customerName?: string;
  currency?: string;
  note?: string;
  items: ApiCreateOrderItem[];
}

export interface ApiOrderResponse {
  id: string;
  orderCode?: string;
  status: number;
  paymentStatus: number;
  totalAmount: number;
  currency: string;
  customerName: string | null;
  customerEmail: string | null;
  customerPhone: string | null;
  note: string | null;
  createdAt: string;
  items?: ApiCreateOrderItem[];
}

// ─── Payments ─────────────────────────────────────────────────────────────────
export interface ApiCreatePaymentQrRequest {
  orderId: string;
  paymentProviderCode?: string | null;
}

export interface ApiPaymentQrResponse {
  qrCodeUrl?: string;
  qrContent?: string;
  amount?: number;
  bankName?: string;
  accountNumber?: string;
  accountName?: string;
  orderId?: string;
  expiresAt?: string;
}

export interface ApiPaymentStatusResponse {
  orderId: string;
  status: string;
  amount: number;
  paidAt?: string;
  transactionId?: string;
}
