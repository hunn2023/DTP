import { z } from "zod";
import type { Language } from "@/lib/i18n";

export function getProfileSchema(language: Language = "vi") {
  const t = {
    nameMin: language === "vi" ? "Họ tên phải có ít nhất 2 ký tự" : "Full name must be at least 2 characters",
    nameMax: language === "vi" ? "Họ tên không được vượt quá 100 ký tự" : "Full name must be at most 100 characters",
    phoneInvalid:
      language === "vi"
        ? "Số điện thoại không hợp lệ (VD: 0987654321)"
        : "Invalid phone number (e.g. 0987654321)",
  };

  return z.object({
    name: z
      .string()
      .trim()
      .min(2, t.nameMin)
      .max(100, t.nameMax),
    phone: z
      .string()
      .trim()
      .regex(/^(0|\+84)\d{9,10}$/, t.phoneInvalid),
  });
}

export const profileSchema = getProfileSchema("vi");

export type ProfileFormData = z.infer<typeof profileSchema>;
