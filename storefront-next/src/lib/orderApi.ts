import { CheckoutFormData } from "./schemas/checkoutSchema";
import { fetchWithAuth } from "./fetchWithAuth";
import { useAuthStore } from "./authStore";
import { getCatalogThumbnails } from "./api/esimApi";
import type { ApiCreateOrderCommand, ApiCreateOrderItem, ApiCreatePaymentQrRequest, OrderItemType } from "@/types/api";

export interface OrderItem {
  id: string;
  name: string;
  price: number;
  quantity: number;
  image?: string;
  // For API mapping
  itemType?: OrderItemType | number;
  productId?: string;
  productVariantId?: string;
  esimPackageId?: string;
  phoneCardId?: string;
}

export interface CreateOrderPayload {
  customer: {
    fullName: string;
    phone: string;
    email?: string;
  };
  items: OrderItem[];
  paymentMethod: string;
  orderNote?: string;
  totalAmount: number;
}

export interface CreateOrderResponse {
  success: boolean;
  orderId?: string;
  message?: string;
}

export class OrderApiError extends Error {
  statusCode: number;

  constructor(message: string, statusCode: number) {
    super(message);
    this.name = "OrderApiError";
    this.statusCode = statusCode;
  }
}

function getApiErrorMessage(json: unknown, fallback: string): string {
  if (typeof json !== "object" || json === null) return fallback;

  const payload = json as { error?: string | null; message?: string | null };
  if (typeof payload.error === "string" && payload.error.trim()) return payload.error;
  if (typeof payload.message === "string" && payload.message.trim()) return payload.message;

  return fallback;
}

function isApiFailure(json: unknown): boolean {
  return typeof json === "object" && json !== null && "isSuccess" in json && json.isSuccess === false;
}

export async function createOrder(
  payload: CreateOrderPayload
): Promise<CreateOrderResponse> {
  // Map to backend API schema (CreateOrderCommand)
  const apiItems: ApiCreateOrderItem[] = payload.items.map((item) => ({
    itemType: item.itemType ?? 1, // default EsimPackage
    productId: item.productId || item.id,
    productVariantId: item.productVariantId || null,
    esimPackageId: item.esimPackageId || item.id,
    phoneCardId: item.phoneCardId || null,
    productName: item.name,
    variantName: null,
    sku: null,
    quantity: item.quantity,
    unitPrice: item.price,
  }));

  const apiPayload: ApiCreateOrderCommand = {
    customerId: useAuthStore.getState().user?.id,
    customerEmail: payload.customer.email || undefined,
    customerPhone: payload.customer.phone,
    customerName: payload.customer.fullName,
    currency: "VND",
    note: payload.orderNote || undefined,
    items: apiItems,
  };

  try {
    const response = await fetchWithAuth("/api/orders", {
      method: "POST",
      body: JSON.stringify(apiPayload),
    });

    const json = await response.json().catch(() => ({}));

    if (!response.ok || isApiFailure(json)) {
      throw new OrderApiError(
        getApiErrorMessage(json, `Không thể tạo đơn hàng. HTTP ${response.status}`),
        response.status || 400
      );
    }

    const data = json.data ?? json;
    const orderId = typeof data === "string" ? data : (data.id || data.orderId);

    if (!orderId || orderId === "00000000-0000-0000-0000-000000000000") {
      throw new OrderApiError(
        getApiErrorMessage(json, "Không thể tạo đơn hàng."),
        response.status || 400
      );
    }

    return {
      success: true,
      orderId,
      message: "Đặt hàng thành công!",
    };
  } catch (error) {
    if (error instanceof OrderApiError) throw error;
    if (error instanceof Error) {
      if (error.name === "AbortError") {
        throw new OrderApiError("Yêu cầu bị timeout", 408);
      }
      throw new OrderApiError(error.message, 500);
    }
    throw new OrderApiError("Lỗi không xác định", 500);
  }
}

export function mapFormDataToPayload(
  formData: CheckoutFormData,
  items: OrderItem[],
  totalAmount: number
): CreateOrderPayload {
  return {
    customer: {
      fullName: formData.fullName,
      phone: formData.phone,
      email: formData.email || undefined,
    },
    items,
    paymentMethod: formData.paymentMethod,
    orderNote: formData.orderNote || undefined,
    totalAmount,
  };
}

// ─── Payment QR ───────────────────────────────────────────────────────────────

export interface PaymentQrData {
  paymentTransactionId?: string;
  orderId?: string;
  orderCode?: string;
  amount?: number;
  currency?: string;
  status?: string;
  providerTransactionId?: string;
  qrCode?: string;
  qrImageUrl?: string;
  paymentUrl?: string;
  bankCode?: string;
  bankAccountNo?: string;
  bankAccountName?: string;
  transferContent?: string;
  expiredAt?: string;
  expiresAt?: string;
  qrCodeUrl?: string;
  qrDataUrl?: string;
  qrUrl?: string;
  bankName?: string;
  accountNumber?: string;
  accountName?: string;
  content?: string;
  description?: string;
  transactionId?: string;
}

