"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import Icon from "@/components/ui/Icon";
import { useCartStore } from "@/lib/cartStore";
import { useCartAnimation } from "@/components/ui/CartAnimation";
import { useLanguage } from "@/hooks/useLanguage";
import { filterEsimPackages } from "@/lib/api/esimApi";
import type { EsimCountryDetail, EsimPackageFilters, PackageQuickTag } from "@/types/esim";
import Sidebar from "./Sidebar";
import QuickPills from "./QuickPills";
import PackageCard from "./PackageCard";
import InfoTabs from "./InfoTabs";
import type { ProductContent, ProductFaq } from "@/types/productContent";

const INITIAL_FILTERS: EsimPackageFilters = {
  days: [],
  dataRanges: [],
  featureTags: [],
  quickTag: "all",
  sort: "recommended",
};

type ViewMode = "grid" | "list";
type GbGroupKey = "1" | "2" | "3" | "5" | "10" | "15" | "20" | "unlimited";

const GB_GROUP_ORDER: GbGroupKey[] = ["1", "2", "3", "5", "10", "15", "20", "unlimited"];

function getGbGroupKey(dataGB: number | null): GbGroupKey {
  if (dataGB === null) return "unlimited";
  if (dataGB <= 1) return "1";
  if (dataGB <= 2) return "2";
  if (dataGB <= 3) return "3";
  if (dataGB <= 5) return "5";
  if (dataGB <= 10) return "10";
  if (dataGB <= 15) return "15";
  return "20";
}

interface EsimCountryBrowserProps {
  country: EsimCountryDetail;
  contents: ProductContent[];
  faqs: ProductFaq[];
}

