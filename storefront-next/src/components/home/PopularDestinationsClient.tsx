"use client";

import { useState } from "react";
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
  const [selectedRegion, setSelectedRegion] = useState("");

  const defaultRegionOrder = ["Châu Á", "Châu Âu", "Châu Mỹ", "Châu Đại Dương", "Châu Phi"];
  const groupedProducts = products.reduce<Record<string, HomeEsimProduct[]>>((groups, product) => {
    const region = product.region?.trim();
    if (!region) return groups;
    (groups[region] ??= []).push(product);
    return groups;
  }, {});
  const regions = [
    ...defaultRegionOrder.filter((region) => groupedProducts[region]?.length),
    ...Object.keys(groupedProducts)
      .filter((region) => !defaultRegionOrder.includes(region))
      .sort((left, right) => left.localeCompare(right, language === "vi" ? "vi" : "en")),
  ];
  const activeRegion = regions.includes(selectedRegion) ? selectedRegion : regions[0];
  const visibleProducts = activeRegion ? groupedProducts[activeRegion].slice(0, 5) : [];

  const displayRegion = (region: string) => {
    if (language === "vi") return region;
    const labels: Record<string, string> = {
      "Châu Á": "Asia",
      "Châu Âu": "Europe",
      "Châu Mỹ": "Americas",
      "Châu Đại Dương": "Oceania",
      "Châu Phi": "Africa",
    };
    return labels[region] ?? region;
  };

  const text = {
    heading: language === "vi" ? "Điểm đến nổi bật" : "Featured destinations",
    subtitle:
      language === "vi"
        ? "Khám phá các điểm đến nổi bật theo từng châu lục"
        : "Explore featured destinations by region",
    viewAll: language === "vi" ? "Xem tất cả 200+ quốc gia" : "View all 200+ countries",
    priceFrom: language === "vi" ? "Giá từ" : "From",
    hot: "HOT",
  };

  if (products.length === 0 || regions.length === 0) return null;

  return (
    <section style={{ padding: "0 0 64px" }}>
      <div className="max-w-container mx-auto px-6">
        <div className="mb-8">
          <h2 className="section-title">{text.heading}</h2>
          <p className="section-subtitle">{text.subtitle}</p>
        </div>

        <div className="mb-7 flex gap-2 overflow-x-auto pb-2" role="tablist" aria-label={text.heading}>
          {regions.map((region) => {
            const isActive = region === activeRegion;
            return (
              <button
                key={region}
                type="button"
                role="tab"
                aria-selected={isActive}
                onClick={() => setSelectedRegion(region)}
                className={`shrink-0 rounded-full border px-4 py-2 text-sm font-semibold transition ${
                  isActive
                    ? "border-primary bg-primary text-white shadow-sm"
                    : "border-slate-200 bg-white text-slate-600 hover:border-primary hover:text-primary"
                }`}
              >
                {displayRegion(region)}
                <span className={`ml-2 text-xs ${isActive ? "text-white/75" : "text-slate-400"}`}>
                  {groupedProducts[region].length}
                </span>
              </button>
            );
          })}
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5 gap-5">
          {visibleProducts.map((product, index) => (
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
              <div className="relative overflow-hidden bg-gradient-to-br from-sky-50 via-blue-100 to-indigo-100" style={{ height: "164px" }}>
                {product.thumbnailUrl ? (
                  <Image
                    src={product.thumbnailUrl}
                    alt={product.name}
                    fill
                    sizes="(max-width: 640px) 100vw, (max-width: 1280px) 33vw, 20vw"
                    priority={index < 3}
                    className="h-full w-full object-cover transition-transform duration-700 group-hover:scale-110"
                  />
                ) : (
                  <div className="relative flex h-full w-full items-center justify-center">
                    {product.flagUrl && (
                      <>
                        <Image
                          src={product.flagUrl}
                          alt=""
                          fill
                          sizes="320px"
                          className="scale-125 object-cover opacity-15 blur-2xl"
                          aria-hidden="true"
                        />
                        <span className="relative inline-flex h-16 w-24 overflow-hidden rounded-xl border-4 border-white shadow-lg">
                          <Image src={product.flagUrl} alt={product.name} fill sizes="96px" className="object-cover" />
                        </span>
                      </>
                    )}
                  </div>
                )}
                <div className="absolute inset-0 bg-gradient-to-t from-slate-950/15 via-transparent to-slate-950/5" />
                {product.region && (
                  <span className="absolute left-3 top-3 rounded-full border border-white/60 bg-white/90 px-2.5 py-1 text-[10px] font-bold uppercase tracking-wide text-slate-700 shadow-sm backdrop-blur">
                    {product.region}
                  </span>
                )}
                {false && product.flagUrl && (
                  <span
                    className="hidden"
                    aria-label={`Cờ ${product.name}`}
                  >
                    <Image
                      src={product.flagUrl!}
                      alt={product.name}
                      width={64}
                      height={44}
                      className="h-full w-full object-cover"
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

              <div className="px-4 pb-4 pt-4">
                <div className="flex min-w-0 items-center gap-2.5">
                  {product.flagUrl && (
                    <span className="relative inline-flex h-5 w-7 shrink-0 overflow-hidden rounded border border-slate-200 bg-slate-50 shadow-sm">
                      <Image
                        src={product.flagUrl}
                        alt={`Flag ${product.name}`}
                        fill
                        sizes="28px"
                        className="object-cover"
                      />
                    </span>
                  )}
                  <div className="truncate font-extrabold text-navy" style={{ fontSize: "18px", letterSpacing: "-0.2px" }}>
                    {product.name}
                  </div>
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
            href={`/esim-du-lich?region=${encodeURIComponent(activeRegion)}`}
            className="inline-flex items-center justify-center gap-2 px-5 py-3 rounded-xl text-sm font-semibold text-white gradient-primary shadow-sm hover:opacity-90 transition"
          >
            {text.viewAll} <Icon icon="arrow-right" />
          </Link>
        </div>
      </div>
    </section>
  );
}