export interface PaymentStatusData {
  orderId?: string;
  status?: string | number;
  paymentStatus?: string | number;
  amount?: number;
  paidAt?: string;
  transactionId?: string;
}

export async function confirmOrder(orderId: string, paymentMethod: string) {
  const response = await fetchWithAuth(`/api/orders/${orderId}/confirm`, {
    method: "POST",
    body: JSON.stringify({ paymentMethod }),
  });
  const json = await response.json().catch(() => ({}));

  if (!response.ok || isApiFailure(json)) {
    throw new OrderApiError(
      getApiErrorMessage(json, `Không thể xác nhận đơn hàng. HTTP ${response.status}`),
      response.status || 400
    );
  }

  return json.data ?? json;
}

export async function createPaymentQr(
  orderId: string,
  paymentProviderCode?: string
): Promise<PaymentQrData> {
  const payload: ApiCreatePaymentQrRequest = { orderId };
  if (paymentProviderCode) {
    payload.paymentProviderCode = paymentProviderCode;
  }

  const response = await fetchWithAuth("/api/payments/qr", {
    method: "POST",
    body: JSON.stringify(payload),
  });
  const json = await response.json().catch(() => ({}));

  if (!response.ok || isApiFailure(json)) {
    throw new OrderApiError(
      getApiErrorMessage(json, "Không thể tạo mã QR thanh toán."),
      response.status || 400
    );
  }

  const data = json.data ?? json;
  return typeof data === "string" ? { qrCodeUrl: data } : data;
}

export async function getPaymentStatus(orderId: string): Promise<PaymentStatusData> {
  const response = await fetchWithAuth(`/api/payments/orders/${orderId}`, {
    method: "GET",
  });
  if (!response.ok) {
    throw new OrderApiError("Không thể kiểm tra trạng thái thanh toán.", response.status);
  }
  const json = await response.json();
  return json.data ?? json;
}

const PAID_STATUS_VALUES = new Set(["paid", "success", "completed", "2"]);

function isPaidStatusValue(value: string | number | undefined): boolean {
  if (value == null) return false;
  if (value === 2 || value === "2") return true;
  if (typeof value === "string") return PAID_STATUS_VALUES.has(value.toLowerCase());
  return false;
}

export function isPaymentPaid(data: PaymentStatusData): boolean {
  if (data.paidAt) return true;
  return isPaidStatusValue(data.paymentStatus) || isPaidStatusValue(data.status);
}

// ─── Order Detail ─────────────────────────────────────────────────────────────

export async function getOrderDetail(orderId: string) {
  const response = await fetchWithAuth(`/api/orders/${orderId}`, {
    method: "GET",
  });
  if (!response.ok) {
    throw new OrderApiError("Không tìm thấy đơn hàng.", response.status);
  }
  const json = await response.json();
  return json.data ?? json;
}

// ─── Order History ────────────────────────────────────────────────────────────

export type OrderStatus =
  | "pending"
  | "confirmed"
  | "processing"
  | "shipped"
  | "delivered"
  | "cancelled"
  | "refunded";

export type OrderPaymentMethod = "cod" | "banking" | "momo" | "vnpay";

export interface OrderHistoryLineItem {
  id: string;
  name: string;
  quantity: number;
  price: number;
  image?: string;
  variantName?: string;
  sku?: string;
  itemType?: number;
  productId?: string;
  productVariantId?: string;
}

export interface OrderHistoryItem {
  id: string;
  orderCode: string;
  createdAt: string;
  status: OrderStatus;
  paymentStatus?: string;
  paymentStatusCode?: number;
  paymentMethod: OrderPaymentMethod;
  totalAmount: number;
  subTotal?: number;
  discountAmount?: number;
  shippingFee?: number;
  currency?: string;
  note?: string;
  customerName?: string;
  customerEmail?: string;
  customerPhone?: string;
  paidAt?: string;
  items: OrderHistoryLineItem[];
}

export interface OrderHistoryResponse {
  orders: OrderHistoryItem[];
  total: number;
  page: number;
  pageSize: number;
}

const PAGE_SIZE = 10;

// Map backend integer status to string (enum starts from 1)
function mapOrderStatus(status: number | string): OrderStatus {
  if (typeof status === "string") {
    const lower = status.toLowerCase();
    if (lower === "pending" || lower === "new") return "pending";
    if (lower === "confirmed") return "confirmed";
    if (lower === "processing") return "processing";
    if (lower === "shipped") return "shipped";
    if (lower === "delivered") return "delivered";
    if (lower === "cancelled") return "cancelled";
    if (lower === "refunded") return "refunded";
    return "pending";
  }
  const map: Record<number, OrderStatus> = {
    1: "pending",
    2: "confirmed",
    3: "processing",
    4: "shipped",
    5: "delivered",
    6: "cancelled",
    7: "refunded",
  };
  return map[status] ?? "pending";
}

