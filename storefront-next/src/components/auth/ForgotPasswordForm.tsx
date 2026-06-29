"use client";

import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";
import Icon from "@/components/ui/Icon";
import OtpInput from "@/components/ui/OtpInput";
import { useForgotPassword } from "@/hooks/useForgotPassword";
import {
  forgotPasswordEmailSchema,
  resetPasswordSchema,
  type ForgotPasswordEmailFormData,
  type ResetPasswordFormData,
} from "@/lib/schemas/forgotPasswordSchema";

export default function ForgotPasswordForm() {
  const {
    step,
    maskedEmail,
    isLoading,
    expiresAt,
    requestOtp,
    resendOtp,
    resetPassword,
    backToEmail,
  } = useForgotPassword();

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [otpCode, setOtpCode] = useState("");

  const {
    register: registerEmail,
    handleSubmit: handleEmailSubmit,
    formState: { errors: emailErrors },
  } = useForm<ForgotPasswordEmailFormData>({
    resolver: zodResolver(forgotPasswordEmailSchema),
    mode: "onBlur",
  });

  const {
    register: registerReset,
    handleSubmit: handleResetSubmit,
    formState: { errors: resetErrors },
  } = useForm<ResetPasswordFormData>({
    resolver: zodResolver(resetPasswordSchema),
    mode: "onBlur",
  });

  // Countdown
  const [countdownMs, setCountdownMs] = useState(0);

  useEffect(() => {
    if (!expiresAt) { setCountdownMs(0); return; }
    setCountdownMs(Math.max(0, expiresAt - Date.now()));
    const timer = window.setInterval(() => {
      setCountdownMs(Math.max(0, expiresAt - Date.now()));
    }, 1000);
    return () => window.clearInterval(timer);
  }, [expiresAt]);

  const otpExpired = step === "reset" && countdownMs <= 0;
  const countdownLabel = useMemo(() => {
    const minutes = Math.floor(countdownMs / 60000);
    const seconds = Math.floor((countdownMs % 60000) / 1000);
    return `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
  }, [countdownMs]);

  if (step === "email") {
    return (
      <form onSubmit={handleEmailSubmit((data) => requestOtp(data.email))} noValidate className="space-y-5">
        <div>
          <label htmlFor="forgot-email" className="block text-sm font-medium text-navy mb-1.5">
            Email tài khoản
          </label>
          <div className="relative">
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
              <Icon icon="envelope" className="text-sm" />
            </span>
            <input
              id="forgot-email"
              type="email"
              autoComplete="email"
              placeholder="you@example.com"
              disabled={isLoading}
              {...registerEmail("email")}
              className={`input pl-10 ${emailErrors.email ? "input-error" : ""} disabled:bg-gray-50 disabled:cursor-not-allowed`}
            />
          </div>
          {emailErrors.email && (
            <p role="alert" className="text-danger text-xs mt-1.5 flex items-center gap-1">
              <span aria-hidden>⚠</span> {emailErrors.email.message}
            </p>
          )}
        </div>

        <button
          type="submit"
          disabled={isLoading}
          className="btn btn-primary w-full py-3.5 text-base disabled:opacity-60 disabled:cursor-not-allowed"
        >
          {isLoading ? "Đang gửi OTP..." : "Nhận mã OTP"}
        </button>

        <p className="text-xs text-center text-gray-500">
          Đã nhớ mật khẩu?{" "}
          <Link href="/login" className="font-semibold text-primary hover:text-primary-dark">
            Quay lại đăng nhập
          </Link>
        </p>
      </form>
    );
  }

  // Step "reset": OTP + New password combined
  return (
    <form
      onSubmit={handleResetSubmit((data) => resetPassword(otpCode, data.password))}
      noValidate
      className="space-y-5"
    >
      {/* OTP Section */}
      <div className="text-center">
        <div className="mx-auto mb-3 w-12 h-12 rounded-full bg-blue-50 flex items-center justify-center">
          <Icon icon="envelope" className="text-xl text-primary" />
        </div>
        <p className="text-sm text-slate-600">
          Mã OTP gồm 6 số đã được gửi tới
        </p>
        <p className="mt-1 font-semibold text-navy">{maskedEmail}</p>
      </div>

      <div>
        <label className="block text-sm font-medium text-navy mb-3">
          Nhập mã 6 số <span className="text-danger">*</span>
        </label>
        <OtpInput
          value={otpCode}
          onChange={setOtpCode}
          disabled={isLoading}
          autoFocus
        />
        <p className="mt-2 text-center text-xs text-slate-500">
          Mã còn hiệu lực:{" "}
          <span className={`font-semibold ${otpExpired ? "text-danger" : "text-primary"}`}>
            {otpExpired ? "Đã hết hạn" : countdownLabel}
          </span>
        </p>
      </div>

      {/* New password fields */}
      <div>
        <label htmlFor="new-password" className="block text-sm font-medium text-navy mb-1.5">
          Mật khẩu mới
        </label>
        <div className="relative">
          <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
            <Icon icon="lock" className="text-sm" />
          </span>
          <input
            id="new-password"
            type={showPassword ? "text" : "password"}
            autoComplete="new-password"
            placeholder="••••••••"
            disabled={isLoading}
            {...registerReset("password")}
            className={`input pl-10 pr-12 ${resetErrors.password ? "input-error" : ""} disabled:bg-gray-50 disabled:cursor-not-allowed`}
          />
          <button
            type="button"
            onClick={() => setShowPassword((v) => !v)}
            className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-navy transition"
            aria-label={showPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
          >
            <Icon icon={showPassword ? "eye-slash" : "eye"} className="text-sm" />
          </button>
        </div>
        {resetErrors.password && (
          <p role="alert" className="text-danger text-xs mt-1.5 flex items-center gap-1">
            <span aria-hidden>⚠</span> {resetErrors.password.message}
          </p>
        )}
      </div>

      <div>
        <label htmlFor="confirm-password" className="block text-sm font-medium text-navy mb-1.5">
          Xác nhận mật khẩu mới
        </label>
        <div className="relative">
          <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
            <Icon icon="lock" className="text-sm" />
          </span>
          <input
            id="confirm-password"
            type={showConfirmPassword ? "text" : "password"}
            autoComplete="new-password"
            placeholder="••••••••"
            disabled={isLoading}
            {...registerReset("confirmPassword")}
            className={`input pl-10 pr-12 ${resetErrors.confirmPassword ? "input-error" : ""} disabled:bg-gray-50 disabled:cursor-not-allowed`}
          />
          <button
            type="button"
            onClick={() => setShowConfirmPassword((v) => !v)}
            className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-navy transition"
            aria-label={showConfirmPassword ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
          >
            <Icon icon={showConfirmPassword ? "eye-slash" : "eye"} className="text-sm" />
          </button>
        </div>
        {resetErrors.confirmPassword && (
          <p role="alert" className="text-danger text-xs mt-1.5 flex items-center gap-1">
            <span aria-hidden>⚠</span> {resetErrors.confirmPassword.message}
          </p>
        )}
      </div>

      {/* Submit */}
      <button
        type="submit"
        disabled={isLoading || otpExpired || otpCode.replace(/\s/g, "").length < 6}
        className="btn btn-primary w-full py-3.5 text-base disabled:opacity-60 disabled:cursor-not-allowed"
      >
        {isLoading ? "Đang cập nhật..." : "Đặt mật khẩu mới"}
      </button>

      {/* Resend / Back */}
      <div className="text-center text-sm text-slate-500">
        <span>Chưa nhận được mã? </span>
        <button
          type="button"
          onClick={resendOtp}
          disabled={isLoading}
          className="font-semibold text-primary hover:text-primary-dark transition disabled:opacity-60"
        >
          Gửi lại mã
        </button>
      </div>

      <div className="text-center">
        <button
          type="button"
          onClick={backToEmail}
          disabled={isLoading}
          className="text-sm font-semibold text-gray-500 hover:text-navy transition disabled:opacity-60"
        >
          ← Đổi email khác
        </button>
      </div>
    </form>
  );
}
