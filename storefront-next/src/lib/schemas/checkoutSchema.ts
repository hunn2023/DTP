import { z } from "zod";
import type { Language } from "@/lib/i18n";

export function getCheckoutSchema(language: Language = "vi") {
  const t = {
    fullNameMin: language === "vi" ? "Tên phải có ít nhất 2 ký tự" : "Name must be at least 2 characters",
    fullNameMax: language === "vi" ? "Tên không được vượt quá 100 ký tự" : "Name must be at most 100 characters",
    phoneInvalid:
      language === "vi"
        ? "Số điện thoại không hợp lệ (phải là số Việt Nam)"
        : "Invalid phone number (must be a Vietnam number format)",
    emailInvalid: language === "vi" ? "Email không hợp lệ" : "Invalid email address",
    orderNoteMax:
      language === "vi"
        ? "Ghi chú không được vượt quá 500 ký tự"
        : "Order note must be at most 500 characters",
    paymentRequired:
      language === "vi"
        ? "Vui lòng chọn phương thức thanh toán"
        : "Please select a payment method",
    termsRequired:
      language === "vi"
        ? "Bạn cần đồng ý Điều khoản dịch vụ để tiếp tục"
        : "You must agree to the Terms of Service to continue",
    esimDeviceRequired:
      language === "vi"
        ? "Vui lòng xác nhận thiết bị tương thích trong danh sách trước khi tiếp tục"
        : "Please confirm device compatibility from the list before continuing",
  };

  return z.object({
    fullName: z
      .string()
      .min(2, t.fullNameMin)
      .max(100, t.fullNameMax),

    phone: z
      .string()
      .regex(/^(0|\+84)\d{9,10}$/, t.phoneInvalid),

    email: z
      .string()
      .email(t.emailInvalid)
      .optional()
      .or(z.literal("")),

    orderNote: z
      .string()
      .max(500, t.orderNoteMax)
      .optional()
      .or(z.literal("")),
    paymentMethod: z.string().min(1, t.paymentRequired),
    requestInvoice: z.boolean(),
    agreeTerms: z.boolean().refine((value) => value === true, {
      message: t.termsRequired,
    }),
    confirmDeviceSupport: z.boolean().refine((value) => value === true, {
      message: t.esimDeviceRequired,
    }),
  });
}

export const checkoutSchema = getCheckoutSchema("vi");

export type CheckoutFormData = z.infer<typeof checkoutSchema>;

/** Provider `code` from GET /api/public/payment-providers (e.g. SEPAY). Mapped to `paymentMethod` on confirm. */
export type PaymentMethodType = string;