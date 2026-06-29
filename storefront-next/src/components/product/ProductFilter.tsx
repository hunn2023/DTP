"use client";

import { Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useCallback } from "react";
import Icon from "@/components/ui/Icon";

const categories = [
  { id: "", name: "Tất cả" },
  { id: "esim", name: "eSIM Du lịch" },
  { id: "the-nap", name: "Thẻ Viễn thông" },
  { id: "the-game", name: "Thẻ Game" },
  { id: "data", name: "Data 4G/5G" },
];

export default function ProductFilter() {
  return (
    <Suspense fallback={<div className="min-h-[100px]" />}>
      <ProductFilterInner />
    </Suspense>
  );
}

function ProductFilterInner() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const currentCategory = searchParams.get("categoryId") || "";
  const minPrice = searchParams.get("minPrice") || "";
  const maxPrice = searchParams.get("maxPrice") || "";

  const updateParams = useCallback(
    (updates: Record<string, string>) => {
      const params = new URLSearchParams(searchParams.toString());
      Object.entries(updates).forEach(([k, v]) => {
        if (v) params.set(k, v);
        else params.delete(k);
      });
      params.delete("pageIndex");
      router.push(`/esim-du-lich?${params.toString()}`);
    },
    [router, searchParams]
  );

  return (
    <aside className="space-y-6">
      {/* Category */}
      <div>
        <h4 className="flex items-center gap-2 mb-3 text-sm">
          <Icon icon="filter" /> Danh mục
        </h4>
        <ul className="space-y-1">
          {categories.map((cat) => (
            <li key={cat.id}>
              <button
                onClick={() => updateParams({ categoryId: cat.id })}
                className={`w-full text-left px-3 py-2 rounded-lg text-sm transition ${
                  currentCategory === cat.id
                    ? "bg-primary text-white font-semibold"
                    : "text-gray-700 hover:bg-gray-100"
                }`}
              >
                {cat.name}
              </button>
            </li>
          ))}
        </ul>
      </div>

      {/* Price range */}
      <div>
        <h4 className="flex items-center gap-2 mb-3 text-sm">
          <Icon icon="tag" /> Khoảng giá
        </h4>
        <div className="flex gap-2 items-center">
          <input
            type="number"
            placeholder="Từ"
            defaultValue={minPrice}
            onBlur={(e) => updateParams({ minPrice: e.target.value })}
            className="input py-2 text-xs"
          />
          <span className="text-gray-400">-</span>
          <input
            type="number"
            placeholder="Đến"
            defaultValue={maxPrice}
            onBlur={(e) => updateParams({ maxPrice: e.target.value })}
            className="input py-2 text-xs"
          />
        </div>
      </div>
    </aside>
  );
}
