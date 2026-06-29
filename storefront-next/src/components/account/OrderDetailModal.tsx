"use client";

import { useEffect, useState } from "react";
import { createPortal } from "react-dom";
import type { IconProp } from "@fortawesome/fontawesome-svg-core";
import Icon from "@/components/ui/Icon";
import {
  getOrderById,
  type OrderHistoryItem as OrderHistoryItemType,
  type OrderStatus,
  type OrderPaymentMethod,
} from "@/lib/orderApi";
import type { Language } from "@/lib/i18n";

// ── Style maps ────────────────────────────────────────────────────────────────

const STATUS_CLASSNAMES: Record<OrderStatus, string> = {
  pending: "bg-yellow-50 text-yellow-700 border-yellow-200",
  confirmed: "bg-blue-50 text-blue-700 border-blue-200",
  processing: "bg-indigo-50 text-indigo-700 border-indigo-200",
  shipped: "bg-purple-50 text-purple-700 border-purple-200",
  delivered: "bg-green-50 text-green-700 border-green-200",
  cancelled: "bg-red-50 text-red-700 border-red-200",
  refunded: "bg-orange-50 text-orange-700 border-orange-200",
};

const PAYMENT_STATUS_CLASSNAMES: Record<number, string> = {
  1: "bg-amber-50 text-amber-700 border-amber-200",
  2: "bg-green-50 text-green-700 border-green-200",
  3: "bg-red-50 text-red-700 border-red-200",
  4: "bg-orange-50 text-orange-700 border-orange-200",
  5: "bg-gray-100 text-gray-600 border-gray-200",
};

// ── Helpers ───────────────────────────────────────────────────────────────────

