"use client";

import { useEffect, useMemo, useState } from "react";
import Icon from "@/components/ui/Icon";
import { useLanguage } from "@/hooks/useLanguage";
import { getDataRangeForPackage } from "@/lib/api/esimApi";
import type { EsimDataRange, EsimPackage } from "@/types/esim";

const dataRangeLabels: Record<EsimDataRange, string> = {
  "1-3": "1 - 3 GB",
  "5": "5 GB",
  "10": "10 GB",
  "20": "20 GB trở lên",
  unlimited: "Không giới hạn",
};

// Feature filter options derived dynamically from package data

interface SidebarFilters {
  days: number[];
  dataRanges: EsimDataRange[];
  minPrice?: number;
  maxPrice?: number;
}

interface SidebarProps {
  packages: EsimPackage[];
  appliedFilters: SidebarFilters;
  onApply: (filters: SidebarFilters) => void;
  onReset: () => void;
}

export default function Sidebar({ packages, appliedFilters, onApply, onReset }: SidebarProps) {
  const { language } = useLanguage();
  const [panelOpen, setPanelOpen] = useState(false);
  const [days, setDays] = useState<number[]>(appliedFilters.days);
  const [dataRanges, setDataRanges] = useState<EsimDataRange[]>(appliedFilters.dataRanges);
  const [minPrice, setMinPrice] = useState(appliedFilters.minPrice?.toString() ?? "");
  const [maxPrice, setMaxPrice] = useState(appliedFilters.maxPrice?.toString() ?? "");

  useEffect(() => {
    setDays(appliedFilters.days);
    setDataRanges(appliedFilters.dataRanges);
    setMinPrice(appliedFilters.minPrice?.toString() ?? "");
    setMaxPrice(appliedFilters.maxPrice?.toString() ?? "");
  }, [appliedFilters]);

  const dayOptions = useMemo(
    () => Array.from(new Set(packages.map((pkg) => pkg.days))).sort((a, b) => a - b),
    [packages]
  );

  const dataOptions = useMemo(() => {
    const counts = packages.reduce<Record<EsimDataRange, number>>(
      (acc, pkg) => {
        acc[getDataRangeForPackage(pkg.dataGB)] += 1;
        return acc;
      },
      { "1-3": 0, "5": 0, "10": 0, "20": 0, unlimited: 0 }
    );

    return (Object.keys(counts) as EsimDataRange[])
      .filter((key) => counts[key] > 0)
      .map((key) => ({ key, label: dataRangeLabels[key], count: counts[key] }));
  }, [packages]);


  const toggleArrayValue = <T,>(current: T[], value: T) =>
    current.includes(value) ? current.filter((item) => item !== value) : [...current, value];

  const text = {
    filterPackages: language === "vi" ? "Lọc gói" : "Filter packages",
    usageDays: language === "vi" ? "Số ngày sử dụng" : "Usage days",
    daysSuffix: language === "vi" ? "ngày" : "days",
    dataAmount: language === "vi" ? "Dung lượng data" : "Data allowance",
    priceRange: language === "vi" ? "Khoảng giá (VNĐ)" : "Price range (VND)",
    from: language === "vi" ? "Từ" : "From",
    to: language === "vi" ? "Đến" : "To",
    features: language === "vi" ? "Tính năng" : "Features",
    applyFilters: language === "vi" ? "Áp dụng bộ lọc" : "Apply filters",
    clearFilters: language === "vi" ? "Xóa bộ lọc" : "Clear filters",
  };

  const dataLabel = (range: EsimDataRange) => {
    if (language === "vi") return dataRangeLabels[range];

    const map: Record<EsimDataRange, string> = {
      "1-3": "1 - 3 GB",
      "5": "5 GB",
      "10": "10 GB",
      "20": "20 GB and up",
      unlimited: "Unlimited",
    };

    return map[range];
  };

  const handleApply = () => {
    onApply({
      days,
      dataRanges,
      minPrice: minPrice ? Number(minPrice.replace(/\D/g, "")) : undefined,
      maxPrice: maxPrice ? Number(maxPrice.replace(/\D/g, "")) : undefined,
    });
    setPanelOpen(false);
  };

  const hasActiveFilters =
    days.length > 0 ||
    dataRanges.length > 0 ||
    Boolean(minPrice.trim()) ||
    Boolean(maxPrice.trim());

  return (
    <aside className="relative z-20 bg-white rounded-2xl p-3.5 md:p-4 border border-gray-200 h-fit md:sticky md:top-[2px]">
      <button
        type="button"
        onClick={() => setPanelOpen((current) => !current)}
        className="w-full flex items-center justify-between gap-2 text-left"
      >
        <span className="inline-flex items-center gap-2 text-base font-bold">
          <Icon icon="filter" /> {text.filterPackages}
          {hasActiveFilters && <span className="text-xs text-primary font-semibold">•</span>}
        </span>
        <Icon icon={panelOpen ? "chevron-up" : "chevron-down"} className="text-xs text-gray-500" />
      </button>

      <div className={`${panelOpen ? "block" : "hidden"} mt-3 md:mt-4`}>
      <div className="mb-4 pb-4 border-b border-gray-100">
        <div className="font-bold text-sm mb-3">{text.usageDays}</div>
        <div className="flex flex-wrap gap-2">
          {dayOptions.map((day) => (
            <button
              key={day}
              onClick={() => setDays((current) => toggleArrayValue(current, day))}
              className={`px-2.5 py-1.5 border-[1.5px] rounded-full text-xs cursor-pointer font-medium transition ${
                days.includes(day)
                  ? "bg-primary text-white border-primary"
                  : "bg-white text-gray-700 border-gray-200"
              }`}
            >
              {day} {text.daysSuffix}
            </button>
          ))}
        </div>
      </div>

      <div className="mb-4 pb-4 border-b border-gray-100">
        <div className="font-bold text-sm mb-3">{text.dataAmount}</div>
        <div className="flex flex-col gap-2.5">
          {dataOptions.map((opt) => (
            <label key={opt.label} className="flex items-center gap-2.5 text-sm cursor-pointer">
              <input
                type="checkbox"
                checked={dataRanges.includes(opt.key)}
                onChange={() => setDataRanges((current) => toggleArrayValue(current, opt.key))}
                className="w-4 h-4 accent-primary"
              />
              {dataLabel(opt.key)}
              <span className="ml-auto text-gray-500 text-xs">{opt.count}</span>
            </label>
          ))}
        </div>
      </div>

      <div className="mb-4 pb-4 border-b border-gray-100">
        <div className="font-bold text-sm mb-3">{text.priceRange}</div>
        <div className="grid grid-cols-2 gap-2 mt-1.5">
          <input
            type="text"
            inputMode="numeric"
            placeholder={text.from}
            value={minPrice}
            onChange={(event) => setMinPrice(event.target.value)}
            className="w-full min-w-0 py-1.5 px-2.5 border-[1.5px] border-gray-200 rounded-lg text-[13px] font-sans"
          />
          <input
            type="text"
            inputMode="numeric"
            placeholder={text.to}
            value={maxPrice}
            onChange={(event) => setMaxPrice(event.target.value)}
            className="w-full min-w-0 py-1.5 px-2.5 border-[1.5px] border-gray-200 rounded-lg text-[13px] font-sans"
          />
        </div>
      </div>

      <div className="flex flex-col gap-2">
        <button
          type="button"
          onClick={handleApply}
          className="w-full gradient-primary text-white py-2.5 rounded-[10px] font-bold text-sm flex items-center justify-center gap-2 cursor-pointer"
        >
          <Icon icon="check-circle" /> {text.applyFilters}
        </button>
        <button
          type="button"
          onClick={() => {
            onReset();
            setPanelOpen(false);
          }}
          className="w-full bg-gray-50 text-gray-700 py-2.5 rounded-[10px] font-bold text-sm border border-gray-200 cursor-pointer"
        >
          {text.clearFilters}
        </button>
      </div>
      </div>
    </aside>
  );
}