export default function EsimCountryBrowser({ country, contents, faqs }: EsimCountryBrowserProps) {
  const addToCart = useCartStore((state) => state.addToCart);
  const setBuyNowItem = useCartStore((state) => state.setBuyNowItem);
  const router = useRouter();
  const { triggerFlyToCart } = useCartAnimation();
  const { language } = useLanguage();
  const [filters, setFilters] = useState<EsimPackageFilters>(INITIAL_FILTERS);
  const [viewMode, setViewMode] = useState<ViewMode>("grid");
  const [visibleCount, setVisibleCount] = useState(6);
  const [activeGbGroup, setActiveGbGroup] = useState<GbGroupKey | null>(null);
  const [isTransitioning, setIsTransitioning] = useState(false);
  const gbScrollRef = useRef<HTMLDivElement>(null);

  const filteredPackages = useMemo(
    () => filterEsimPackages(country.packages, filters),
    [country.packages, filters]
  );

  // Group packages by GB
  const gbGroups = useMemo(() => {
    const grouped: Record<GbGroupKey, EsimCountryDetail["packages"]> = {
      "1": [], "2": [], "3": [], "5": [], "10": [], "15": [], "20": [], unlimited: [],
    };
    for (const pkg of filteredPackages) {
      grouped[getGbGroupKey(pkg.dataGB)].push(pkg);
    }
    return GB_GROUP_ORDER.map((key) => ({ key, packages: grouped[key] }));
  }, [filteredPackages]);

  // If the selected GB group becomes empty (e.g. after a filter change), reset to "all"
  useEffect(() => {
    if (activeGbGroup !== null) {
      const hasPackages = gbGroups.some((g) => g.key === activeGbGroup && g.packages.length > 0);
      if (!hasPackages) {
        setActiveGbGroup(null);
        setVisibleCount(6);
      }
    }
  }, [gbGroups, activeGbGroup]);

  // null = show all packages; GbGroupKey = show only that group
  const activeGroupPackages = useMemo(
    () => activeGbGroup === null
      ? filteredPackages
      : gbGroups.find((g) => g.key === activeGbGroup)?.packages ?? [],
    [activeGbGroup, gbGroups, filteredPackages]
  );

  const activeGroupDays = useMemo(
    () => Array.from(new Set(activeGroupPackages.map((pkg) => pkg.days))).sort((a, b) => a - b),
    [activeGroupPackages]
  );

  const visiblePackages = activeGroupPackages.slice(0, visibleCount);

  const handleGbGroupChange = (key: GbGroupKey | null) => {
    if (key === activeGbGroup) return;
    setIsTransitioning(true);
    setTimeout(() => {
      setActiveGbGroup(key);
      setVisibleCount(6);
      setTimeout(() => setIsTransitioning(false), 20);
    }, 150);
  };

  const handleQuickTagChange = (quickTag: PackageQuickTag | "all") => {
    setFilters((current) => ({ ...current, quickTag }));
    setVisibleCount(6);
  };

  const handleSidebarApply = (
    nextFilters: Pick<EsimPackageFilters, "days" | "dataRanges" | "minPrice" | "maxPrice">
  ) => {
    setFilters((current) => ({ ...current, ...nextFilters, featureTags: [] }));
    setVisibleCount(6);
  };

  const handleReset = () => {
    setFilters(INITIAL_FILTERS);
    setActiveGbGroup(null);
    setVisibleCount(6);
  };

  const handleSortChange = (sort: EsimPackageFilters["sort"]) => {
    setFilters((current) => ({ ...current, sort }));
  };

  const buildCartItem = (pkg: EsimCountryDetail["packages"][number], quantity: number) => ({
    id: pkg.id,
    name: `${country.name} ${pkg.data} ${pkg.dataUnit} - ${pkg.days} ngày`,
    slug: pkg.slug,
    href: `/esim-du-lich/${country.slug}`,
    image: pkg.image,
    price: pkg.price,
    quantity,
    stock: pkg.stock,
    productId: pkg.productId,
    productVariantId: pkg.productVariantId,
    esimPackageId: pkg.esimPackageId || pkg.id,
    itemType: 1, // EsimPackage
  });

  const handleBuy = (pkg: EsimCountryDetail["packages"][number], quantity: number, triggerElement: HTMLElement | null) => {
    addToCart(buildCartItem(pkg, quantity));
    triggerFlyToCart(pkg.image, triggerElement);
  };

  const handleBuyNow = (pkg: EsimCountryDetail["packages"][number], quantity: number) => {
    setBuyNowItem(buildCartItem(pkg, quantity));
    router.push("/checkout?buyNow=1");
  };

  const text = {
    showing: language === "vi" ? "Hiển thị" : "Showing",
    inTotal: language === "vi" ? "trong tổng số" : "out of",
    packages: language === "vi" ? "gói" : "packages",
    sortBy: language === "vi" ? "Sắp xếp:" : "Sort by:",
    sortRecommended: language === "vi" ? "Phù hợp nhất" : "Recommended",
    sortPriceAsc: language === "vi" ? "Giá thấp đến cao" : "Price low to high",
    sortPriceDesc: language === "vi" ? "Giá cao đến thấp" : "Price high to low",
    sortBestSeller: language === "vi" ? "Bán chạy nhất" : "Best seller",
    sortRating: language === "vi" ? "Đánh giá cao nhất" : "Top rated",
    gridView: language === "vi" ? "Hiển thị dạng lưới" : "Grid view",
    listView: language === "vi" ? "Hiển thị dạng danh sách" : "List view",
    noPackageTitle: language === "vi" ? "Không tìm thấy gói phù hợp" : "No matching package found",
    noPackageDescription:
      language === "vi"
        ? "Thử nới bộ lọc hoặc chọn nhanh một nhóm gói khác."
        : "Try relaxing filters or picking another quick package group.",
    clearFilters: language === "vi" ? "Xóa bộ lọc" : "Clear filters",
    showMore: language === "vi" ? "Xem thêm" : "Show more",
    packageSuffix: language === "vi" ? "gói" : "packages",
    gbGroupBy: language === "vi" ? "Chọn dung lượng" : "Select data",
    allPackages: language === "vi" ? "Tất cả" : "All",
    unlimitedLabel: language === "vi" ? "Vô hạn" : "Unlimited",
    daysList: language === "vi" ? "Số ngày khả dụng" : "Available days",
    daysSuffix: language === "vi" ? "ngày" : "days",
  };

  const getGbLabel = (key: GbGroupKey) =>
    key === "unlimited" ? text.unlimitedLabel : key === "20" ? "20+ GB" : `${key} GB`;

  return (
    <div className="max-w-container mx-auto w-full overflow-x-hidden px-4 md:px-6 py-8 grid md:grid-cols-[280px_1fr] gap-6">
      <Sidebar
        packages={country.packages}
        appliedFilters={{
          days: filters.days,
          dataRanges: filters.dataRanges,
          minPrice: filters.minPrice,
          maxPrice: filters.maxPrice,
        }}
        onApply={handleSidebarApply}
        onReset={handleReset}
      />

      <main className="min-w-0">
        <div className="hidden md:block">
          <QuickPills
            packages={country.packages}
            activeTag={filters.quickTag}
            onSelect={handleQuickTagChange}
          />
        </div>

        {/* GB Group Tabs — horizontally scrollable on mobile */}
        {gbGroups.some((g) => g.packages.length > 0) && (
          <div className="mb-5 rounded-2xl border border-slate-200 bg-white p-3 md:p-4">
            <div className="text-[11px] font-semibold uppercase tracking-wide text-slate-500 mb-2">
              {text.gbGroupBy}
            </div>

            {/* Scroll container — fixed width, no page overflow */}
            <div
              ref={gbScrollRef}
              className="relative -mx-3 px-3 md:-mx-4 md:px-4"
            >
              <div
                className="flex gap-2 overflow-x-auto pb-2 snap-x snap-mandatory hide-scrollbar"
                style={{ WebkitOverflowScrolling: "touch" }}
              >
                {/* "Tất cả" — show all packages */}
                <button
                  type="button"
                  onClick={() => handleGbGroupChange(null)}
                  className={`snap-start shrink-0 whitespace-nowrap px-4 py-2.5 rounded-full text-sm font-semibold transition-all duration-200 ${
                    activeGbGroup === null
                      ? "bg-primary text-white shadow-md"
                      : "bg-slate-50 text-slate-700 border border-slate-200 active:scale-95"
                  }`}
                >
                  {text.allPackages}
                </button>

                {gbGroups.map((group) => {
                  const isActive = group.key === activeGbGroup;
                  const isEmpty = group.packages.length === 0;
                  return (
                    <button
                      key={group.key}
                      type="button"
                      onClick={() => !isEmpty && handleGbGroupChange(group.key)}
                      disabled={isEmpty}
                      className={`snap-start shrink-0 whitespace-nowrap px-4 py-2.5 rounded-full text-sm font-semibold transition-all duration-200 ${
                        isActive
                          ? "bg-primary text-white shadow-md"
                          : isEmpty
                            ? "bg-slate-100 text-slate-400 cursor-not-allowed"
                            : "bg-slate-50 text-slate-700 border border-slate-200 active:scale-95"
                      }`}
                    >
                      {getGbLabel(group.key)}
                    </button>
                  );
                })}
              </div>
            </div>

            {/* Available days for active group */}
            {activeGroupDays.length > 0 && (
              <div className="mt-2.5 flex flex-wrap items-center gap-1.5 text-sm">
                <span className="font-semibold text-navy text-xs">{text.daysList}:</span>
                {activeGroupDays.map((day) => (
                  <span key={day} className="rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-600">
                    {day} {text.daysSuffix}
                  </span>
                ))}
              </div>
            )}
          </div>
        )}

        <div
          className="bg-white flex flex-col gap-4 lg:flex-row lg:justify-between lg:items-center"
          style={{
            padding: "16px 20px",
            borderRadius: "12px",
            border: "1px solid #E2E8F0",
            marginBottom: "20px",
          }}
        >
          <div className="hidden md:block text-gray-700" style={{ fontSize: "14px" }}>
            {text.showing} <b className="text-navy">{visiblePackages.length === 0 ? 0 : 1}-{visiblePackages.length}</b> {text.inTotal}{" "}
            <b className="text-navy">{activeGroupPackages.length}</b> {text.packages} {country.name}
          </div>

          <div className="flex flex-wrap items-center gap-3" style={{ fontSize: "14px" }}>
            <span>{text.sortBy}</span>
            <select
              value={filters.sort}
              onChange={(event) => handleSortChange(event.target.value as EsimPackageFilters["sort"])}
              className="font-sans bg-white cursor-pointer"
              style={{
                padding: "8px 32px 8px 12px",
                border: "1.5px solid #E2E8F0",
                borderRadius: "8px",
                fontSize: "14px",
              }}
            >
              <option value="recommended">{text.sortRecommended}</option>
              <option value="price_asc">{text.sortPriceAsc}</option>
              <option value="price_desc">{text.sortPriceDesc}</option>
              <option value="bestseller">{text.sortBestSeller}</option>
              <option value="rating">{text.sortRating}</option>
            </select>
            <div className="hidden md:flex gap-1">
              <button
                type="button"
                onClick={() => setViewMode("grid")}
                className={`cursor-pointer flex items-center justify-center ${
                  viewMode === "grid" ? "bg-primary text-white" : "bg-white text-gray-500"
                }`}
                style={{
                  width: "36px",
                  height: "36px",
                  border: viewMode === "grid" ? "1.5px solid #0066FF" : "1.5px solid #E2E8F0",
                  borderRadius: "8px",
                }}
                aria-label={text.gridView}
              >
                <Icon icon="th-large" />
              </button>
              <button
                type="button"
                onClick={() => setViewMode("list")}
                className={`cursor-pointer flex items-center justify-center ${
                  viewMode === "list" ? "bg-primary text-white" : "bg-white text-gray-500"
                }`}
                style={{
                  width: "36px",
                  height: "36px",
                  border: viewMode === "list" ? "1.5px solid #0066FF" : "1.5px solid #E2E8F0",
                  borderRadius: "8px",
                }}
                aria-label={text.listView}
              >
                <Icon icon="list" />
              </button>
            </div>
          </div>
        </div>

        {/* Package cards with transition */}
        <div className={`transition-opacity duration-200 ${isTransitioning ? "opacity-0" : "opacity-100"}`}>
          {visiblePackages.length > 0 ? (
            <div className={`grid gap-4 ${viewMode === "grid" ? "grid-cols-1 lg:grid-cols-2" : "grid-cols-1"}`}>
              {visiblePackages.map((pkg) => (
                <PackageCard key={pkg.id} pkg={pkg} onBuy={handleBuy} onBuyNow={handleBuyNow} />
              ))}
            </div>
          ) : (
            <div
              className="bg-white text-center"
              style={{ borderRadius: "16px", border: "1px solid #E2E8F0", padding: "48px 24px" }}
            >
              <div className="text-4xl mb-3">🧭</div>
              <h3 className="text-navy font-bold mb-2">{text.noPackageTitle}</h3>
              <p className="text-gray-500 mb-4">{text.noPackageDescription}</p>
              <button
                type="button"
                onClick={handleReset}
                className="btn-outline"
              >
                {text.clearFilters}
              </button>
            </div>
          )}
        </div>

        {visiblePackages.length < activeGroupPackages.length && (
          <div className="text-center" style={{ marginTop: "32px" }}>
            <button
              type="button"
              onClick={() => setVisibleCount((current) => current + 6)}
              className="gradient-primary text-white font-bold cursor-pointer inline-flex items-center gap-2"
              style={{
                padding: "12px 24px",
                borderRadius: "10px",
                fontSize: "14px",
              }}
            >
              <Icon icon="plus" /> {text.showMore} {Math.min(6, activeGroupPackages.length - visiblePackages.length)} {text.packageSuffix}
            </button>
          </div>
        )}

        <InfoTabs contents={contents} faqs={faqs} />
      </main>
    </div>
  );
}
