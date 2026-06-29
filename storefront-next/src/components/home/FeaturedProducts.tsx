"use client";

import Link from "next/link";
import Icon from "@/components/ui/Icon";
import ProductCard from "@/components/product/ProductCard";
import SectionHeading from "@/components/ui/SectionHeading";
import { mockProducts } from "@/lib/mock-products";
import { useCartStore } from "@/lib/cartStore";
import { Product } from "@/types/product";

export default function FeaturedProducts() {
  const addToCart = useCartStore((s) => s.addToCart);

  const handleAddToCart = (product: Product) => {
    addToCart({
      id: product.id,
      name: product.name,
      slug: product.slug,
      image: product.image,
      price: product.price,
      quantity: 1,
      stock: product.inStock ? 99 : 0,
    });
  };

  return (
    <section className="py-12 md:py-16 bg-gray-50">
      <div className="max-w-container mx-auto px-4 md:px-6">
        <div className="flex items-end justify-between mb-8">
          <SectionHeading eyebrow="Bán chạy" title="Sản phẩm" highlight="nổi bật" align="left" />
          <Link href="/esim-du-lich" className="text-primary text-sm font-semibold hover:underline flex items-center gap-1 flex-shrink-0 mb-1">
            Xem tất cả <Icon icon="arrow-right" />
          </Link>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 md:gap-6">
          {mockProducts.slice(0, 8).map((product) => (
            <ProductCard key={product.id} product={product} onAddToCart={handleAddToCart} />
          ))}
        </div>
      </div>
    </section>
  );
}
