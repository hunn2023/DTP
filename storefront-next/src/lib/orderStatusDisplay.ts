import type { Language } from "@/lib/i18n";
import type { OrderStatus } from "@/lib/orderApi";

export function getOrderStatusLabel(status: OrderStatus, language: Language): string {
  switch (status) {
    case "draft":
      return language === "vi" ? "Nháp" : "Draft";
    case "pendingPayment":
      return language === "vi" ? "Chờ thanh toán" : "Pending payment";
    case "paid":
      return language === "vi" ? "Đã thanh toán" : "Paid";
    case "processing":
      return language === "vi" ? "Đang xử lý" : "Processing";
    case "completed":
      return language === "vi" ? "Hoàn tất" : "Completed";
    case "cancelled":
      return language === "vi" ? "Đã hủy" : "Cancelled";
    case "failed":
      return language === "vi" ? "Thất bại" : "Failed";
    case "fulfillmentFailed":
      return language === "vi" ? "Giao hàng thất bại" : "Fulfillment failed";
  }
}

export function getOrderStatusClassName(status: OrderStatus): string {
  switch (status) {
    case "draft":
      return "bg-gray-50 text-gray-600 border-gray-200";
    case "pendingPayment":
      return "bg-amber-50 text-amber-700 border-amber-200";
    case "paid":
      return "bg-emerald-50 text-emerald-700 border-emerald-200";
    case "processing":
      return "bg-indigo-50 text-indigo-700 border-indigo-200";
    case "completed":
      return "bg-green-50 text-green-700 border-green-200";
    case "cancelled":
      return "bg-slate-100 text-slate-600 border-slate-200";
    case "failed":
    case "fulfillmentFailed":
      return "bg-red-50 text-red-700 border-red-200";
  }
}

export function getPaymentStatusLabel(status: number, language: Language): string {
  switch (status) {
    case 0:
      return language === "vi" ? "Đang tạo QR" : "Creating QR";
    case 1:
      return language === "vi" ? "Chờ thanh toán" : "Pending";
    case 2:
      return language === "vi" ? "Đang xử lý" : "Processing";
    case 3:
      return language === "vi" ? "Đã thanh toán" : "Paid";
    case 4:
      return language === "vi" ? "Thanh toán thất bại" : "Failed";
    case 5:
      return language === "vi" ? "Đã hủy" : "Cancelled";
    case 6:
      return language === "vi" ? "Hết hạn" : "Expired";
    case 7:
      return language === "vi" ? "Đã hoàn tiền" : "Refunded";
    default:
      return language === "vi" ? "Không xác định" : "Unknown";
  }
}

export function getPaymentStatusClassName(status: number): string {
  switch (status) {
    case 0:
      return "bg-blue-50 text-blue-700 border-blue-200";
    case 1:
      return "bg-amber-50 text-amber-700 border-amber-200";
    case 2:
      return "bg-indigo-50 text-indigo-700 border-indigo-200";
    case 3:
      return "bg-green-50 text-green-700 border-green-200";
    case 4:
    case 6:
      return "bg-red-50 text-red-700 border-red-200";
    case 5:
      return "bg-slate-100 text-slate-600 border-slate-200";
    case 7:
      return "bg-orange-50 text-orange-700 border-orange-200";
    default:
      return "bg-gray-50 text-gray-600 border-gray-200";
  }
}

export function getOrderStatusOptions(language: Language): { value: number | undefined; label: string }[] {
  return [
    { value: undefined, label: language === "vi" ? "Tất cả trạng thái" : "All statuses" },
    { value: 1, label: getOrderStatusLabel("draft", language) },
    { value: 2, label: getOrderStatusLabel("pendingPayment", language) },
    { value: 3, label: getOrderStatusLabel("paid", language) },
    { value: 4, label: getOrderStatusLabel("processing", language) },
    { value: 5, label: getOrderStatusLabel("completed", language) },
    { value: 6, label: getOrderStatusLabel("cancelled", language) },
    { value: 7, label: getOrderStatusLabel("failed", language) },
    { value: 8, label: getOrderStatusLabel("fulfillmentFailed", language) },
  ];
}

export function getPaymentStatusOptions(language: Language): { value: number | undefined; label: string }[] {
  return [
    { value: undefined, label: language === "vi" ? "Tất cả TT thanh toán" : "All payment statuses" },
    { value: 0, label: getPaymentStatusLabel(0, language) },
    { value: 1, label: getPaymentStatusLabel(1, language) },
    { value: 2, label: getPaymentStatusLabel(2, language) },
    { value: 3, label: getPaymentStatusLabel(3, language) },
    { value: 4, label: getPaymentStatusLabel(4, language) },
    { value: 5, label: getPaymentStatusLabel(5, language) },
    { value: 6, label: getPaymentStatusLabel(6, language) },
    { value: 7, label: getPaymentStatusLabel(7, language) },
  ];
}
