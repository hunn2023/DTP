import { z } from "zod";
import type { Language } from "@/lib/i18n";

export function getLoginSchema(language: Language = "vi") {
  const t = {
    emailRequired: language === "vi" ? "Vui lòng nhập email" : "Please enter your email",
    emailInvalid: language === "vi" ? "Email không hợp lệ" : "Invalid email address",
    passwordRequired: language === "vi" ? "Vui lòng nhập mật khẩu" : "Please enter your password",
    passwordMin: language === "vi" ? "Mật khẩu phải có ít nhất 6 ký tự" : "Password must be at least 6 characters",
  };

  return z.object({
    email: z
      .string()
      .min(1, t.emailRequired)
      .email(t.emailInvalid),
    password: z
      .string()
      .min(1, t.passwordRequired)
      .min(6, t.passwordMin),
  });
}

export const loginSchema = getLoginSchema("vi");

export type LoginFormData = z.infer<typeof loginSchema>;
