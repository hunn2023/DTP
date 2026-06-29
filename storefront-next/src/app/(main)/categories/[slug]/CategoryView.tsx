import { Suspense } from "react";
import Breadcrumb from "@/components/ui/Breadcrumb";
import type { Category, Product } from "@/types/product";
import CategoryProducts from "./CategoryProducts";

interface CategoryViewProps {
  category: Category;
  products: Product[];
  totalPages: number;
  slug: string;
}

export default function CategoryView({ category, products, totalPages, slug }: CategoryViewProps) {
  return (
    <>
      <Breadcrumb
        items={[
          { label: "Danh mục", href: "/esim-du-lich" },
          { label: category.name },
        ]}
      />

      <section className="max-w-container mx-auto px-4 md:px-6 py-6 md:py-10">
        <div className="mb-8">
          <h1 className="text-2xl md:text-3xl font-bold text-navy">{category.name}</h1>
          {category.description && (
            <p className="text-gray-500 mt-2 max-w-2xl">{category.description}</p>
          )}
        </div>

        <Suspense fallback={<div className="min-h-[400px]" />}>
          <CategoryProducts products={products} totalPages={totalPages} categorySlug={slug} />
        </Suspense>
      </section>
    </>
  );
}
