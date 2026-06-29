import type { Metadata } from "next";
import SupportContent from "./SupportContent";

export const metadata: Metadata = {
  title: "Hỗ trợ | EZSIM",
  description: "Liên hệ hỗ trợ EZSIM qua email và hotline.",
};

export default function SupportPage() {
  return <SupportContent />;
}
