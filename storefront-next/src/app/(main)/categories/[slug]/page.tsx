import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { SITE } from "@/lib/constants";
import { getCategoryBySlug, getProductsByCategory } from "@/lib/api/categoriesApi";
import CategoryView from "./CategoryView";

interface PageProps {
  params: { slug: string };
}

export const revalidate = 60;

export async function generateStaticParams() {
  return [
    { slug: "esim" },
    { slug: "the-nap" },
    { slug: "the-game" },
    { slug: "data" },
  ];
}

export async function generateMetadata({ params }: PageProps): Promise<Metadata> {
  const category = await getCategoryBySlug(params.slug);

  if (category) {
    return {
      title: `${category.name} | ${SITE.name}`,
      description:
        category.description ||
        `Khám phá các sản phẩm ${category.name} chính hãng tại ${SITE.name}. Giá tốt, giao hàng nhanh.`,
    };
  }

  const nameMap: Record<string, string> = {
    esim: "eSIM Du lịch",
    "the-nap": "Thẻ Viễn thông",
    "the-game": "Thẻ Game",
    data: "Data 4G/5G",
  };
  const name = nameMap[params.slug] ?? params.slug;
  return {
    title: `${name} | ${SITE.name}`,
    description: `Khám phá các sản phẩm ${name} chính hãng tại ${SITE.name}. Giá tốt, giao hàng nhanh.`,
  };
}

export default async function CategoryPage({ params }: PageProps) {
  const category = await getCategoryBySlug(params.slug);

  if (!category) {
    notFound();
  }

  const { products, totalPages } = await getProductsByCategory(params.slug, {});

  return (
    <CategoryView
      category={category}
      products={products}
      totalPages={totalPages}
      slug={params.slug}
    />
  );
}
