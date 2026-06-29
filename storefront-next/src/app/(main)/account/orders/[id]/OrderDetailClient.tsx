"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import Icon from "@/components/ui/Icon";
import { useAuth } from "@/hooks/useAuth";
import { useLanguage } from "@/hooks/useLanguage";
import { getOrderById, OrderHistoryItem, OrderPaymentMethod } from "@/lib/orderApi";
import AccountSkeleton from "@/components/account/AccountSkeleton";
import {
  getOrderStatusClassName,
  getOrderStatusLabel,
  getPaymentStatusClassName,
  getPaymentStatusLabel,
} from "@/lib/orderStatusDisplay";

function formatDate(iso: string, language: string): string {
  return new Date(iso).toLocaleDateString(language === "vi" ? "vi-VN" : "en-US", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function formatCurrency(amount: number): string {
  return amount.toLocaleString("vi-VN") + "đ";
}

export default function OrderDetailClient() {
  const params = useParams();
  const router = useRouter();
  const { initialized, isAuthenticated } = useAuth();
  const { language } = useLanguage();
  const [order, setOrder] = useState<OrderHistoryItem | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const orderId = params?.id as string;

  useEffect(() => {
    if (initialized && !isAuthenticated) {
      router.replace(`/login?returnUrl=/account/orders/${orderId}`);
    }
  }, [initialized, isAuthenticated, router, orderId]);

  useEffect(() => {
    if (!orderId || !isAuthenticated) return;

    setLoading(true);
    setError(null);
    getOrderById(orderId)
      .then(setOrder)
      .catch(() => {
        setError(
          language === "vi"
            ? "Không thể tải thông tin đơn hàng."
            : "Unable to load order details."
        );
      })
      .finally(() => setLoading(false));
  }, [orderId, isAuthenticated, language]);

  if (!initialized || !isAuthenticated) {
    return <AccountSkeleton />;
  }

  const paymentLabels: Record<OrderPaymentMethod, string> = {
    cod: "COD",
    banking: language === "vi" ? "Chuyển khoản" : "Bank transfer",
    momo: "MoMo",
    vnpay: "VNPay",
  };

  const text = {
    back: language === "vi" ? "Quay lại đơn hàng" : "Back to orders",
    title: language === "vi" ? "Chi tiết đơn hàng" : "Order details",
    orderCode: language === "vi" ? "Mã đơn" : "Order code",
    createdAt: language === "vi" ? "Ngày đặt" : "Order date",
    status: language === "vi" ? "Trạng thái" : "Status",
    paymentMethod: language === "vi" ? "Thanh toán" : "Payment",
    paymentStatus: language === "vi" ? "Trạng thái thanh toán" : "Payment status",
    products: language === "vi" ? "Sản phẩm" : "Products",
    quantity: language === "vi" ? "SL" : "Qty",
    price: language === "vi" ? "Đơn giá" : "Price",
    subtotal: language === "vi" ? "Thành tiền" : "Subtotal",
    total: language === "vi" ? "Tổng cộng" : "Total",
    retry: language === "vi" ? "Thử lại" : "Retry",
    notFound: language === "vi" ? "Không tìm thấy đơn hàng" : "Order not found",
  };

  if (loading) {
    return (
      <div className="max-w-3xl mx-auto px-4 py-10">
        <div className="animate-pulse space-y-4">
          <div className="h-6 w-40 bg-gray-200 rounded" />
          <div className="h-48 bg-gray-100 rounded-xl" />
          <div className="h-32 bg-gray-100 rounded-xl" />
        </div>
      </div>
    );
  }

  if (error || !order) {
    return (
      <div className="max-w-3xl mx-auto px-4 py-10 text-center">
        <div className="bg-white rounded-2xl border border-red-100 px-6 py-12">
          <div className="w-12 h-12 rounded-full bg-red-50 flex items-center justify-center mx-auto mb-3">
            <Icon icon="times" className="text-danger text-lg" />
          </div>
          <p className="text-sm text-gray-600 mb-4">{error || text.notFound}</p>
          <div className="flex items-center justify-center gap-3">
            <button
              onClick={() => {
                setLoading(true);
                setError(null);
                getOrderById(orderId)
                  .then(setOrder)
                  .catch(() => setError(error))
                  .finally(() => setLoading(false));
              }}
              className="btn btn-outline btn-sm"
            >
              {text.retry}
            </button>
            <Link href="/account/orders" className="btn btn-primary btn-sm">
              {text.back}
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="relative bg-gradient-to-br from-slate-50 via-blue-50/30 to-cyan-50/20 px-4 py-10 md:py-14 overflow-hidden">
      <div className="relative max-w-3xl mx-auto">
        {/* Back link */}
        <Link
          href="/account/orders"
          className="inline-flex items-center gap-1.5 text-sm text-gray-500 hover:text-primary transition mb-6"
        >
          <Icon icon="chevron-left" className="text-xs" />
          {text.back}
        </Link>

        {/* Order card */}
        <div className="bg-white rounded-2xl shadow-[0_4px_24px_rgba(0,0,0,0.06)] border border-gray-100 overflow-hidden">
          {/* Header */}
          <div className="px-6 py-5 border-b border-gray-100">
            <div className="flex items-center justify-between flex-wrap gap-3">
              <div>
                <h1 className="text-lg font-bold text-navy">{text.title}</h1>
                <p className="text-xs text-gray-400 mt-0.5">
                  #{order.orderCode}
                </p>
              </div>
              <span
                className={`text-xs font-semibold border px-3 py-1.5 rounded-full ${getOrderStatusClassName(order.status)}`}
              >
                {getOrderStatusLabel(order.status, language)}
              </span>
            </div>
          </div>

          {/* Info grid */}
          <div className="grid grid-cols-1 sm:grid-cols-4 gap-4 px-6 py-5 border-b border-gray-100 text-sm">
            <div>
              <p className="text-xs text-gray-400 mb-1">{text.createdAt}</p>
              <p className="font-medium text-navy">{formatDate(order.createdAt, language)}</p>
            </div>
            <div>
              <p className="text-xs text-gray-400 mb-1">{text.paymentMethod}</p>
              <p className="font-medium text-navy">{paymentLabels[order.paymentMethod]}</p>
            </div>
            {order.paymentStatusCode != null && (
              <div>
                <p className="text-xs text-gray-400 mb-1">{text.paymentStatus}</p>
                <span
                  className={`inline-flex text-[11px] font-semibold border px-2 py-0.5 rounded-full ${getPaymentStatusClassName(order.paymentStatusCode)}`}
                >
                  {getPaymentStatusLabel(order.paymentStatusCode, language)}
                </span>
              </div>
            )}
            <div>
              <p className="text-xs text-gray-400 mb-1">{text.total}</p>
              <p className="font-bold text-primary text-base">{formatCurrency(order.totalAmount)}</p>
            </div>
          </div>

          {/* Items table */}
          <div className="px-6 py-5">
            <h3 className="font-semibold text-navy text-sm mb-4">{text.products}</h3>
            <div className="space-y-3">
              {order.items.map((item) => (
                <div
                  key={item.id}
                  className="flex items-center justify-between gap-3 py-3 border-b border-gray-50 last:border-0"
                >
                  <div className="flex items-center gap-3 min-w-0">
                    {item.image && (
                      // eslint-disable-next-line @next/next/no-img-element
                      <img
                        src={item.image}
                        alt={item.name}
                        className="w-10 h-10 rounded-lg object-cover border border-gray-100 flex-shrink-0"
                      />
                    )}
                    <div className="min-w-0">
                      <p className="text-sm font-medium text-navy truncate">{item.name}</p>
                      <p className="text-xs text-gray-400">
                        {text.quantity}: {item.quantity} × {formatCurrency(item.price)}
                      </p>
                    </div>
                  </div>
                  <p className="text-sm font-semibold text-navy whitespace-nowrap">
                    {formatCurrency(item.price * item.quantity)}
                  </p>
                </div>
              ))}
            </div>

            {/* Total */}
            <div className="flex items-center justify-between mt-5 pt-4 border-t border-gray-100">
              <span className="font-semibold text-navy">{text.total}</span>
              <span className="text-xl font-bold text-primary">{formatCurrency(order.totalAmount)}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
