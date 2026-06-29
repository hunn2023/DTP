"use client";

import Image from "next/image";
import { formatPrice } from "@/lib/product";
import { CartItem } from "@/types/cart";
import type { Language } from "@/lib/i18n";

interface OrderReviewProps {
  items: CartItem[];
  shippingFee?: number;
  language?: Language;
}

export default function OrderReview({ items, shippingFee = 0, language = "vi" }: OrderReviewProps) {
  const subtotal = items.reduce((sum, item) => sum + item.price * item.quantity, 0);
  const total = subtotal + shippingFee;

  const text = {
    title: language === "vi" ? "Kiểm tra đơn hàng" : "Order review",
    subtotal: language === "vi" ? "Tạm tính" : "Subtotal",
    productCount: language === "vi" ? "sản phẩm" : "items",
    shipping: language === "vi" ? "Phí vận chuyển" : "Shipping fee",
    free: language === "vi" ? "Miễn phí" : "Free",
    total: language === "vi" ? "Tổng thanh toán" : "Total payment",
    commitmentTitle: language === "vi" ? "Cam kết:" : "Commitment:",
    commitmentBody:
      language === "vi"
        ? "Thông tin đơn hàng chính xác, hỗ trợ xử lý nhanh sau thanh toán, hoàn tiền nếu sản phẩm không đúng mô tả."
        : "Accurate order information, fast support after payment, and refund if products do not match the description.",
  };

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-semibold text-navy mb-4">{text.title}</h3>

      {/* Items List */}
      <div className="space-y-3">
        {items.map((item) => (
          <div
            key={item.id}
            className="flex items-center gap-4 p-3 rounded-lg bg-gray-50"
          >
            {/* Product Image */}
            <div className="flex-shrink-0 w-16 h-16 rounded-lg overflow-hidden bg-gray-200 relative">
              {item.image ? (
                <Image
                  src={item.image}
                  alt={item.name}
                  fill
                  className="object-cover"
                  sizes="64px"
                />
              ) : (
                <div className="w-full h-full flex items-center justify-center text-gray-400">
                  <svg className="w-8 h-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={1.5}
                      d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
                    />
                  </svg>
                </div>
              )}
            </div>

            {/* Product Info */}
            <div className="flex-1 min-w-0">
              <p className="font-medium text-navy truncate">{item.name}</p>
              <p className="text-sm text-gray-500 mt-1">
                {formatPrice(item.price)} × {item.quantity}
              </p>
            </div>

            {/* Subtotal */}
            <div className="flex-shrink-0 text-right">
              <p className="font-semibold text-navy">
                {formatPrice(item.price * item.quantity)}
              </p>
            </div>
          </div>
        ))}
      </div>

      {/* Summary */}
      <div className="border-t pt-4 space-y-2">
        <div className="flex justify-between text-sm">
          <span className="text-gray-500">
            {text.subtotal} ({items.reduce((sum, i) => sum + i.quantity, 0)} {text.productCount})
          </span>
          <span className="font-medium text-navy">{formatPrice(subtotal)}</span>
        </div>

        <div className="flex justify-between text-sm">
          <span className="text-gray-500">{text.shipping}</span>
          <span className={shippingFee === 0 ? "text-success font-medium" : "font-medium text-navy"}>
            {shippingFee === 0 ? text.free : formatPrice(shippingFee)}
          </span>
        </div>

        <div className="flex justify-between pt-2 border-t">
          <span className="font-semibold text-navy">{text.total}</span>
          <span className="text-xl font-bold text-primary">{formatPrice(total)}</span>
        </div>
      </div>

      {/* Notice */}
      <div className="p-3 rounded-lg bg-success-light border border-success text-sm text-success">
        <p>
          <strong>{text.commitmentTitle}</strong> {text.commitmentBody}
        </p>
      </div>
    </div>
  );
}