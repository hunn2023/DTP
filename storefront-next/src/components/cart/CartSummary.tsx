"use client";

import Link from "next/link";
import { useCartStore } from "@/lib/cartStore";
import { formatPrice } from "@/lib/product";
import type { Language } from "@/lib/i18n";

interface CartSummaryProps {
  language?: Language;
}

export default function CartSummary({ language = "vi" }: CartSummaryProps) {
  const totalAmount = useCartStore((s) => s.getTotalAmount());
  const totalQuantity = useCartStore((s) => s.getTotalQuantity());

  const text = {
    title: language === "vi" ? "Tóm tắt đơn hàng" : "Order summary",
    subtotal: language === "vi" ? "Tạm tính" : "Subtotal",
    productCount: language === "vi" ? "sản phẩm" : "items",
    shipping: language === "vi" ? "Phí vận chuyển" : "Shipping fee",
    free: language === "vi" ? "Miễn phí" : "Free",
    total: language === "vi" ? "Tổng cộng" : "Total",
    checkout: language === "vi" ? "Thanh toán" : "Checkout",
    continueShopping: language === "vi" ? "Tiếp tục mua hàng" : "Continue shopping",
  };

  const shippingFee = 0; // Free ship placeholder
  const grandTotal = totalAmount + shippingFee;

  return (
    <div className="bg-gray-50 rounded-xl p-5 md:p-6 space-y-4 sticky top-28">
      <h2 className="text-lg font-bold text-navy">{text.title}</h2>

      <div className="space-y-3 text-sm">
        <div className="flex justify-between">
          <span className="text-gray-500">{text.subtotal} ({totalQuantity} {text.productCount})</span>
          <span className="font-medium text-navy">{formatPrice(totalAmount)}</span>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-500">{text.shipping}</span>
          <span className="font-medium text-success">{text.free}</span>
        </div>
        <div className="border-t border-gray-200 pt-3 flex justify-between">
          <span className="font-semibold text-navy">{text.total}</span>
          <span className="text-lg font-bold text-primary">{formatPrice(grandTotal)}</span>
        </div>
      </div>

      <Link
        href="/checkout"
        className="btn-primary w-full btn-lg mt-4 text-center"
      >
        {text.checkout}
      </Link>

      <Link
        href="/esim-du-lich"
        className="block text-center text-sm text-gray-500 hover:text-primary transition mt-2"
      >
        {text.continueShopping}
      </Link>
    </div>
  );
}
