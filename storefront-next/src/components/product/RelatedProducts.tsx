import Link from "next/link";
import { Product } from "@/types/product";
import { ProductCard } from "@/components/product";

interface Props {
  products: Product[];
}

export default function RelatedProducts({ products }: Props) {
  if (products.length === 0) return null;

  return (
    <section className="mt-12 pt-8 border-t border-gray-100">
      <h2 className="text-xl font-bold text-navy mb-6">Sản phẩm liên quan</h2>
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 md:gap-6">
        {products.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </section>
  );
}
