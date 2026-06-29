"use client";

import { Suspense } from "react";
import Image from "next/image";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import CountrySearchBox from "@/components/common/CountrySearchBox";
import { Breadcrumb } from "@/components/ui";
import { useLanguage } from "@/hooks/useLanguage";
import { removeDiacritics } from "@/lib/text";
import type { EsimCountrySummary } from "@/types/esim";

const COUNTRY_FLAG_CODES: Record<string, string> = {
  "nhat-ban": "jp",
  "han-quoc": "kr",
  "thai-lan": "th",
  "chau-au": "eu",
  "my": "us",
};

type EsimRegion = NonNullable<EsimCountrySummary["region"]>;

const REGION_BADGES: Record<EsimRegion, string> = {
  "Châu Á": "🌏",
  "Châu Âu": "",
  "Châu Mỹ": "🌎",
  "Châu Đại Dương": "🌊",
};

const REGION_FLAG_CODES: Partial<Record<EsimRegion, string>> = {
  "Châu Âu": "eu",
};

const DEFAULT_VISIBLE_COUNTRIES = 50;
export default function EsimDuLichContent({
  destinations,
}: {
  destinations: EsimCountrySummary[];
}) {
  return (
    <Suspense fallback={<div className="min-h-[400px]" />}>
      <EsimDuLichContentInner destinations={destinations} />
    </Suspense>
  );
}

