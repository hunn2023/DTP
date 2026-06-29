"use client";

import { UseFormRegister, FieldErrors } from "react-hook-form";
import { CheckoutFormData } from "@/lib/schemas/checkoutSchema";
import type { Language } from "@/lib/i18n";

interface Props {
  register: UseFormRegister<CheckoutFormData>;
  errors: FieldErrors<CheckoutFormData>;
  language?: Language;
}

export default function ShippingInfoForm({ register, errors, language = "vi" }: Props) {
  const text = {
    title: language === "vi" ? "Thông tin người mua" : "Buyer information",
    fullName: language === "vi" ? "Họ và tên *" : "Full name *",
    fullNamePlaceholder: language === "vi" ? "Nhập họ và tên" : "Enter full name",
    phone: language === "vi" ? "Số điện thoại *" : "Phone number *",
    phonePlaceholder: language === "vi" ? "0xxxxxxxxx hoặc +84xxxxxxxxx" : "0xxxxxxxxx or +84xxxxxxxxx",
    email: language === "vi" ? "Email (tùy chọn)" : "Email (optional)",
    orderNote: language === "vi" ? "Ghi chú đơn hàng (tùy chọn)" : "Order note (optional)",
    orderNotePlaceholder:
      language === "vi"
        ? "Ghi chú cho đơn hàng (ví dụ: yêu cầu đặc biệt)"
        : "Notes for your order (e.g. special requests)",
  };

  return (
    <div className="space-y-4">
      <h3 className="text-lg font-semibold text-navy">{text.title}</h3>

      {/* Full name */}
      <div>
        <label className="block text-sm font-medium text-navy mb-1">{text.fullName}</label>
        <input
          {...register("fullName")}
          className={`input ${errors.fullName ? "border-danger" : ""}`}
          placeholder={text.fullNamePlaceholder}
        />
        {errors.fullName && (
          <p className="text-danger text-xs mt-1">{errors.fullName.message}</p>
        )}
      </div>

      {/* Phone */}
      <div>
        <label className="block text-sm font-medium text-navy mb-1">{text.phone}</label>
        <input
          {...register("phone")}
          className={`input ${errors.phone ? "border-danger" : ""}`}
          placeholder={text.phonePlaceholder}
        />
        {errors.phone && (
          <p className="text-danger text-xs mt-1">{errors.phone.message}</p>
        )}
      </div>

      {/* Email */}
      <div>
        <label className="block text-sm font-medium text-navy mb-1">{text.email}</label>
        <input
          {...register("email")}
          type="email"
          className={`input ${errors.email ? "border-danger" : ""}`}
          placeholder="email@example.com"
        />
        {errors.email && (
          <p className="text-danger text-xs mt-1">{errors.email.message}</p>
        )}
      </div>

      {/* Order note */}
      <div>
        <label className="block text-sm font-medium text-navy mb-1">{text.orderNote}</label>
        <textarea
          {...register("orderNote")}
          rows={3}
          className={`input resize-none ${errors.orderNote ? "border-danger" : ""}`}
          placeholder={text.orderNotePlaceholder}
        />
        {errors.orderNote && (
          <p className="text-danger text-xs mt-1">{errors.orderNote.message}</p>
        )}
      </div>
    </div>
  );
}