function mapPaymentMethod(method: number | string | null | undefined): OrderPaymentMethod {
  if (typeof method === "string") {
    const lower = method.toLowerCase();
    if (lower === "cod") return "cod";
    if (lower === "momo") return "momo";
    if (lower === "vnpay") return "vnpay";
    if (lower === "banking" || lower === "bankqr" || lower === "sepay" || lower === "vnpt_epay") {
      return "banking";
    }
    return "banking";
  }
  // Integer enum: 0=COD, 1=Banking, 2=Momo, 3=VNPay
  if (method === 0) return "cod";
  if (method === 2) return "momo";
  if (method === 3) return "vnpay";
  return "banking";
}

/* eslint-disable */
function mapApiOrderToHistoryItem(apiOrder: any): OrderHistoryItem {
  const items: OrderHistoryLineItem[] = (apiOrder.items || apiOrder.orderItems || []).map(
    (item: any) => ({
      id: item.id || item.productId || "",
      name: item.productName || item.name || "Sản phẩm",
      quantity: item.quantity || 1,
      price: item.unitPrice || item.price || 0,
      image: item.image || item.imageUrl || item.thumbnailUrl || undefined,
      variantName: item.variantName || undefined,
      sku: item.sku || undefined,
      itemType: typeof item.itemType === "number" ? item.itemType : undefined,
      productId: item.productId || undefined,
      productVariantId: item.productVariantId || undefined,
    })
  );

  return {
    id: apiOrder.id,
    orderCode: apiOrder.orderCode || apiOrder.id?.substring(0, 8).toUpperCase() || "",
    createdAt: apiOrder.createdAt || new Date().toISOString(),
    status: mapOrderStatus(apiOrder.status),
    paymentStatus: apiOrder.paymentStatus != null ? String(apiOrder.paymentStatus) : undefined,
    paymentStatusCode: typeof apiOrder.paymentStatus === "number" ? apiOrder.paymentStatus : undefined,
    paymentMethod: mapPaymentMethod(apiOrder.paymentMethod),
    totalAmount: apiOrder.totalAmount || 0,
    subTotal: apiOrder.subTotal ?? apiOrder.subtotal,
    discountAmount: apiOrder.discountAmount,
    shippingFee: apiOrder.shippingFee,
    currency: apiOrder.currency || "VND",
    note: apiOrder.note || apiOrder.orderNote || undefined,
    customerName: apiOrder.customerName,
    customerEmail: apiOrder.customerEmail,
    customerPhone: apiOrder.customerPhone,
    paidAt: apiOrder.paidAt,
    items,
  };
}
/* eslint-enable */

export interface OrderFilters {
  customerId?: string;
  keyword?: string;
  status?: number;
  paymentStatus?: number;
}

export async function getMyOrders(page: number = 1, filters: OrderFilters = {}): Promise<OrderHistoryResponse> {
  const params = new URLSearchParams({
    pageIndex: page.toString(),
    pageSize: PAGE_SIZE.toString(),
  });
  if (filters.customerId) params.set("customerId", filters.customerId);
  if (filters.keyword) params.set("keyword", filters.keyword);
  if (filters.status) params.set("status", filters.status.toString());
  if (filters.paymentStatus) params.set("paymentStatus", filters.paymentStatus.toString());

  const response = await fetchWithAuth(`/api/orders/paged?${params.toString()}`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new OrderApiError("Không thể tải danh sách đơn hàng.", response.status);
  }

  const json = await response.json();
  const data = json.data ?? json;

  // Handle paged response: { items: [], totalCount, pageIndex, pageSize }
  // or possibly an array directly
  if (Array.isArray(data)) {
    return {
      orders: data.map(mapApiOrderToHistoryItem),
      total: data.length,
      page,
      pageSize: PAGE_SIZE,
    };
  }

  const items = data.items || data.orders || [];
  const total = data.totalCount ?? data.total ?? items.length;

  return {
    orders: items.map(mapApiOrderToHistoryItem),
    total,
    page,
    pageSize: data.pageSize || PAGE_SIZE,
  };
}

export async function getOrderById(id: string): Promise<OrderHistoryItem> {
  const response = await fetchWithAuth(`/api/orders/${id}`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new OrderApiError("Không tìm thấy đơn hàng.", response.status);
  }

  const json = await response.json();
  const data = json.data ?? json;
  return enrichOrderImages(mapApiOrderToHistoryItem(data));
}

/**
 * Order items returned by the backend don't carry a thumbnail. Resolve the
 * correct eSIM/product image from the public catalog by productVariantId
 * (falling back to productId) so the order detail view shows real images.
 */
async function enrichOrderImages(order: OrderHistoryItem): Promise<OrderHistoryItem> {
  if (!order.items.some((item) => !item.image)) return order;
  try {
    const { byVariant, byProduct } = await getCatalogThumbnails();
    return {
      ...order,
      items: order.items.map((item) => {
        if (item.image) return item;
        const resolved =
          (item.productVariantId && byVariant.get(item.productVariantId)) ||
          (item.productId && byProduct.get(item.productId)) ||
          undefined;
        return resolved ? { ...item, image: resolved } : item;
      }),
    };
  } catch {
    return order;
  }
}

