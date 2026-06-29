import type { Metadata } from "next";
import { Suspense } from "react";
import Breadcrumb from "@/components/ui/Breadcrumb";
import OrderSuccessContent from "./OrderSuccessContent";

export const metadata: Metadata = {
  title: "Đặt hàng thành công | EZSIM",
  robots: "noindex",
};

export default function OrderSuccessPage() {
  return (
    <>
      <Breadcrumb items={[{ label: "Đặt hàng thành công" }]} />
      <main
        className="min-h-[60vh] flex items-center justify-center px-4 py-10 md:py-16"
        aria-label="Trang đặt hàng thành công"
      >
        <Suspense>
          <OrderSuccessContent />
        </Suspense>
      </main>
    </>
  );
}
