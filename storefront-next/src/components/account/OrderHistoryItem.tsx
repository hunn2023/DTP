"use client";

import { useState } from "react";
import Icon from "@/components/ui/Icon";
import OrderDetailModal from "@/components/account/OrderDetailModal";
import type { OrderHistoryItem as OrderHistoryItemType, OrderStatus, OrderPaymentMethod } from "@/lib/orderApi";
import type { Language } from "@/lib/i18n";

const STATUS_CLASSNAMES: Record<OrderStatus, string> = {
  pending: "bg-yellow-50 text-yellow-700 border-yellow-200",
  confirmed: "bg-blue-50 text-blue-700 border-blue-200",
  processing: "bg-indigo-50 text-indigo-700 border-indigo-200",
  shipped: "bg-purple-50 text-purple-700 border-purple-200",
  delivered: "bg-green-50 text-green-700 border-green-200",
  cancelled: "bg-red-50 text-red-700 border-red-200",
  refunded: "bg-orange-50 text-orange-700 border-orange-200",
};

function formatDate(iso: string, language: Language): string {
  return new Date(iso).toLocaleDateString(language === "vi" ? "vi-VN" : "en-US", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}

function formatCurrency(amount: number): string {
  return amount.toLocaleString("vi-VN") + "đ";
}

interface Props {
  order: OrderHistoryItemType;
  language?: Language;
}

export default function OrderHistoryItem({ order, language = "vi" }: Props) {
  const [showDetail, setShowDetail] = useState(false);

  const statusLabels: Record<OrderStatus, string> = {
    pending: language === "vi" ? "Chờ xác nhận" : "Pending confirmation",
    confirmed: language === "vi" ? "Đã xác nhận" : "Confirmed",
    processing: language === "vi" ? "Đang xử lý" : "Processing",
    shipped: language === "vi" ? "Đang giao" : "Shipping",
    delivered: language === "vi" ? "Đã giao" : "Delivered",
    cancelled: language === "vi" ? "Đã hủy" : "Cancelled",
    refunded: language === "vi" ? "Hoàn tiền" : "Refunded",
  };

  const paymentLabels: Record<OrderPaymentMethod, string> = {
    cod: "COD",
    banking: language === "vi" ? "Chuyển khoản" : "Bank transfer",
    momo: "MoMo",
    vnpay: "VNPay",
  };

  const text = {
    extraItems: language === "vi" ? "sản phẩm khác" : "more items",
    viewDetails: language === "vi" ? "Xem chi tiết" : "View details",
    noItems: language === "vi" ? "Đơn hàng" : "Order",
  };

  const firstItem = order.items?.[0];
  const extraCount = (order.items?.length || 0) - 1;

  return (
    <>
      <article
        role="button"
        tabIndex={0}
        onClick={() => setShowDetail(true)}
        onKeyDown={(e) => {
          if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            setShowDetail(true);
          }
        }}
        className="bg-white rounded-2xl border border-gray-100 shadow-[0_2px_12px_rgba(0,0,0,0.05)] overflow-hidden hover:shadow-[0_4px_20px_rgba(0,0,0,0.08)] hover:border-primary/30 transition-all cursor-pointer focus:outline-none focus-visible:ring-2 focus-visible:ring-primary/40"
      >
        {/* Header row */}
        <div className="flex items-center justify-between px-5 py-4 border-b border-gray-50 gap-3 flex-wrap">
          <div className="flex items-center gap-3 min-w-0">
            <span className="text-xs font-bold text-primary bg-primary/8 px-2.5 py-1 rounded-full shrink-0">
              #{order.orderCode}
            </span>
            <span className="text-xs text-gray-400 flex items-center gap-1 shrink-0">
              <Icon icon="clock" className="text-[10px]" />
              {formatDate(order.createdAt, language)}
            </span>
          </div>
          <span
            className={`text-[11px] font-semibold border px-2.5 py-1 rounded-full whitespace-nowrap ${STATUS_CLASSNAMES[order.status]}`}
          >
            {statusLabels[order.status]}
          </span>
        </div>

        {/* Items preview */}
        <div className="px-5 py-3.5 border-b border-gray-50">
          {firstItem ? (
            <>
              <p className="text-sm text-navy font-medium line-clamp-1">
                {firstItem.name}
                {firstItem.quantity > 1 && (
                  <span className="text-gray-400 font-normal ml-1">x{firstItem.quantity}</span>
                )}
              </p>
              {extraCount > 0 && (
                <p className="text-xs text-gray-400 mt-0.5">
                  + {extraCount} {text.extraItems}
                </p>
              )}
            </>
          ) : (
            <p className="text-sm text-gray-400">{text.noItems} #{order.orderCode}</p>
          )}
        </div>

        {/* Footer row */}
        <div className="flex items-center justify-between px-5 py-3.5 gap-3 flex-wrap">
          <div className="flex items-center gap-4 text-xs text-gray-500">
            <span className="flex items-center gap-1">
              <Icon icon="credit-card" className="text-gray-400 text-[10px]" />
              {paymentLabels[order.paymentMethod]}
            </span>
            <span className="text-navy font-bold text-sm">
              {formatCurrency(order.totalAmount)}
            </span>
          </div>
          <span className="inline-flex items-center gap-1.5 text-xs font-semibold text-primary shrink-0">
            {text.viewDetails}
            <Icon icon="chevron-right" className="text-[10px]" />
          </span>
        </div>
      </article>

      {showDetail && (
        <OrderDetailModal
          orderId={order.id}
          initialOrder={order}
          language={language}
          onClose={() => setShowDetail(false)}
        />
      )}
    </>
  );
}
