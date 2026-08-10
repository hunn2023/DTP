"use client";

import { useLanguage } from "@/hooks/useLanguage";
import type { EsimPackage } from "@/types/esim";

export type CoverageScope = "all" | "single" | "regional" | "global";

export function getPackageCoverageScope(pkg: EsimPackage): Exclude<CoverageScope, "all"> {
  const type = pkg.coverageType?.trim().toLowerCase() ?? "";
  if (type.includes("global") || type.includes("world")) return "global";
  if (type.includes("regional") || type.includes("region") || (pkg.coverages?.length ?? 0) > 1) {
    return "regional";
  }
  return "single";
}

interface CoverageScopePillsProps {
  packages: EsimPackage[];
  activeScope: CoverageScope;
  onSelect: (scope: CoverageScope) => void;
}

export default function CoverageScopePills({ packages, activeScope, onSelect }: CoverageScopePillsProps) {
  const { language } = useLanguage();
  const counts = packages.reduce(
    (result, pkg) => {
      result[getPackageCoverageScope(pkg)] += 1;
      return result;
    },
    { single: 0, regional: 0, global: 0 }
  );

  const options: Array<{ key: CoverageScope; icon: string; label: string; count: number }> = [
    { key: "all", icon: "🌐", label: language === "vi" ? "Tất cả phạm vi" : "All coverage", count: packages.length },
    { key: "single", icon: "📍", label: language === "vi" ? "Một quốc gia" : "Single country", count: counts.single },
    { key: "regional", icon: "🗺️", label: language === "vi" ? "Nhiều quốc gia" : "Multi-country", count: counts.regional },
    { key: "global", icon: "🌍", label: language === "vi" ? "Toàn cầu" : "Global", count: counts.global },
  ];

  return (
    <div className="mb-5">
      <div className="mb-2 text-[11px] font-semibold uppercase tracking-wide text-slate-500">
        {language === "vi" ? "Phạm vi phủ sóng" : "Coverage scope"}
      </div>
      <div className="flex flex-wrap gap-2 pb-2">
        {options.filter((option) => option.key === "all" || option.count > 0).map((option) => {
          const isActive = activeScope === option.key;
          return (
            <button
              key={option.key}
              type="button"
              onClick={() => onSelect(option.key)}
              aria-pressed={isActive}
              className={`max-w-full whitespace-nowrap rounded-3xl border-[1.5px] px-4 py-2 text-[13px] font-semibold transition ${
                isActive
                  ? "border-primary bg-primary text-white"
                  : "border-gray-200 bg-white text-inherit hover:border-primary hover:text-primary"
              }`}
            >
              <span className="mr-1.5">{option.icon}</span>{option.label}
              <span className={`ml-1.5 rounded px-1.5 text-[11px] ${isActive ? "bg-white/25" : "bg-gray-100 text-gray-700"}`}>
                {option.count}
              </span>
            </button>
          );
        })}
      </div>
    </div>
  );
}
