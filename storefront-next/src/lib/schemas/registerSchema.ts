import { z } from "zod";
import type { Language } from "@/lib/i18n";

export function getRegisterSchema(language: Language = "vi") {
  const t = {
    nameMin: language === "vi" ? "Họ tên phải có ít nhất 2 ký tự" : "Full name must be at least 2 characters",
    nameMax: language === "vi" ? "Họ tên không được vượt quá 100 ký tự" : "Full name must be at most 100 characters",
    emailRequired: language === "vi" ? "Vui lòng nhập email" : "Please enter your email",
    emailInvalid: language === "vi" ? "Email không hợp lệ" : "Invalid email address",
    phoneInvalid:
      language === "vi"
        ? "Số điện thoại không hợp lệ (VD: 0987654321)"
        : "Invalid phone number (e.g. 0987654321)",
    passwordRequired: language === "vi" ? "Vui lòng nhập mật khẩu" : "Please enter your password",
    passwordMin: language === "vi" ? "Mật khẩu phải có ít nhất 6 ký tự" : "Password must be at least 6 characters",
    confirmPasswordRequired:
      language === "vi" ? "Vui lòng xác nhận mật khẩu" : "Please confirm your password",
    passwordMismatch:
      language === "vi" ? "Mật khẩu xác nhận không khớp" : "Password confirmation does not match",
  };

  return z
    .object({
      name: z
        .string()
        .trim()
        .min(2, t.nameMin)
        .max(100, t.nameMax),
      email: z
        .string()
        .trim()
        .min(1, t.emailRequired)
        .email(t.emailInvalid),
      phone: z
        .string()
        .trim()
        .regex(/^(0|\+84)\d{9,10}$/, t.phoneInvalid),
      password: z
        .string()
        .min(1, t.passwordRequired)
        .min(6, t.passwordMin),
      confirmPassword: z.string().min(1, t.confirmPasswordRequired),
    })
    .refine((data) => data.password === data.confirmPassword, {
      message: t.passwordMismatch,
      path: ["confirmPassword"],
    });
}

export const registerSchema = getRegisterSchema("vi");

export type RegisterFormData = z.infer<typeof registerSchema>;
