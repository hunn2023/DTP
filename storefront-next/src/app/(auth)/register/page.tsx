import type { Metadata } from "next";
import RegisterContent from "@/app/(auth)/register/RegisterContent";
import { SITE } from "@/lib/constants";

export const metadata: Metadata = {
  title: `Tạo tài khoản | ${SITE.name}`,
  description: "Tạo tài khoản để mua sắm, theo dõi đơn hàng và nhận ưu đãi thành viên.",
};

export default function RegisterPage() {
  return <RegisterContent />;
}
