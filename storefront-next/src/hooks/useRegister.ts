"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { register, verifyRegisterOtp, resendRegisterOtp, AuthApiError } from "@/lib/authApi";
import type { RegisterPayload } from "@/types/auth";

const OTP_LENGTH = 6;
const OTP_EXPIRES_IN_MS = 5 * 60 * 1000;

interface RegisterOtpSession {
  email: string;
  maskedEmail: string;
  expiresAt: number;
}

function maskEmail(email: string): string {
  const [localPart, domain = ""] = email.split("@");
  if (!localPart) return email;

  const start = localPart.slice(0, 2);
  const end = localPart.length > 2 ? localPart.slice(-1) : "";
  const masked = "*".repeat(Math.max(localPart.length - 3, 2));
  return `${start}${masked}${end}@${domain}`;
}

export function useRegister() {
  const [isLoading, setIsLoading] = useState(false);
  const [otpSession, setOtpSession] = useState<RegisterOtpSession | null>(null);
  const router = useRouter();

  const requestOtp = async (payload: RegisterPayload) => {
    if (isLoading) return;
    setIsLoading(true);

    try {
      // Call register API — backend sends OTP to email
      await register(payload);

      const nextSession: RegisterOtpSession = {
        email: payload.email,
        maskedEmail: maskEmail(payload.email),
        expiresAt: Date.now() + OTP_EXPIRES_IN_MS,
      };

      setOtpSession(nextSession);
      toast.success(`Mã OTP đã được gửi tới ${nextSession.maskedEmail}.`);
    } catch (error) {
      if (error instanceof AuthApiError) {
        toast.error(error.message);
      } else {
        toast.error("Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  const resendOtp = async () => {
    if (!otpSession || isLoading) return;
    setIsLoading(true);

    try {
      await resendRegisterOtp(otpSession.email);
      setOtpSession({
        ...otpSession,
        expiresAt: Date.now() + OTP_EXPIRES_IN_MS,
      });
      toast.success(`Đã gửi lại mã OTP tới ${otpSession.maskedEmail}.`);
    } catch (error) {
      if (error instanceof AuthApiError) {
        toast.error(error.message);
      } else {
        toast.error("Gửi lại OTP thất bại. Vui lòng thử lại.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  const cancelOtp = () => {
    setOtpSession(null);
  };

  const verifyOtpAndRegister = async (otpCode: string) => {
    if (isLoading || !otpSession) return;

    const normalizedCode = otpCode.trim();

    if (normalizedCode.length !== OTP_LENGTH) {
      toast.error("Mã OTP phải gồm 6 chữ số.");
      return;
    }

    if (Date.now() > otpSession.expiresAt) {
      toast.error("Mã OTP đã hết hạn. Vui lòng gửi lại mã mới.");
      return;
    }

    setIsLoading(true);

    try {
      await verifyRegisterOtp(otpSession.email, normalizedCode);
      setOtpSession(null);
      toast.success("Tạo tài khoản thành công! Vui lòng đăng nhập.");
      router.push("/login");
    } catch (error) {
      if (error instanceof AuthApiError) {
        toast.error(error.message);
      } else {
        toast.error("Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return {
    requestOtp,
    verifyOtpAndRegister,
    resendOtp,
    cancelOtp,
    otpSession,
    isLoading,
  };
}
