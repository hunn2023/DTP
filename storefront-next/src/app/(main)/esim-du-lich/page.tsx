import type { Metadata } from "next";
import { getEsimCountries } from "@/lib/api/esimApi";
import EsimDuLichContent from "./EsimDuLichContent";

export const revalidate = 60;

export const metadata: Metadata = {
  title: "eSIM Du lịch 200+ quốc gia | EZSIM",
  description: "Chọn quốc gia bạn đang đến để xem các gói eSIM phù hợp. Kích hoạt 30 giây.",
};

export default async function EsimDuLichPage() {
  const destinations = await getEsimCountries();
  return <EsimDuLichContent destinations={destinations} />;
}
