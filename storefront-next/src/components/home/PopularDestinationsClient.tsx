"use client";

import Link from "next/link";
import Image from "next/image";
import Icon from "@/components/ui/Icon";
import { useLanguage } from "@/hooks/useLanguage";
import type { HomeEsimProduct } from "@/lib/api/esimApi";

interface PopularDestinationsClientProps {
  products: HomeEsimProduct[];
}

export default function PopularDestinationsClient({ products }: PopularDestinationsClientProps) {
  const { language } = useLanguage();

  const text = {
    heading: language === "vi" ? "Điểm đến nổi bật" : "Featured destinations",
    subtitle:
      language === "vi"
        ? "Top quốc gia được mua nhiều nhất với giá tốt nhất hôm nay"
        : "Top booked destinations with best prices today",
    viewAll: language === "vi" ? "Xem tất cả 200+ quốc gia" : "View all 200+ countries",
    priceFrom: language === "vi" ? "Giá từ" : "From",
    hot: "HOT",
  };

  if (products.length === 0) return null;

  return (
    <section style={{ padding: "0 0 64px" }}>
      <div className="max-w-container mx-auto px-6">
        <div className="mb-8">
          <h2 className="section-title">{text.heading}</h2>
          <p className="section-subtitle">{text.subtitle}</p>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5 gap-5">
          {products.map((product, index) => (
            <Link
              key={product.id}
              href={`/esim-du-lich/${product.slug}`}
              className="bg-white text-navy no-underline transition-all duration-300 group hover:-translate-y-1"
              style={{
                border: "1.5px solid #E2E8F0",
                borderRadius: "18px",
                overflow: "hidden",
              }}
            >
              <div className="relative" style={{ height: "150px" }}>
                {product.thumbnailUrl ? (
                  <Image
                    src={product.thumbnailUrl}
                    alt={product.name}
                    fill
                    sizes="(max-width: 640px) 100vw, (max-width: 1280px) 33vw, 20vw"
                    priority={index < 3}
                    className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                  />
                ) : (
                  <div className="w-full h-full bg-gradient-to-br from-sky-100 to-blue-200" />
                )}
                <div
                  className="absolute inset-x-0 bottom-0"
                  style={{
                    height: "64px",
                    background: "linear-gradient(180deg, rgba(15,23,42,0) 0%, rgba(15,23,42,0.72) 100%)",
                  }}
                />
                {product.flagUrl && (
                  <span
                    className="absolute top-3 left-3 inline-flex items-center justify-center rounded-md bg-white/95 border border-white/80 shadow-sm overflow-hidden"
                    style={{ width: "38px", height: "26px" }}
                    aria-label={`Cờ ${product.name}`}
                  >
                    <Image
                      src={product.flagUrl}
                      alt={product.name}
                      width={38}
                      height={26}
                      className="w-full h-full object-cover"
                    />
                  </span>
                )}
                {product.isHot && (
                  <span
                    className="absolute top-3 right-3 text-white font-bold"
                    style={{
                      background: "linear-gradient(135deg, #E11D48 0%, #F43F5E 100%)",
                      padding: "4px 10px",
                      borderRadius: "999px",
                      fontSize: "10px",
                      letterSpacing: "0.2px",
                    }}
                  >
                    {text.hot}
                  </span>
                )}
              </div>

              <div style={{ padding: "14px 14px 16px" }}>
                {product.locationText && (
                  <p className="text-gray-500 mb-1" style={{ fontSize: "12px" }}>
                    {product.locationText}
                  </p>
                )}
                <div className="font-extrabold text-navy" style={{ fontSize: "18px", letterSpacing: "-0.2px" }}>
                  {product.name}
                </div>
                <div className="flex items-center justify-between mt-2.5">
                  <div>
                    <p className="text-gray-500" style={{ fontSize: "11px" }}>
                      {text.priceFrom}
                    </p>
                    <p className="text-primary font-extrabold" style={{ fontSize: "18px", letterSpacing: "-0.2px" }}>
                      {product.priceFrom.toLocaleString("vi-VN")}đ
                    </p>
                  </div>
                  <span
                    className="inline-flex items-center justify-center rounded-full text-primary"
                    style={{ width: "34px", height: "34px", background: "#EFF6FF" }}
                  >
                    <Icon icon="arrow-right" className="text-sm" />
                  </span>
                </div>
              </div>
            </Link>
          ))}
        </div>

        <div className="mt-8 flex justify-center">
          <Link
            href="/esim-du-lich"
            className="inline-flex items-center justify-center gap-2 px-5 py-3 rounded-xl text-sm font-semibold text-white gradient-primary shadow-sm hover:opacity-90 transition"
          >
            {text.viewAll} <Icon icon="arrow-right" />
          </Link>
        </div>
      </div>
    </section>
  );
}
