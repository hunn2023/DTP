"use client";

import { useState } from "react";
import Icon from "@/components/ui/Icon";
import { useLanguage } from "@/hooks/useLanguage";
import type { EsimPackage } from "@/types/esim";

interface PackageCardProps {
  pkg: EsimPackage;
  onBuy?: (pkg: EsimPackage, quantity: number, triggerElement: HTMLElement | null) => void;
  onBuyNow?: (pkg: EsimPackage, quantity: number) => void;
}

const BOOLEAN_FEATURE_ICONS: Record<string, string> = {
  "Data không giới hạn": "∞",
  "Hỗ trợ chia sẻ Hotspot": "📶",
  "Hỗ trợ số điện thoại": "📞",
  "Hỗ trợ SMS": "💬",
  "Yêu cầu KYC": "🪪",
  "Unlimited data": "∞",
  "Hotspot supported": "📶",
  "Phone number supported": "📞",
  "SMS supported": "💬",
  "KYC required": "🪪",
};

export default function PackageCard({ pkg, onBuy, onBuyNow }: PackageCardProps) {
  const [quantity, setQuantity] = useState(1);
  const [showAllCoverage, setShowAllCoverage] = useState(false);
  const { language } = useLanguage();

  const tagStyles: Record<NonNullable<EsimPackage["tagType"]>, { bg: string; color: string }> = {
    default:   { bg: "rgba(0,102,255,0.1)", color: "#0066FF" },
    unlimited: { bg: "#FEF3C7",              color: "#92400E" },
    popular:   { bg: "#DCFCE7",              color: "#166534" },
  };
  const tagStyle = tagStyles[pkg.tagType || "default"];
  const maxQuantity = Math.max(1, Math.min(pkg.stock, 99));
  const totalPrice = pkg.price * quantity;
  const formatPrice = (value: number) => `${value.toLocaleString("vi-VN")}đ`;

  const text = {
    unitPrice: language === "vi" ? "Đơn giá" : "Unit price",
    quantity: language === "vi" ? "Số lượng" : "Quantity",
    subtotal: language === "vi" ? "Tạm tính" : "Subtotal",
    decreaseQuantity: language === "vi" ? "Giảm số lượng" : "Decrease quantity",
    increaseQuantity: language === "vi" ? "Tăng số lượng" : "Increase quantity",
    addToCart: language === "vi" ? "Thêm vào giỏ hàng" : "Add to cart",
    buyNow: language === "vi" ? "Đặt hàng ngay" : "Buy now",
    coverage: language === "vi" ? "Phạm vi phủ sóng" : "Coverage",
    countries: language === "vi" ? "quốc gia" : "countries",
    collapse: language === "vi" ? "Thu gọn" : "Show less",
    operators: language === "vi" ? "Nhà mạng tại điểm đến" : "Network at destination",
  };

  const coverageTypeLabel = (() => {
    if (!pkg.coverageType) return null;
    const normalizedType = pkg.coverageType.toLowerCase();
    if (normalizedType === "regional") return language === "vi" ? "Khu vực" : "Regional";
    if (normalizedType === "local") return language === "vi" ? "Quốc gia" : "Local";
    if (normalizedType === "global") return language === "vi" ? "Toàn cầu" : "Global";
    return pkg.coverageType;
  })();

  const structuredCoverageLocations = (pkg.coverages ?? [])
    .map((coverage) => coverage.countryName.trim())
    .filter(Boolean);
  const coverageLocations = structuredCoverageLocations.length > 0
    ? structuredCoverageLocations
    : (pkg.coverageDescription ?? "")
        .split(/[;\n]+/)
        .map((location) => location.trim())
        .filter(Boolean);
  const visibleCoverageLocations = showAllCoverage
    ? coverageLocations
    : coverageLocations.slice(0, 2);

  const translateText = (value: string) => {
    if (language === "vi") return value;
    const map: Record<string, string> = {
      "Tốc độ cao toàn thời gian": "High-speed data at all times",
      "Phù hợp đi 7-10 ngày": "Ideal for 7–10 day trips",
      "2GB/ngày tốc độ cao": "2GB/day high-speed data",
      "Đi ngắn ngày, công tác": "Short trips & business travel",
      "Đi dài ngày, tour 2 tuần": "Long stays & 2-week tours",
      "Du học, công tác dài hạn": "Study abroad & long-term business",
      "Mạng 5G NTT Docomo": "NTT Docomo 5G network",
      "Hỗ trợ chia sẻ Hotspot": "Hotspot sharing supported",
      "Kích hoạt khi đặt chân Nhật": "Activates on arrival in Japan",
      "Không cần đăng ký giấy tờ": "No paperwork required",
      "Mạng 5G/4G LTE": "5G/4G LTE network",
      "Tự động chuyển mạng tốt nhất": "Auto-switch to best network",
      "Mạng SoftBank 4G LTE": "SoftBank 4G LTE network",
      "Mạng 5G/4G+ chuẩn": "Stable 5G/4G+ connectivity",
      "30GB tốc độ cao": "30GB high-speed data",
      "Có số điện thoại gọi, nhắn tin": "Includes phone number",
      "7 NGÀY": "7 DAYS",
      "10 NGÀY": "10 DAYS",
      "5 NGÀY": "5 DAYS",
      "15 NGÀY": "15 DAYS",
      "⭐ UNLIMITED": "⭐ UNLIMITED",
      "📞 CÓ SĐT": "📞 WITH NUMBER",
      "Kích hoạt khi cài đặt eSIM": "Activates on eSIM install",
      "Kích hoạt khi sử dụng lần đầu": "Activates on first use",
      "Kích hoạt ngay sau khi mua": "Activates immediately after purchase",
      "Giao hàng: Qua email, ngay sau khi thanh toán, 24/7": "Delivery: Via email, right after payment, 24/7",
      "Chức năng: Kết nối internet, nghe gọi, nhắn tin": "Features: Internet, calls, SMS",
    };
    return map[value] ?? value;
  };

  const changeQuantity = (next: number) => {
    setQuantity(Math.max(1, Math.min(next, maxQuantity)));
  };

  // Split: booleanFeatures → chips, regular features → bullet list
  const chips = (pkg.booleanFeatures ?? []);
  const bullets = pkg.features ?? [];

  return (
    <div
      className="bg-white relative transition-all flex flex-col group hover:-translate-y-0.5"
      style={{
        borderRadius: "16px",
        border: pkg.featured ? "2px solid #0066FF" : "1.5px solid #E2E8F0",
        boxShadow: pkg.featured ? "0 4px 20px rgba(0,102,255,0.08)" : "none",
        overflow: "hidden",
      }}
    >
      {pkg.featuredLabel && (
        <span
          className="absolute text-white font-bold"
          style={{
            top: "-12px",
            left: "24px",
            background: "#0066FF",
            padding: "4px 12px",
            borderRadius: "6px",
            fontSize: "11px",
          }}
        >
          {pkg.featuredLabel}
        </span>
      )}

      {/* Header strip */}
      <div
        className="flex justify-between items-start"
        style={{ padding: "20px 20px 16px" }}
      >
        <div className="min-w-0 flex-1">
          <div
            className="font-extrabold text-navy leading-none"
            style={{ fontSize: "30px", letterSpacing: "-0.5px" }}
          >
            {pkg.data}
            <span className="text-gray-400 font-medium ml-1.5" style={{ fontSize: "14px" }}>
              {pkg.dataUnit}
            </span>
          </div>
          {pkg.subtitle && pkg.subtitle.toLowerCase() !== pkg.coverageType?.toLowerCase() && (
            <div className="text-gray-500 mt-1" style={{ fontSize: "12.5px" }}>
              {translateText(pkg.subtitle)}
            </div>
          )}
          {(coverageTypeLabel || coverageLocations.length > 0) && (
            <div className="mt-3 rounded-xl border border-blue-100 bg-blue-50/70 p-2.5">
              <div className="mb-2 flex items-center gap-2">
                <span className="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-white text-primary shadow-sm">
                  <Icon icon="globe" className="text-[11px]" />
                </span>
                <span className="text-[11px] font-semibold uppercase tracking-wide text-slate-600">
                  {text.coverage}
                </span>
                {coverageTypeLabel && (
                  <span className="ml-auto rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-bold text-primary">
                    {coverageTypeLabel}
                  </span>
                )}
              </div>

              {coverageLocations.length > 0 && (
                <div className="flex flex-wrap gap-1.5">
                  {visibleCoverageLocations.map((location) => (
                    <span
                      key={location}
                      className="max-w-full truncate rounded-full border border-slate-200 bg-white px-2.5 py-1 text-[11px] font-medium text-slate-700 shadow-sm"
                      title={location}
                    >
                      {location}
                    </span>
                  ))}
                  {coverageLocations.length > 2 && (
                    <button
                      type="button"
                      onClick={() => setShowAllCoverage((current) => !current)}
                      className="rounded-full border border-primary/20 bg-white px-2.5 py-1 text-[11px] font-semibold text-primary transition hover:border-primary hover:bg-primary hover:text-white"
                      aria-expanded={showAllCoverage}
                    >
                      {showAllCoverage
                        ? text.collapse
                        : `+${coverageLocations.length - 2} ${text.countries}`}
                    </button>
                  )}
                </div>
              )}
              {(pkg.destinationOperators?.length ?? 0) > 0 && (
                <div className="mt-2 border-t border-blue-100 pt-2 text-[11px] text-slate-600">
                  <span className="font-semibold">{text.operators}:</span>{" "}
                  {pkg.destinationOperators!.join(", ")}
                </div>
              )}
            </div>
          )}
        </div>
        <span
          className="ml-3 font-bold shrink-0"
          style={{
            background: tagStyle.bg,
            color: tagStyle.color,
            padding: "4px 10px",
            borderRadius: "6px",
            fontSize: "11px",
            whiteSpace: "nowrap",
          }}
        >
          {translateText(pkg.tag)}
        </span>
      </div>

      {pkg.description && (
        <div className="mx-5 mb-3 rounded-lg border border-amber-200 bg-amber-50 px-3 py-2 text-[12.5px] leading-relaxed text-amber-900">
          {pkg.description}
        </div>
      )}

      {/* Boolean feature chips */}
      {chips.length > 0 && (
        <div
          className="flex flex-wrap gap-1.5 px-5"
          style={{ paddingBottom: "12px" }}
        >
          {chips.map((chip) => (
            <span
              key={chip}
              className="inline-flex items-center gap-1 font-medium"
              style={{
                background: "#F0F7FF",
                color: "#0055CC",
                fontSize: "11px",
                padding: "3px 9px",
                borderRadius: "20px",
                border: "1px solid rgba(0,102,255,0.15)",
              }}
            >
              {BOOLEAN_FEATURE_ICONS[translateText(chip)] ?? "✓"} {translateText(chip)}
            </span>
          ))}
        </div>
      )}

      <div style={{ borderTop: "1px solid #F1F5F9", margin: "0 20px" }} />

      {/* Feature bullet list */}
      {bullets.length > 0 && (
        <ul
          className="list-none flex-grow"
          style={{ margin: "12px 0", padding: "0 20px" }}
        >
          {bullets.map((f) => (
            <li
              key={f}
              className="flex items-start gap-2 text-gray-600"
              style={{ fontSize: "12.5px", padding: "4px 0", lineHeight: "1.45" }}
            >
              <span className="text-primary font-bold mt-px shrink-0">✓</span>
              <span>{translateText(f)}</span>
            </li>
          ))}
        </ul>
      )}

      <div style={{ borderTop: "1px solid #F1F5F9", margin: "0 20px" }} />

      {/* Price + quantity + buy */}
      <div className="flex flex-col gap-3 p-5 pt-4">
        {/* Price row */}
        <div className="relative rounded-xl border border-slate-200 bg-slate-50/80 px-3 py-2.5">
          {pkg.discount && (
            <span
              className="absolute font-bold"
              style={{
                top: "8px",
                right: "8px",
                background: "#FEE2E2",
                color: "#991B1B",
                padding: "2px 8px",
                borderRadius: "6px",
                fontSize: "11px",
              }}
            >
              {pkg.discount}
            </span>
          )}
          <p className="text-[11px] font-semibold uppercase tracking-wide text-slate-500">{text.unitPrice}</p>
          {pkg.oldPrice && (
            <div className="line-through text-gray-400" style={{ fontSize: "12px" }}>
              {formatPrice(pkg.oldPrice)}
            </div>
          )}
          <span className="font-extrabold text-navy" style={{ fontSize: "24px", letterSpacing: "-0.5px" }}>
            {formatPrice(pkg.price)}
          </span>
        </div>

        {/* Quantity row */}
        <div className="grid grid-cols-[1fr_auto] items-center gap-3 rounded-xl border border-slate-200 bg-white px-3 py-2.5">
          <div>
            <p className="text-[11px] font-semibold uppercase tracking-wide text-slate-500">{text.quantity}</p>
            <p className="text-sm font-semibold text-navy">{text.subtotal} x{quantity}</p>
          </div>
          <div className="flex items-center gap-2 rounded-xl border border-slate-200 bg-slate-50 p-1">
            <button
              type="button"
              onClick={() => changeQuantity(quantity - 1)}
              disabled={quantity <= 1}
              className="flex h-8 w-8 items-center justify-center rounded-lg bg-white text-gray-700 shadow-sm disabled:cursor-not-allowed disabled:opacity-50"
              aria-label={text.decreaseQuantity}
            >
              <Icon icon="minus" className="text-xs" />
            </button>
            <span className="min-w-7 text-center text-sm font-bold text-navy">{quantity}</span>
            <button
              type="button"
              onClick={() => changeQuantity(quantity + 1)}
              disabled={quantity >= maxQuantity}
              className="flex h-8 w-8 items-center justify-center rounded-lg bg-white text-gray-700 shadow-sm disabled:cursor-not-allowed disabled:opacity-50"
              aria-label={text.increaseQuantity}
            >
              <Icon icon="plus" className="text-xs" />
            </button>
          </div>
        </div>

        {/* Subtotal */}
        <div className="flex items-center justify-between rounded-xl border border-primary/25 bg-primary/5 px-3 py-2.5">
          <p className="text-xs font-semibold text-slate-600">
            {formatPrice(pkg.price)} × {quantity}
          </p>
          <p className="text-lg font-extrabold text-primary">{formatPrice(totalPrice)}</p>
        </div>

        {/* Action buttons */}
        <div className="grid grid-cols-2 gap-2">
          <button
            type="button"
            onClick={(event) => onBuy?.(pkg, quantity, event.currentTarget)}
            className="gradient-primary text-white font-bold no-underline flex min-h-[46px] items-center justify-center gap-2 transition hover:brightness-105"
            style={{
              padding: "12px 10px",
              borderRadius: "10px",
              fontSize: "13px",
            }}
          >
            <Icon icon="shopping-cart" /> <span className="leading-tight">{text.addToCart}</span>
          </button>
          <button
            type="button"
            onClick={() => onBuyNow?.(pkg, quantity)}
            className="bg-navy text-white font-bold no-underline flex min-h-[46px] items-center justify-center gap-2 transition hover:brightness-110"
            style={{
              padding: "12px 10px",
              borderRadius: "10px",
              fontSize: "13px",
            }}
          >
            <Icon icon="bolt" /> <span className="leading-tight">{text.buyNow}</span>
          </button>
        </div>
      </div>
    </div>
  );
}
