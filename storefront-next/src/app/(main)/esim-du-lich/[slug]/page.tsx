import type { Metadata } from "next";
import { getEsimCountryBySlug } from "@/lib/api/esimApi";
import { getProductContents, getProductFaqs } from "@/lib/api/productContentApi";
import { fetchWithAuth } from "@/lib/fetchWithAuth";
import EsimCountryView from "./EsimCountryView";

export const revalidate = 60;

export async function generateStaticParams() {
  // Only the product slug ("esim-nhat-ban") is canonical. We deliberately do NOT
  // emit country slugs ("nhat-ban") so each detail page exists at exactly one URL.
  const slugSet = new Set<string>();

  try {
    const res = await fetchWithAuth("/api/catalog/products/home/esim-products");
    if (res.ok) {
      const json = await res.json();
      const items: { slug: string }[] = json.data ?? [];
      for (const item of items) slugSet.add(item.slug);
    }
  } catch { /* ignore */ }

  if (slugSet.size > 0) {
    return Array.from(slugSet).map((slug) => ({ slug }));
  }

  // Fallback
  return [{ slug: "esim-han-quoc" }];
}

export async function generateMetadata({ params }: { params: { slug: string } }): Promise<Metadata> {
  const country = await getEsimCountryBySlug(params.slug);
  const description =
    country.tags && country.tags.length > 0
      ? `Gói ${country.name}: ${country.tags.join(", ")}. Kích hoạt nhanh bằng QR Code.`
      : `Các gói ${country.name} giá tốt, kích hoạt nhanh bằng QR Code.`;
  return {
    title: `${country.name} - Kết nối ngay khi đặt chân | EZSIM`,
    description,
  };
}

export default async function EsimCountryPage({ params }: { params: { slug: string } }) {
  const country = await getEsimCountryBySlug(params.slug);
  const [contents, faqs] = country.productId
    ? await Promise.all([
        getProductContents(country.productId),
        getProductFaqs(country.productId),
      ])
    : [[], []];

  return <EsimCountryView country={country} contents={contents} faqs={faqs} />;
}
