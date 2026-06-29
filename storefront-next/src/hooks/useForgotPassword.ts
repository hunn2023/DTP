"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

const OTP_EXPIRES_IN_MS = 5 * 60 * 1000;

type ForgotPasswordStep = "email" | "reset";

function maskEmail(email: string): string {
  const [localPart, domain = ""] = email.split("@");
  if (!localPart) return email;

  const start = localPart.slice(0, 2);
  const end = localPart.length > 2 ? localPart.slice(-1) : "";
  const masked = "*".repeat(Math.max(localPart.length - 3, 2));
  return `${start}${masked}${end}@${domain}`;
}

export function useForgotPassword() {
  const [isLoading, setIsLoading] = useState(false);
  const [step, setStep] = useState<ForgotPasswordStep>("email");
  const [email, setEmail] = useState("");
  const [maskedEmail, setMaskedEmail] = useState("");
  const [expiresAt, setExpiresAt] = useState<number | null>(null);
  const router = useRouter();

  const requestOtp = async (nextEmail: string) => {
    if (isLoading) return;

    setIsLoading(true);
    try {
      const response = await fetchWithAuth("/api/auth/forgot-password", {
        method: "POST",
        body: JSON.stringify({ email: nextEmail }),
        _skipRefresh: true,
      });

      const json = await response.json().catch(() => ({}));

      if (!response.ok || (json && json.isSuccess === false)) {
        const errorMsg = json?.error || json?.message || "Không thể gửi mã OTP. Vui lòng thử lại.";
        toast.error(errorMsg);
        return;
      }

      setEmail(nextEmail);
      setMaskedEmail(maskEmail(nextEmail));
      setExpiresAt(Date.now() + OTP_EXPIRES_IN_MS);
      setStep("reset");
      toast.success("Mã OTP đã được gửi tới email của bạn.");
    } catch {
      toast.error("Lỗi kết nối. Vui lòng thử lại.");
    } finally {
      setIsLoading(false);
    }
  };

  const resendOtp = async () => {
    if (!email || isLoading) return;

    setIsLoading(true);
    try {
      const response = await fetchWithAuth("/api/auth/forgot-password", {
        method: "POST",
        body: JSON.stringify({ email }),
        _skipRefresh: true,
      });

      const json = await response.json().catch(() => ({}));

      if (!response.ok || (json && json.isSuccess === false)) {
        const errorMsg = json?.error || json?.message || "Không thể gửi lại mã OTP.";
        toast.error(errorMsg);
        return;
      }

      setExpiresAt(Date.now() + OTP_EXPIRES_IN_MS);
      toast.success("Đã gửi lại mã OTP.");
    } catch {
      toast.error("Lỗi kết nối. Vui lòng thử lại.");
    } finally {
      setIsLoading(false);
    }
  };

  const resetPassword = async (otp: string, newPassword: string) => {
    if (isLoading) return;

    setIsLoading(true);
    try {
      const response = await fetchWithAuth("/api/auth/reset-password", {
        method: "POST",
        body: JSON.stringify({ email, otpCode: otp, newPassword }),
        _skipRefresh: true,
      });

      const json = await response.json().catch(() => ({}));

      if (!response.ok || (json && json.isSuccess === false)) {
        const errorMsg = json?.error || json?.message || "Đổi mật khẩu thất bại. Vui lòng thử lại.";
        toast.error(errorMsg);
        return;
      }

      toast.success("Đổi mật khẩu thành công! Vui lòng đăng nhập lại.");
      router.push("/login");
    } catch {
      toast.error("Lỗi kết nối. Vui lòng thử lại.");
    } finally {
      setIsLoading(false);
    }
  };

  const backToEmail = () => {
    setStep("email");
    setExpiresAt(null);
  };

  return {
    isLoading,
    step,
    email,
    maskedEmail,
    expiresAt,
    requestOtp,
    resendOtp,
    resetPassword,
    backToEmail,
  };
}