function formatDateTime(iso: string, language: Language): string {
  return new Date(iso).toLocaleString(language === "vi" ? "vi-VN" : "en-US", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function formatCurrency(amount: number, currency = "VND"): string {
  if (currency === "USD") return "$" + amount.toLocaleString("en-US");
  return amount.toLocaleString("vi-VN") + "đ";
}

// ── Component ─────────────────────────────────────────────────────────────────

interface Props {
  orderId: string;
  initialOrder?: OrderHistoryItemType;
  language?: Language;
  onClose: () => void;
}

export default function OrderDetailModal({ orderId, initialOrder, language = "vi", onClose }: Props) {
  const [order, setOrder] = useState<OrderHistoryItemType | null>(initialOrder ?? null);
  const [loading, setLoading] = useState(!initialOrder);
  const [error, setError] = useState<string | null>(null);
  const [mounted, setMounted] = useState(false);

  // Enter animation + portal target
  useEffect(() => {
    setMounted(true);
  }, []);

  // Lock body scroll + close on ESC
  useEffect(() => {
    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    document.addEventListener("keydown", onKey);
    return () => {
      document.body.style.overflow = previousOverflow;
      document.removeEventListener("keydown", onKey);
    };
  }, [onClose]);

  // Always fetch the freshest, fullest detail
  useEffect(() => {
    let active = true;
    setLoading(!initialOrder);
    setError(null);
    getOrderById(orderId)
      .then((data) => {
        if (active) setOrder(data);
      })
      .catch(() => {
        if (active && !initialOrder) {
          setError(
            language === "vi" ? "Không thể tải thông tin đơn hàng." : "Unable to load order details."
          );
        }
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, [orderId, initialOrder, language]);

  const statusLabels: Record<OrderStatus, string> = {
    pending: language === "vi" ? "Chờ xác nhận" : "Pending",
    confirmed: language === "vi" ? "Đã xác nhận" : "Confirmed",
    processing: language === "vi" ? "Đang xử lý" : "Processing",
    shipped: language === "vi" ? "Đang giao" : "Shipping",
    delivered: language === "vi" ? "Đã giao" : "Delivered",
    cancelled: language === "vi" ? "Đã hủy" : "Cancelled",
    refunded: language === "vi" ? "Hoàn tiền" : "Refunded",
  };

  const paymentMethodLabels: Record<OrderPaymentMethod, string> = {
    cod: "COD",
    banking: language === "vi" ? "Chuyển khoản" : "Bank transfer",
    momo: "MoMo",
    vnpay: "VNPay",
  };

  const paymentStatusLabels: Record<number, string> = {
    1: language === "vi" ? "Chờ thanh toán" : "Pending",
    2: language === "vi" ? "Đã thanh toán" : "Paid",
    3: language === "vi" ? "Thanh toán thất bại" : "Failed",
    4: language === "vi" ? "Đã hoàn tiền" : "Refunded",
    5: language === "vi" ? "Đã hủy" : "Cancelled",
  };

  const itemTypeLabels: Record<number, string> = {
    1: "eSIM",
    2: language === "vi" ? "Thẻ nạp" : "Phone card",
  };

  const t = {
    title: language === "vi" ? "Chi tiết đơn hàng" : "Order details",
    close: language === "vi" ? "Đóng" : "Close",
    createdAt: language === "vi" ? "Ngày đặt" : "Order date",
    paidAt: language === "vi" ? "Ngày thanh toán" : "Paid at",
    paymentMethod: language === "vi" ? "Phương thức" : "Payment method",
    paymentStatus: language === "vi" ? "Thanh toán" : "Payment",
    customer: language === "vi" ? "Thông tin khách hàng" : "Customer information",
    name: language === "vi" ? "Họ tên" : "Full name",
    phone: language === "vi" ? "Số điện thoại" : "Phone",
    email: "Email",
    products: language === "vi" ? "Sản phẩm" : "Products",
    note: language === "vi" ? "Ghi chú" : "Note",
    subtotal: language === "vi" ? "Tạm tính" : "Subtotal",
    discount: language === "vi" ? "Giảm giá" : "Discount",
    shipping: language === "vi" ? "Phí vận chuyển" : "Shipping fee",
    total: language === "vi" ? "Tổng cộng" : "Total",
    retry: language === "vi" ? "Thử lại" : "Retry",
    quantity: language === "vi" ? "SL" : "Qty",
    notFound: language === "vi" ? "Không tìm thấy đơn hàng" : "Order not found",
  };

  const currency = order?.currency || "VND";
  const computedSubtotal =
    order?.subTotal ??
    (order ? order.items.reduce((sum, it) => sum + it.price * it.quantity, 0) : 0);

  if (!mounted) return null;

  return createPortal(
    <div
      className="fixed inset-0 z-[400] flex items-end sm:items-center justify-center px-3 pt-20 pb-4 sm:p-6"
      role="dialog"
      aria-modal="true"
      aria-label={t.title}
    >
      {/* Backdrop */}
      <button
        type="button"
        aria-label={t.close}
        onClick={onClose}
        className="absolute inset-0 bg-navy/40 backdrop-blur-sm animate-[fadeIn_0.2s_ease]"
      />

      {/* Panel */}
      <div
        className="relative w-full sm:max-w-2xl max-h-[calc(100dvh-6rem)] sm:max-h-[calc(100dvh-3rem)] flex flex-col bg-white rounded-3xl shadow-2xl overflow-hidden transition-all duration-300 ease-out"
        style={{
          transform: mounted ? "translateY(0)" : "translateY(24px)",
          opacity: mounted ? 1 : 0,
        }}
      >
        {/* Header */}
        <div className="relative gradient-primary px-6 py-5 text-white shrink-0">
          <div className="flex items-start justify-between gap-3">
            <div className="min-w-0">
              <p className="text-[11px] font-semibold uppercase tracking-widest text-white/70">
                {t.title}
              </p>
              <h2 className="mt-0.5 text-lg font-bold leading-tight truncate">
                #{order?.orderCode || "—"}
              </h2>
            </div>
            <button
              type="button"
              onClick={onClose}
              aria-label={t.close}
              className="shrink-0 w-9 h-9 rounded-full bg-white/15 hover:bg-white/25 flex items-center justify-center transition"
            >
              <Icon icon="times" className="text-sm" />
            </button>
          </div>

          {order && (
            <div className="mt-4 flex flex-wrap items-center gap-2">
              <span className="text-[11px] font-semibold bg-white/95 px-3 py-1 rounded-full border border-white/40 text-navy">
                {statusLabels[order.status]}
              </span>
              {order.paymentStatusCode != null && (
                <span className="text-[11px] font-semibold bg-white/15 border border-white/30 px-3 py-1 rounded-full text-white">
                  {t.paymentStatus}: {paymentStatusLabels[order.paymentStatusCode] ?? order.paymentStatus}
                </span>
              )}
            </div>
          )}
        </div>

        {/* Body */}
        <div className="modal-scroll flex-1 overflow-y-auto overscroll-contain px-6 py-5">
          {loading ? (
            <div className="animate-pulse space-y-4">
              <div className="h-20 bg-gray-100 rounded-xl" />
              <div className="h-24 bg-gray-100 rounded-xl" />
              <div className="h-32 bg-gray-100 rounded-xl" />
            </div>
          ) : error || !order ? (
            <div className="py-12 text-center">
              <div className="w-12 h-12 rounded-full bg-red-50 flex items-center justify-center mx-auto mb-3">
                <Icon icon="times" className="text-danger text-lg" />
              </div>
              <p className="text-sm text-gray-600 mb-4">{error || t.notFound}</p>
              <button
                onClick={() => {
                  setLoading(true);
                  setError(null);
                  getOrderById(orderId)
                    .then(setOrder)
                    .catch(() =>
                      setError(
                        language === "vi"
                          ? "Không thể tải thông tin đơn hàng."
                          : "Unable to load order details."
                      )
                    )
                    .finally(() => setLoading(false));
                }}
                className="btn btn-outline btn-sm"
              >
                {t.retry}
              </button>
            </div>
          ) : (
            <div className="space-y-5">
              {/* Meta grid */}
              <div className="grid grid-cols-2 gap-3">
                <InfoTile icon="clock" label={t.createdAt} value={formatDateTime(order.createdAt, language)} />
                <InfoTile icon="credit-card" label={t.paymentMethod} value={paymentMethodLabels[order.paymentMethod]} />
                {order.paidAt && (
                  <InfoTile icon="check-circle" label={t.paidAt} value={formatDateTime(order.paidAt, language)} />
                )}
                {order.paymentStatusCode != null && (
                  <div className="rounded-xl border border-gray-100 bg-gray-50/60 px-3.5 py-3">
                    <p className="text-[11px] text-gray-400 mb-1 flex items-center gap-1.5">
                      <Icon icon="wallet" className="text-[10px]" />
                      {t.paymentStatus}
                    </p>
                    <span
                      className={`inline-block text-[11px] font-semibold border px-2 py-0.5 rounded-full ${
                        PAYMENT_STATUS_CLASSNAMES[order.paymentStatusCode] ?? STATUS_CLASSNAMES[order.status]
                      }`}
                    >
                      {paymentStatusLabels[order.paymentStatusCode] ?? order.paymentStatus}
                    </span>
                  </div>
                )}
              </div>

              {/* Customer */}
              {(order.customerName || order.customerPhone || order.customerEmail) && (
                <section>
                  <SectionTitle icon="user">{t.customer}</SectionTitle>
                  <div className="rounded-xl border border-gray-100 divide-y divide-gray-50">
                    {order.customerName && <DetailRow label={t.name} value={order.customerName} />}
                    {order.customerPhone && <DetailRow label={t.phone} value={order.customerPhone} />}
                    {order.customerEmail && <DetailRow label={t.email} value={order.customerEmail} />}
                  </div>
                </section>
              )}

              {/* Products */}
              <section>
                <SectionTitle icon="shopping-cart">
                  {t.products}
                  <span className="ml-1 text-gray-400 font-normal">({order.items.length})</span>
                </SectionTitle>
                <div className="rounded-xl border border-gray-100 divide-y divide-gray-50">
                  {order.items.map((item) => (
                    <div key={item.id} className="flex items-center gap-3 px-3.5 py-3">
                      <div className="w-11 h-11 rounded-lg border border-gray-100 bg-gray-50 flex items-center justify-center overflow-hidden shrink-0">
                        {item.image ? (
                          // eslint-disable-next-line @next/next/no-img-element
                          <img src={item.image} alt={item.name} className="w-full h-full object-cover" />
                        ) : (
                          <Icon icon="sim-card" className="text-gray-300" />
                        )}
                      </div>
                      <div className="min-w-0 flex-1">
                        <div className="flex items-center gap-2">
                          <p className="text-sm font-medium text-navy truncate">{item.name}</p>
                          {item.itemType != null && itemTypeLabels[item.itemType] && (
                            <span className="shrink-0 text-[10px] font-semibold text-primary bg-primary/8 px-1.5 py-0.5 rounded">
                              {itemTypeLabels[item.itemType]}
                            </span>
                          )}
                        </div>
                        <p className="text-xs text-gray-400 mt-0.5">
                          {item.variantName ? `${item.variantName} · ` : ""}
                          {t.quantity}: {item.quantity} × {formatCurrency(item.price, currency)}
                        </p>
                      </div>
                      <p className="text-sm font-semibold text-navy whitespace-nowrap">
                        {formatCurrency(item.price * item.quantity, currency)}
                      </p>
                    </div>
                  ))}
                </div>
              </section>

              {/* Note */}
              {order.note && (
                <section>
                  <SectionTitle icon="pen">{t.note}</SectionTitle>
                  <p className="rounded-xl border border-gray-100 bg-amber-50/40 px-3.5 py-3 text-sm text-gray-600 whitespace-pre-line">
                    {order.note}
                  </p>
                </section>
              )}

              {/* Summary */}
              <section className="rounded-xl bg-slate-50 border border-gray-100 px-4 py-3.5 space-y-2">
                <SummaryRow label={t.subtotal} value={formatCurrency(computedSubtotal, currency)} />
                {order.discountAmount ? (
                  <SummaryRow
                    label={t.discount}
                    value={`- ${formatCurrency(order.discountAmount, currency)}`}
                    accent="text-green-600"
                  />
                ) : null}
                {order.shippingFee ? (
                  <SummaryRow label={t.shipping} value={formatCurrency(order.shippingFee, currency)} />
                ) : null}
                <div className="flex items-center justify-between pt-2 mt-1 border-t border-gray-200">
                  <span className="font-semibold text-navy">{t.total}</span>
                  <span className="text-xl font-bold text-primary">
                    {formatCurrency(order.totalAmount, currency)}
                  </span>
                </div>
              </section>
            </div>
          )}
        </div>
      </div>

      <style jsx>{`
        @keyframes fadeIn {
          from {
            opacity: 0;
          }
          to {
            opacity: 1;
          }
        }
        .modal-scroll {
          scroll-behavior: smooth;
          -webkit-overflow-scrolling: touch;
          scrollbar-width: thin;
          scrollbar-color: rgb(203 213 225) transparent;
        }
        .modal-scroll::-webkit-scrollbar {
          width: 8px;
        }
        .modal-scroll::-webkit-scrollbar-track {
          background: transparent;
          margin: 6px 0;
        }
        .modal-scroll::-webkit-scrollbar-thumb {
          background: rgb(203 213 225);
          border-radius: 9999px;
          border: 2px solid transparent;
          background-clip: content-box;
        }
        .modal-scroll::-webkit-scrollbar-thumb:hover {
          background: rgb(148 163 184);
          background-clip: content-box;
        }
      `}</style>
    </div>,
    document.body
  );
}

// ── Sub-components ──────────────────────────────────────────────────────────────

function SectionTitle({ icon, children }: { icon: IconProp; children: React.ReactNode }) {
  return (
    <h3 className="flex items-center gap-2 text-sm font-semibold text-navy mb-2">
      <Icon icon={icon} className="text-primary text-xs" />
      {children}
    </h3>
  );
}

function InfoTile({ icon, label, value }: { icon: IconProp; label: string; value: string }) {
  return (
    <div className="rounded-xl border border-gray-100 bg-gray-50/60 px-3.5 py-3">
      <p className="text-[11px] text-gray-400 mb-1 flex items-center gap-1.5">
        <Icon icon={icon} className="text-[10px]" />
        {label}
      </p>
      <p className="text-sm font-medium text-navy break-words">{value}</p>
    </div>
  );
}

function DetailRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between gap-3 px-3.5 py-2.5">
      <span className="text-xs text-gray-400 shrink-0">{label}</span>
      <span className="text-sm font-medium text-navy text-right break-all">{value}</span>
    </div>
  );
}

function SummaryRow({ label, value, accent }: { label: string; value: string; accent?: string }) {
  return (
    <div className="flex items-center justify-between text-sm">
      <span className="text-gray-500">{label}</span>
      <span className={`font-medium ${accent ?? "text-navy"}`}>{value}</span>
    </div>
  );
}
