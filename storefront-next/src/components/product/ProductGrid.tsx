"use client";

import { Suspense } from "react";
import Link from "next/link";
import Image from "next/image";
import { useRouter, useSearchParams } from "next/navigation";
import Icon from "@/components/ui/Icon";

export interface Product {
  id: string;
  name: string;
  slug: string;
  image: string;
  price: number;
  originalPrice?: number;
  badge?: string;
  category: string;
}

interface Props {
  products: Product[];
  totalPages: number;
  loading?: boolean;
}

function formatPrice(price: number) {
  return price.toLocaleString("vi-VN") + "đ";
}

export default function ProductGrid({ products, totalPages, loading }: Props) {
  return (
    <Suspense fallback={<div className="min-h-[400px]" />}>
      <ProductGridInner products={products} totalPages={totalPages} loading={loading} />
    </Suspense>
  );
}

function ProductGridInner({ products, totalPages, loading }: Props) {
  const router = useRouter();
  const searchParams = useSearchParams();
  const currentPage = Number(searchParams.get("pageIndex") || "1");

  const goToPage = (page: number) => {
    const params = new URLSearchParams(searchParams.toString());
    params.set("pageIndex", String(page));
    router.push(`/esim-du-lich?${params.toString()}`);
  };

  if (loading) {
    return (
      <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
        {Array.from({ length: 6 }).map((_, i) => (
          <div key={i} className="card animate-pulse">
            <div className="aspect-[4/3] bg-gray-200" />
            <div className="p-4 space-y-3">
              <div className="h-4 bg-gray-200 rounded w-3/4" />
              <div className="h-5 bg-gray-200 rounded w-1/2" />
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (!products.length) {
    return (
      <div className="text-center py-16">
        <Icon icon="search" className="text-4xl text-gray-300 mb-4" />
        <p className="text-gray-500 font-medium">Không tìm thấy sản phẩm nào</p>
        <p className="text-gray-400 text-sm mt-1">Thử thay đổi bộ lọc hoặc từ khóa tìm kiếm</p>
      </div>
    );
  }

  return (
    <div>
      <div className="grid grid-cols-2 md:grid-cols-3 gap-4 md:gap-6">
        {products.map((p) => (
          <Link key={p.id} href={`/esim-du-lich/${p.slug}`} className="product-card">
            <div className="relative">
              <Image
                src={p.image}
                alt={p.name}
                fill
                sizes="(max-width: 768px) 50vw, (max-width: 1024px) 33vw, 25vw"
                className="product-card-img bg-gray-100"
              />
              {p.badge && (
                <span className="absolute top-2 left-2 bg-danger text-white text-xs font-bold px-2 py-0.5 rounded">
                  {p.badge}
                </span>
              )}
            </div>
            <div className="product-card-body">
              <span className="text-xs text-gray-500">{p.category}</span>
              <h3 className="product-card-title">{p.name}</h3>
              <div className="flex items-center gap-2 mt-auto">
                <span className="product-card-price">{formatPrice(p.price)}</span>
                {p.originalPrice && (
                  <span className="text-gray-400 text-xs line-through">{formatPrice(p.originalPrice)}</span>
                )}
              </div>
            </div>
          </Link>
        ))}
      </div>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex justify-center items-center gap-2 mt-8">
          <button
            onClick={() => goToPage(currentPage - 1)}
            disabled={currentPage <= 1}
            className="btn-outline btn-sm disabled:opacity-40 disabled:cursor-not-allowed"
          >
            <Icon icon="chevron-left" />
          </button>
          {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
            <button
              key={page}
              onClick={() => goToPage(page)}
              className={`w-9 h-9 rounded-lg text-sm font-medium transition ${
                page === currentPage
                  ? "bg-primary text-white"
                  : "text-gray-600 hover:bg-gray-100"
              }`}
            >
              {page}
            </button>
          ))}
          <button
            onClick={() => goToPage(currentPage + 1)}
            disabled={currentPage >= totalPages}
            className="btn-outline btn-sm disabled:opacity-40 disabled:cursor-not-allowed"
          >
            <Icon icon="chevron-right" />
          </button>
        </div>
      )}
    </div>
  );
}
