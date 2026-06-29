"use client";

import { useState } from "react";
import Icon from "@/components/ui/Icon";
import type { IconName } from "@fortawesome/fontawesome-svg-core";
import type { CardPaymentMethod } from "@/types/cardMarketplace";
import { useLanguage } from "@/hooks/useLanguage";

const paymentMethods: { icon: IconName; label: CardPaymentMethod }[] = [
  { icon: "mobile-alt",  label: "Momo"          },
  { icon: "wallet",      label: "ZaloPay"       },
  { icon: "credit-card", label: "VNPay"         },
  { icon: "university",  label: "Banking"       },
  { icon: "qrcode",      label: "QR Code"       },
  { icon: "coins",       label: "Số dư EZSIM"   },
];

export interface CheckoutSectionProps {
  providerName: string;
  optionLabel: string;
  faceValue: number;
  payValue: number;
  discountPercent: number;
  providerDescription?: string;
  deliveryTime?: string;
  itemUnit?: string;
  onCheckout?: (payload: {
    quantity: number;
    paymentMethod: CardPaymentMethod;
    email: string;
    phone: string;
  }) => void;
}

export default function CheckoutSection({
  providerName,
  optionLabel,
  faceValue,
  payValue,
  discountPercent,
  providerDescription,
  deliveryTime,
  itemUnit = "thẻ",
  onCheckout,
}: CheckoutSectionProps) {
  const { language } = useLanguage();
  const [qty, setQty] = useState(2);
  const [payment, setPayment] = useState<CardPaymentMethod>("Momo");
  const [email, setEmail] = useState("khachhang@gmail.com");
  const [phone, setPhone] = useState("");

  const text = {
    itemUnit: language === "vi" ? itemUnit : "cards",
    quantityLabel: language === "vi" ? "Số lượng sản phẩm" : "Product quantity",
    quantityLimit:
      language === "vi" ? "(tối đa 50 sản phẩm/đơn)" : "(max 50 items/order)",
    emailLabel: language === "vi" ? "Email nhận đơn hàng" : "Order email",
    phoneLabel:
      language === "vi" ? "Số điện thoại (hỗ trợ dự phòng)" : "Phone number (backup support)",
    paymentMethod: language === "vi" ? "Phương thức thanh toán" : "Payment method",
    summaryTitle: language === "vi" ? "📋 Tóm tắt đơn hàng" : "📋 Order summary",
    estimatedDelivery:
      language === "vi" ? "Giao mã dự kiến" : "Estimated code delivery",
    product: language === "vi" ? "Sản phẩm" : "Product",
    selectedPackage: language === "vi" ? "Gói đã chọn" : "Selected package",
    quantity: language === "vi" ? "Số lượng" : "Quantity",
    subtotal: language === "vi" ? "Tạm tính" : "Subtotal",
    discount: language === "vi" ? "Chiết khấu" : "Discount",
    fee: language === "vi" ? "Phí giao dịch" : "Transaction fee",
    free: language === "vi" ? "Miễn phí" : "Free",
    total: language === "vi" ? "Tổng cộng" : "Total",
    payNow: language === "vi" ? "Thanh toán & nhận mã ngay" : "Pay now & receive code instantly",
    safeNotice:
      language === "vi"
        ? "🔒 Giao dịch an toàn - Thông tin đơn hàng gửi ngay sau khi thanh toán"
        : "🔒 Secure transaction - Order details are sent right after payment",
  };

  const originalSubtotal = faceValue * qty;
  const discount = originalSubtotal - payValue * qty;
  const total = payValue * qty;

  return (
    <div
      className="grid grid-cols-1 md:grid-cols-2 gap-8"
      style={{
        border: "2px solid #0066FF",
        borderRadius: "20px",
        padding: "28px",
        marginBottom: "32px",
        background: "white",
      }}
    >
      {/* Form */}
      <div className="flex flex-col gap-4">
        <div>
          <label className="font-semibold mb-1.5 block" style={{ fontSize: "13px" }}>
            {text.quantityLabel}
          </label>
          <div className="flex items-center gap-3">
            <button
              onClick={() => setQty(Math.max(1, qty - 1))}
              className="bg-white font-semibold cursor-pointer"
              style={{
                width: "36px",
                height: "36px",
                borderRadius: "8px",
                border: "1.5px solid #E2E8F0",
                fontSize: "18px",
              }}
            >
              −
            </button>
            <input
              type="text"
              value={qty}
              readOnly
              className="text-center font-bold font-sans"
              style={{
                width: "60px",
                padding: "8px",
                border: "1.5px solid #E2E8F0",
                borderRadius: "8px",
                fontSize: "16px",
              }}
            />
            <button
              onClick={() => setQty(Math.min(50, qty + 1))}
              className="bg-white font-semibold cursor-pointer"
              style={{
                width: "36px",
                height: "36px",
                borderRadius: "8px",
                border: "1.5px solid #E2E8F0",
                fontSize: "18px",
              }}
            >
              +
            </button>
            <span className="text-gray-500" style={{ fontSize: "13px" }}>
              {text.quantityLimit}
            </span>
          </div>
        </div>

        <div>
          <label className="font-semibold mb-1.5 block" style={{ fontSize: "13px" }}>
            {text.emailLabel} <span style={{ color: "#DC2626" }}>*</span>
          </label>
          <input
            type="email"
            placeholder="ban@example.com"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            className="w-full font-sans outline-none focus:border-primary"
            style={{
              padding: "12px 16px",
              border: "1.5px solid #E2E8F0",
              borderRadius: "10px",
              fontSize: "14px",
            }}
          />
        </div>

        <div>
          <label className="font-semibold mb-1.5 block" style={{ fontSize: "13px" }}>
            {text.phoneLabel}
          </label>
          <input
            type="tel"
            placeholder="0901 234 567"
            value={phone}
            onChange={(event) => setPhone(event.target.value)}
            className="w-full font-sans outline-none focus:border-primary"
            style={{
              padding: "12px 16px",
              border: "1.5px solid #E2E8F0",
              borderRadius: "10px",
              fontSize: "14px",
            }}
          />
        </div>

        <div>
          <label className="font-semibold mb-1.5 block" style={{ fontSize: "13px" }}>
            {text.paymentMethod}
          </label>
          <div className="grid grid-cols-3 gap-2">
            {paymentMethods.map((pm) => {
              const isSelected = payment === pm.label;
              return (
                <button
                  key={pm.label}
                  onClick={() => setPayment(pm.label)}
                  className="cursor-pointer text-center font-semibold transition"
                  style={{
                    padding: "10px",
                    border: isSelected ? "1.5px solid #0066FF" : "1.5px solid #E2E8F0",
                    borderRadius: "8px",
                    fontSize: "12px",
                    background: isSelected ? "rgba(0,102,255,0.05)" : "white",
                  }}
                >
                  <Icon icon={pm.icon} className="text-primary block mx-auto" style={{ fontSize: "20px", marginBottom: "4px" }} />
                  {pm.label}
                </button>
              );
            })}
          </div>
        </div>
      </div>

      {/* Order Summary */}
      <div
        className="bg-gray-50"
        style={{
          borderRadius: "16px",
          padding: "24px",
        }}
      >
        <h3 className="font-bold" style={{ fontSize: "16px", marginBottom: "16px" }}>
          {text.summaryTitle}
        </h3>

        {(providerDescription || deliveryTime) && (
          <div
            className="bg-white"
            style={{ borderRadius: "12px", padding: "14px 16px", marginBottom: "16px", border: "1px solid #E2E8F0" }}
          >
            {providerDescription && (
              <p className="text-gray-700" style={{ fontSize: "13px", lineHeight: 1.6 }}>
                {providerDescription}
              </p>
            )}
            {deliveryTime && (
              <p className="text-primary font-semibold" style={{ fontSize: "12px", marginTop: providerDescription ? "8px" : 0 }}>
                {text.estimatedDelivery}: {deliveryTime}
              </p>
            )}
          </div>
        )}

        <div style={{ fontSize: "14px" }}>
          <SummaryRow label={text.product} value={providerName} />
          <SummaryRow label={text.selectedPackage} value={optionLabel} />
          <SummaryRow label={text.quantity} value={`${qty} ${text.itemUnit}`} />

          <div
            className="flex justify-between"
            style={{
              padding: "16px 0 8px",
              marginTop: "8px",
              borderTop: "1px dashed #E2E8F0",
            }}
          >
            <span className="text-gray-700">{text.subtotal}</span>
            <span className="font-semibold">{originalSubtotal.toLocaleString("vi-VN")}đ</span>
          </div>

          <SummaryRow
            label={`${text.discount} (${discountPercent}%)`}
            value={`−${discount.toLocaleString("vi-VN")}đ`}
            valueColor="#DC2626"
          />
          <SummaryRow label={text.fee} value={text.free} valueColor="#16A34A" />

          <div
            className="flex justify-between items-center font-extrabold"
            style={{
              padding: "16px 0 0",
              marginTop: "12px",
              borderTop: "2px solid #E2E8F0",
              fontSize: "18px",
            }}
          >
            <span>{text.total}</span>
            <span className="text-primary" style={{ fontSize: "24px" }}>
              {total.toLocaleString("vi-VN")}đ
            </span>
          </div>
        </div>

        <button
          type="button"
          onClick={() => onCheckout?.({ quantity: qty, paymentMethod: payment, email, phone })}
          className="w-full gradient-primary text-white font-bold cursor-pointer flex items-center justify-center gap-2"
          style={{
            padding: "16px",
            borderRadius: "12px",
            fontSize: "16px",
            marginTop: "16px",
          }}
        >
          <Icon icon="bolt" /> {text.payNow}
        </button>

        <p className="text-center text-gray-500" style={{ fontSize: "12px", marginTop: "12px" }}>
          {text.safeNotice}
        </p>
      </div>
    </div>
  );
}

function SummaryRow({
  label,
  value,
  valueColor,
}: {
  label: string;
  value: string;
  valueColor?: string;
}) {
  return (
    <div className="flex justify-between" style={{ padding: "8px 0" }}>
      <span className="text-gray-700">{label}</span>
      <span className="font-semibold" style={{ color: valueColor }}>
        {value}
      </span>
    </div>
  );
}
