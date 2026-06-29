import type { Metadata } from "next";
import LoginContent from "@/app/(auth)/login/LoginContent";
import { SITE } from "@/lib/constants";

export const metadata: Metadata = {
  title: `Đăng nhập | ${SITE.name}`,
  description: "Đăng nhập để quản lý đơn hàng và trải nghiệm mua sắm tốt hơn.",
};

export default function LoginPage() {
  return <LoginContent />;
}
