"use client";

import type { EsimPackage, PackageQuickTag } from "@/types/esim";
import { getPackageCountByQuickTag } from "@/lib/api/esimApi";
import { useLanguage } from "@/hooks/useLanguage";

const pillConfig: Array<{ key: PackageQuickTag | "all"; icon: string }> = [
  { key: "all", icon: "⚡" },
  { key: "bestseller", icon: "🔥" },
  { key: "cheap", icon: "💰" },
  { key: "5g", icon: "🚀" },
  { key: "unlimited", icon: "♾️" },
  { key: "phone", icon: "📞" },
];

interface QuickPillsProps {
  packages: EsimPackage[];
  activeTag: PackageQuickTag | "all";
  onSelect: (tag: PackageQuickTag | "all") => void;
}

export default function QuickPills({ packages, activeTag, onSelect }: QuickPillsProps) {
  const { language } = useLanguage();
  const counts = getPackageCountByQuickTag(packages);

  const labels: Record<PackageQuickTag | "all", string> = {
    all: language === "vi" ? "Tất cả" : "All",
    bestseller: language === "vi" ? "Bán chạy" : "Best seller",
    cheap: language === "vi" ? "Giá rẻ" : "Budget",
    "5g": "5G",
    unlimited: language === "vi" ? "Không giới hạn" : "Unlimited",
    phone: language === "vi" ? "Có SĐT gọi" : "With phone number",
    hotspot: language === "vi" ? "Chia sẻ mạng" : "Hotspot",
  };

  return (
    <div className="mb-5">
      <div className="flex gap-2 flex-wrap">
        {pillConfig.map((pill) => {
          const isActive = activeTag === pill.key;
          const count = pill.key === "all" ? packages.length : counts[pill.key];

          return (
            <button
              key={pill.key}
              type="button"
              onClick={() => {
                onSelect(pill.key);
              }}
              className={`px-4 py-2 rounded-3xl text-[13px] font-semibold cursor-pointer flex items-center gap-1.5 border-[1.5px] transition ${
                isActive
                  ? "bg-primary text-white border-primary"
                  : "bg-white text-inherit border-gray-200 hover:border-primary hover:text-primary"
              }`}
            >
              {pill.icon} {labels[pill.key]}
              <span className={`px-1.5 rounded text-[11px] ${isActive ? "bg-white/25" : "bg-gray-100 text-gray-700"}`}>
                {count}
              </span>
            </button>
          );
        })}
      </div>
    </div>
  );
}
