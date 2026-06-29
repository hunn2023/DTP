"use client";

import { Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useCallback } from "react";
import { Product } from "@/types/product";
import ProductGrid from "@/components/product/ProductGrid";
import { ProductSort } from "@/components/product";
import Icon from "@/components/ui/Icon";

interface Props {
  products: Product[];
  totalPages: number;
  categorySlug: string;
}

export default function CategoryProducts({ products, totalPages, categorySlug }: Props) {
  return (
    <Suspense fallback={<div className="min-h-[200px]" />}>
      <CategoryProductsInner products={products} totalPages={totalPages} categorySlug={categorySlug} />
    </Suspense>
  );
}

function CategoryProductsInner({ products, totalPages, categorySlug }: Props) {
  const router = useRouter();
  const searchParams = useSearchParams();

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
      router.push(`/categories/${categorySlug}?${params.toString()}`);
    },
    [router, searchParams, categorySlug]
  );

  return (
    <div>
      {/* Filter bar */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6 pb-4 border-b border-gray-100">
        <div className="flex items-center gap-3">
          <div className="flex items-center gap-2">
            <Icon icon="tag" className="text-gray-400 text-sm" />
            <input
              type="number"
              placeholder="Giá từ"
              defaultValue={minPrice}
              onBlur={(e) => updateParams({ minPrice: e.target.value })}
              className="input py-2 text-xs w-28"
            />
            <span className="text-gray-400">-</span>
            <input
              type="number"
              placeholder="Giá đến"
              defaultValue={maxPrice}
              onBlur={(e) => updateParams({ maxPrice: e.target.value })}
              className="input py-2 text-xs w-28"
            />
          </div>
        </div>
        <ProductSort />
      </div>

      {/* Product grid or empty state */}
      {products.length > 0 ? (
        <ProductGrid products={products} totalPages={totalPages} />
      ) : (
        <EmptyState />
      )}
    </div>
  );
}

function EmptyState() {
  return (
    <div className="text-center py-16">
      <Icon icon="box-open" className="text-5xl text-gray-300 mb-4" />
      <p className="text-gray-500 font-medium text-lg">Chưa có sản phẩm nào</p>
      <p className="text-gray-400 text-sm mt-2">
        Danh mục này hiện chưa có sản phẩm. Vui lòng quay lại sau.
      </p>
    </div>
  );
}
