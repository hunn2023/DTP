"use client";

import Image from "next/image";
import { useLanguage } from "@/hooks/useLanguage";

export interface CountryHeroProps {
  slug?: string;
  flag: string;
  flagCode?: string;
  name: string;
  /** Product code (e.g. BLC-03-eSIM) */
  nameEn: string;
  tags: string[];
  stats: { label: string; value: string }[];
  description?: string | null;
  gradient?: string;
  textColor?: string;
  tagBg?: string;
}

function hashSlug(slug: string): number {
  let hash = 0;
  for (let i = 0; i < slug.length; i++) {
    hash = (hash << 5) - hash + slug.charCodeAt(i);
    hash |= 0;
  }
  return Math.abs(hash);
}

export default function CountryHero({
  slug = "",
  flag,
  name,
  nameEn,
  tags,
  stats,
  description,
}: CountryHeroProps) {
  const { language } = useLanguage();

  const hue = hashSlug(slug) % 360;
  const heroBg = `linear-gradient(135deg, hsl(${hue} 65% 90%) 0%, hsl(${(hue + 40) % 360} 70% 83%) 100%)`;
  const accentColor = `hsl(${hue} 55% 35%)`;
  const codeBg = `hsla(${hue} 50% 30% / 0.12)`;

  const locationText = tags.length > 0 ? tags.join(" • ") : null;

  const translateStat = (value: string) => {
    if (language === "vi") return value;
    const map: Record<string, string> = {
      "Số gói": "Packages",
      "Giá từ": "From",
      "Nhà mạng": "Carrier",
      "Tốc độ": "Speed",
      "Phủ sóng": "Coverage",
      "Hotspot": "Hotspot",
      "Đã bán": "Sold",
    };
    return map[value] ?? value;
  };

  return (
    <section
      className="relative overflow-hidden"
      style={{ background: heroBg, padding: "48px 0" }}
    >
      {/* Decorative blob */}
      <div
        className="absolute pointer-events-none"
        style={{
          top: "-30%",
          right: "-5%",
          width: "400px",
          height: "400px",
          background: `radial-gradient(circle, hsla(${(hue + 20) % 360} 80% 70% / 0.25), transparent 70%)`,
          borderRadius: "50%",
        }}
      />

      <div className="max-w-container mx-auto px-6 grid md:grid-cols-[2fr_1fr] gap-10 items-start relative">

        {/* ── LEFT: product info ── */}
        <div className="flex flex-col gap-5">

          {/* Thumbnail + name block */}
          <div className="flex gap-5 items-start">
            {/* Thumbnail */}
            <div
              className="flex-shrink-0 flex items-center justify-center overflow-hidden bg-white"
              style={{
                width: "96px",
                height: "96px",
                borderRadius: "20px",
                boxShadow: "0 8px 28px rgba(0,0,0,0.12)",
              }}
            >
              {flag && flag.startsWith("http") ? (
                <Image
                  src={flag}
                  alt={name}
                  width={96}
                  height={96}
                  className="w-full h-full object-cover"
                />
              ) : (
                <span style={{ fontSize: "52px" }}>{flag || "🌐"}</span>
              )}
            </div>

            {/* Name, code, location */}
            <div className="flex flex-col gap-1.5 min-w-0">
              <h1
                className="font-extrabold leading-tight"
                style={{
                  fontSize: "clamp(22px, 4vw, 34px)",
                  color: accentColor,
                  letterSpacing: "-0.5px",
                }}
              >
                {name}
              </h1>

              {nameEn && (
                <span
                  className="inline-flex items-center self-start font-mono font-semibold"
                  style={{
                    fontSize: "11px",
                    background: codeBg,
                    color: accentColor,
                    padding: "2px 10px",
                    borderRadius: "6px",
                    letterSpacing: "0.5px",
                  }}
                >
                  {nameEn}
                </span>
              )}

              {locationText && (
                <p
                  className="font-medium"
                  style={{ fontSize: "13px", color: accentColor, opacity: 0.75 }}
                >
                  📍 {locationText}
                </p>
              )}
            </div>
          </div>

          {/* Description */}
          {description && (
            <div
              className="text-sm leading-relaxed"
              style={{
                background: "rgba(255,255,255,0.45)",
                borderRadius: "14px",
                padding: "14px 18px",
                color: accentColor,
                borderLeft: `3px solid hsla(${hue} 55% 35% / 0.35)`,
              }}
            >
              {description}
            </div>
          )}
        </div>

        {/* ── RIGHT: stats card ── */}
        <div
          className="bg-white"
          style={{
            borderRadius: "18px",
            padding: "22px 24px",
            boxShadow: "0 8px 28px rgba(0,0,0,0.07)",
          }}
        >
          {stats.length > 0 ? (
            stats.map((s, i) => (
              <div
                key={s.label}
                className="flex items-baseline gap-4"
                style={{
                  padding: "10px 0",
                  fontSize: "13px",
                  borderTop: i > 0 ? "1px solid #F1F5F9" : "none",
                }}
              >
                <span className="text-gray-500 shrink-0">{translateStat(s.label)}</span>
                <span className="font-bold text-navy text-right flex-1 leading-snug">{s.value}</span>
              </div>
            ))
          ) : (
            <p className="text-sm text-gray-400 text-center py-2">—</p>
          )}

        </div>
      </div>
    </section>
  );
}