function EsimDuLichContentInner({
  destinations,
}: {
  destinations: EsimCountrySummary[];
}) {
  const { language } = useLanguage();
  const searchParams = useSearchParams();
  const keywordRaw = searchParams.get("q")?.trim() ?? "";
  const keyword = removeDiacritics(keywordRaw);
  const selectedRegion = searchParams.get("region")?.trim() ?? "";

  const keywordFilteredDestinations = keyword
    ? destinations.filter(
        (destination) =>
          removeDiacritics(destination.name).includes(keyword) ||
          removeDiacritics(destination.region ?? "").includes(keyword)
      )
    : destinations;

  const regionOrder: EsimRegion[] = [
    "Châu Á",
    "Châu Âu",
    "Châu Mỹ",
    "Châu Đại Dương",
  ];

  const regionCounts = keywordFilteredDestinations.reduce<Record<string, number>>((acc, destination) => {
    if (!destination.region) return acc;
    acc[destination.region] = (acc[destination.region] ?? 0) + 1;
    return acc;
  }, {});

  const filteredDestinations = keywordFilteredDestinations.filter(
    (destination) => !selectedRegion || destination.region === selectedRegion
  );

  const visibleDestinations =
    !keywordRaw && !selectedRegion
      ? filteredDestinations.slice(0, DEFAULT_VISIBLE_COUNTRIES)
      : filteredDestinations;

  const buildFilterHref = (next: { region?: string }) => {
    const params = new URLSearchParams();
    if (keywordRaw) params.set("q", keywordRaw);
    if (next.region) params.set("region", next.region);
    const queryString = params.toString();
    return queryString ? `/esim-du-lich?${queryString}` : "/esim-du-lich";
  };

  const getFlagCode = (slug: string) => COUNTRY_FLAG_CODES[slug] ?? null;
  const getFlagUrl = (d: EsimCountrySummary) => {
    const code = getFlagCode(d.slug);
    if (code) return `https://flagcdn.com/w80/${code}.png`;
    // Use flag from API (flagUrl) directly
    if (d.flag && d.flag.startsWith("http")) return d.flag;
    return null;
  };
  const displayCountryName = (destination: EsimCountrySummary) => {
    if (language === "vi") return destination.name;

    const map: Record<string, string> = {
      "nhat-ban": "Japan",
      "han-quoc": "South Korea",
      "thai-lan": "Thailand",
      "chau-au": "Europe",
      "my": "United States",
    };
    return map[destination.slug] ?? destination.name;
  };

  const displayRegionName = (region: EsimRegion) => {
    if (language === "vi") return region;

    const map: Record<EsimRegion, string> = {
      "Châu Á": "Asia",
      "Châu Âu": "Europe",
      "Châu Mỹ": "Americas",
      "Châu Đại Dương": "Oceania",
    };

    return map[region];
  };

  const text = {
    breadcrumb: language === "vi" ? "eSIM Du lịch" : "Travel eSIM",
    heroTitle: language === "vi" ? "eSIM Du lịch 200+ quốc gia" : "Travel eSIM in 200+ countries",
    heroDescription:
      language === "vi"
        ? "Kết nối ngay khi đặt chân tới bất kỳ quốc gia nào. Quét QR là dùng — không cần tháo SIM gốc, không lo roaming."
        : "Stay connected the moment you arrive. Scan the QR code and go online instantly with no SIM swap and no roaming stress.",
    searchResultPrefix: language === "vi" ? "Kết quả tìm kiếm cho" : "Search results for",
    searchResultSuffix: language === "vi" ? "điểm đến phù hợp." : "matching destinations.",
    filterTitle: language === "vi" ? "Lọc điểm đến" : "Filter destinations",
    filterRegion: language === "vi" ? "Khu vực" : "Region",
    filterAllRegions: language === "vi" ? "Tất cả khu vực" : "All regions",
    filterSearch: language === "vi" ? "Tìm điểm đến" : "Search destinations",
    searchPlaceholder: language === "vi" ? "Tìm quốc gia eSIM..." : "Search eSIM countries...",
    searchNotFound: language === "vi" ? "Không tìm thấy quốc gia phù hợp." : "No matching country found.",
    clearFilters: language === "vi" ? "Xóa bộ lọc" : "Clear filters",
    from: language === "vi" ? "Từ" : "From",
    packagesAvailable: language === "vi" ? "gói khả dụng" : "packages available",
    bestSeller: language === "vi" ? "🔥 BÁN CHẠY" : "🔥 BEST SELLER",
    emptyTitle: language === "vi" ? "Chưa có điểm đến phù hợp" : "No matching destinations yet",
    emptyDescription:
      language === "vi"
        ? "Thử tìm theo tên quốc gia khác hoặc quay lại danh sách đầy đủ."
        : "Try another country keyword or return to the full destination list.",
  };

  const byRegion = visibleDestinations.reduce<Record<string, EsimCountrySummary[]>>((acc, d) => {
    if (!d.region) return acc;
    (acc[d.region] ??= []).push(d);
    return acc;
  }, {});

  const noRegionDestinations = visibleDestinations.filter((d) => !d.region);

  const orderedByRegionEntries = [
    ...regionOrder
      .map((region) => [region, byRegion[region] ?? []] as const)
      .filter(([, list]) => list.length > 0),
    ...(noRegionDestinations.length > 0 ? [[null, noRegionDestinations] as const] : []),
  ];

  return (
    <>
      <Breadcrumb items={[{ label: text.breadcrumb }]} />

      {/* Hero - mockup style header xanh đậm */}
      <section className="gradient-primary text-white relative overflow-hidden" style={{ padding: "48px 0" }}>
        <div
          className="absolute pointer-events-none"
          style={{
            top: "-50%",
            right: "-10%",
            width: "500px",
            height: "500px",
            background: "radial-gradient(circle, rgba(255,255,255,0.15), transparent 70%)",
            borderRadius: "50%",
          }}
        />
        <div className="max-w-container mx-auto px-6 relative">
          <h1 className="text-white mb-3">{text.heroTitle}</h1>
          <p className="text-white/90 max-w-[680px]" style={{ fontSize: "16px" }}>
            {text.heroDescription}
          </p>
          {keyword && (
            <p className="text-white/90 mt-4" style={{ fontSize: "14px" }}>
              {text.searchResultPrefix} <b>{keywordRaw}</b>: {filteredDestinations.length} {text.searchResultSuffix}
            </p>
          )}
        </div>
      </section>

      <div
        className="max-w-container mx-auto px-6 grid md:grid-cols-[280px_1fr] gap-6"
        style={{ padding: "32px 24px" }}
      >
        <aside className="relative z-20 bg-white rounded-2xl p-6 border border-gray-200 h-fit md:sticky md:top-[120px]">
          <h3 className="text-lg font-bold mb-5">{text.filterTitle}</h3>

          <div className="mb-6 pb-6 border-b border-gray-100">
            <div className="font-bold text-sm mb-3">{text.filterSearch}</div>
            <CountrySearchBox
              language={language}
              placeholder={text.searchPlaceholder}
              notFoundText={text.searchNotFound}
              fromLabel={text.from}
              variant="header"
            />
          </div>

          <div className="mb-6 pb-6 border-b border-gray-100">
            <div className="font-bold text-sm mb-3">{text.filterRegion}</div>
            <div className="flex flex-col gap-2">
              <Link
                href={buildFilterHref({ region: "" })}
                className={`flex items-center justify-between rounded-lg px-3 py-2 text-sm transition ${
                  !selectedRegion ? "bg-primary/10 text-primary" : "text-gray-700 hover:bg-gray-100"
                }`}
              >
                <span>{text.filterAllRegions}</span>
                <span className="text-xs">{keywordFilteredDestinations.length}</span>
              </Link>
              {regionOrder
                .filter((region) => (regionCounts[region] ?? 0) > 0)
                .map((region) => (
                  <Link
                    key={region}
                    href={buildFilterHref({ region })}
                    className={`flex items-center justify-between rounded-lg px-3 py-2 text-sm transition ${
                      selectedRegion === region ? "bg-primary/10 text-primary" : "text-gray-700 hover:bg-gray-100"
                    }`}
                  >
                    <span className="flex items-center gap-2">
                      {REGION_FLAG_CODES[region] ? (
                        <span className="inline-flex h-4.5 w-7 items-center justify-center overflow-hidden rounded-sm border border-slate-200 bg-slate-100" aria-hidden>
                          <Image
                            src={`https://flagcdn.com/w40/${REGION_FLAG_CODES[region]}.png`}
                            alt={region}
                            width={28}
                            height={18}
                            className="h-full w-full object-cover"
                          />
                        </span>
                      ) : (
                        <span className="inline-flex h-5 w-5 items-center justify-center rounded-full bg-slate-100 text-xs" aria-hidden>
                          {REGION_BADGES[region]}
                        </span>
                      )}
                      {displayRegionName(region)}
                    </span>
                    <span className="text-xs">{regionCounts[region]}</span>
                  </Link>
                ))}
            </div>
          </div>

            {(selectedRegion || keywordRaw) && (
            <Link
              href="/esim-du-lich"
              className="w-full inline-flex items-center justify-center bg-gray-50 text-gray-700 py-3 rounded-[10px] font-bold text-sm border border-gray-200"
            >
              {text.clearFilters}
            </Link>
          )}
        </aside>

        <main>
          {orderedByRegionEntries.map(([region, list]) => (
            <section key={region ?? "__no-region__"} style={{ marginBottom: "48px" }}>
              {region && <h2 className="section-title mb-6">{displayRegionName(region)}</h2>}
              <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 xl:grid-cols-5 gap-4">
                {list.map((d) => {
                  const flagUrl = getFlagUrl(d);
                  return (
                  <Link
                    key={d.slug}
                    href={`/esim-du-lich/${d.slug}`}
                    className="bg-white text-center text-navy no-underline transition-all duration-200 hover:-translate-y-0.5 hover:border-primary"
                    style={{
                      border: "1px solid #E2E8F0",
                      borderRadius: "16px",
                      padding: "20px",
                    }}
                  >
                    {flagUrl ? (
                      <div
                        className="relative bg-gray-100 mx-auto flex items-center justify-center overflow-hidden border border-slate-200"
                        style={{
                          width: "56px",
                          height: "38px",
                          borderRadius: "8px",
                          marginBottom: "12px",
                        }}
                      >
                        <Image
                          src={flagUrl}
                          alt={d.name}
                          width={56}
                          height={38}
                          loading="lazy"
                          className="h-full w-full object-cover"
                        />
                      </div>
                    ) : (
                      <div
                        className="bg-gray-100 mx-auto flex items-center justify-center text-2xl"
                        style={{
                          width: "56px",
                          height: "38px",
                          borderRadius: "8px",
                          marginBottom: "12px",
                        }}
                        aria-hidden
                      >
                        🌍
                      </div>
                    )}
                    <div className="font-bold mb-1" style={{ fontSize: "15px" }}>{displayCountryName(d)}</div>
                    <div className="text-primary font-semibold" style={{ fontSize: "13px" }}>
                      {d.startingPrice > 0 ? `${text.from} ${d.startingPrice.toLocaleString("vi-VN")}đ` : ""}
                    </div>
                    {d.packageCount > 0 && (
                      <div className="text-gray-500" style={{ fontSize: "12px", marginTop: "4px" }}>
                        {d.packageCount} {text.packagesAvailable}
                      </div>
                    )}
                    {d.bestseller && (
                      <span
                        className="inline-block font-bold mt-1.5"
                        style={{
                          background: "#FEF3C7",
                          color: "#92400E",
                          padding: "2px 8px",
                          borderRadius: "4px",
                          fontSize: "10px",
                        }}
                      >
                        {text.bestSeller}
                      </span>
                    )}
                  </Link>
                  );
                })}
              </div>
            </section>
          ))}

          {filteredDestinations.length === 0 && (
            <div
              className="bg-white text-center"
              style={{ border: "1px solid #E2E8F0", borderRadius: "16px", padding: "48px 24px" }}
            >
              <div style={{ fontSize: "40px", marginBottom: "12px" }}>🌏</div>
              <h2 className="text-navy font-bold" style={{ fontSize: "20px", marginBottom: "8px" }}>
                {text.emptyTitle}
              </h2>
              <p className="text-gray-500" style={{ fontSize: "14px" }}>
                {text.emptyDescription}
              </p>
            </div>
          )}
        </main>
      </div>
    </>
  );
}
