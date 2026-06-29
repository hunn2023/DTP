"use client";

import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";
import Icon from "@/components/ui/Icon";
import OtpInput from "@/components/ui/OtpInput";
import { getRegisterSchema, type RegisterFormData } from "@/lib/schemas/registerSchema";
import { useRegister } from "@/hooks/useRegister";
import { useLanguage } from "@/hooks/useLanguage";

interface FieldProps {
  id: string;
  label: string;
  error?: string;
  children: React.ReactNode;
}

function Field({ id, label, error, children }: FieldProps) {
  return (
    <div>
      <label htmlFor={id} className="block text-sm font-medium text-navy mb-1.5">
        {label}
      </label>
      {children}
      {error && (
        <p role="alert" className="text-danger text-xs mt-1.5 flex items-center gap-1">
          <span aria-hidden>⚠</span> {error}
        </p>
      )}
    </div>
  );
}

export default function RegisterForm() {
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [otpCode, setOtpCode] = useState("");
  const [remainingMs, setRemainingMs] = useState(0);
  const { requestOtp, verifyOtpAndRegister, resendOtp, cancelOtp, otpSession, isLoading } = useRegister();
  const { language } = useLanguage();
  const registerSchema = useMemo(() => getRegisterSchema(language), [language]);

  const text = {
    otpSentTo:
      language === "vi"
        ? "Mã OTP gồm 6 số đã được gửi tới"
        : "A 6-digit verification code has been sent to",
    otpValidUntil: language === "vi" ? "Mã còn hiệu lực:" : "Code valid for:",
    otpExpired: language === "vi" ? "Đã hết hạn" : "Expired",
    otpLabel: language === "vi" ? "Nhập mã 6 số" : "Enter your 6-digit code",
    verifying: language === "vi" ? "Đang xác thực..." : "Verifying...",
    verifyAndCreate:
      language === "vi" ? "Xác nhận" : "Confirm",
    resendOtp: language === "vi" ? "Gửi lại mã OTP" : "Resend OTP",
    dontHaveCode: language === "vi" ? "Chưa nhận được mã?" : "Don't have a code?",
    editInfo: language === "vi" ? "Chỉnh sửa thông tin" : "Edit information",
    fullName: language === "vi" ? "Họ và tên" : "Full name",
    fullNamePlaceholder: language === "vi" ? "Nguyễn Văn A" : "John Doe",
    phone: language === "vi" ? "Số điện thoại" : "Phone number",
    password: language === "vi" ? "Mật khẩu" : "Password",
    confirmPassword: language === "vi" ? "Xác nhận mật khẩu" : "Confirm password",
    hidePassword: language === "vi" ? "Ẩn mật khẩu" : "Hide password",
    showPassword: language === "vi" ? "Hiện mật khẩu" : "Show password",
    sendingOtp: language === "vi" ? "Đang gửi OTP..." : "Sending OTP...",
    continueOtp: language === "vi" ? "Tiếp tục nhận OTP" : "Continue to OTP",
    haveAccount: language === "vi" ? "đã có tài khoản?" : "already have an account?",
    login: language === "vi" ? "Đăng nhập" : "Sign in",
  };

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
    mode: "onBlur",
  });

  useEffect(() => {
    if (!otpSession) {
      setOtpCode("");
      setRemainingMs(0);
      return;
    }

    const updateCountdown = () => {
      setRemainingMs(Math.max(0, otpSession.expiresAt - Date.now()));
    };

    updateCountdown();
    const interval = window.setInterval(updateCountdown, 1000);
    return () => window.clearInterval(interval);
  }, [otpSession]);

  const otpExpired = otpSession ? remainingMs <= 0 : false;
  const otpCountdownText = `${String(Math.floor(remainingMs / 60000)).padStart(2, "0")}:${String(Math.floor((remainingMs % 60000) / 1000)).padStart(2, "0")}`;

  const handleOtpSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    await verifyOtpAndRegister(otpCode);
  };

  if (otpSession) {
    return (
      <form onSubmit={handleOtpSubmit} noValidate className="space-y-5">
        <div className="text-center">
          <div className="mx-auto mb-4 w-14 h-14 rounded-full bg-blue-50 flex items-center justify-center">
            <Icon icon="envelope" className="text-2xl text-primary" />
          </div>
          <p className="text-sm text-slate-600">
            {text.otpSentTo}
          </p>
          <p className="mt-1 font-semibold text-navy">{otpSession.maskedEmail}</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-navy mb-3">
            {text.otpLabel} <span className="text-danger">*</span>
          </label>
          <OtpInput
            value={otpCode}
            onChange={setOtpCode}
            disabled={isLoading}
            autoFocus
          />
          {otpSession && (
            <p className="mt-3 text-center text-xs text-slate-500">
              {text.otpValidUntil}{" "}
              <span className={`font-semibold ${otpExpired ? "text-danger" : "text-primary"}`}>
                {otpExpired ? text.otpExpired : otpCountdownText}
              </span>
            </p>
          )}
        </div>

        <button
          type="submit"
          disabled={isLoading || otpExpired || otpCode.replace(/\s/g, "").length < 6}
          aria-busy={isLoading}
          className="btn btn-primary w-full py-3.5 text-base disabled:opacity-60 disabled:cursor-not-allowed"
        >
          {isLoading ? text.verifying : text.verifyAndCreate}
        </button>

        <div className="text-center text-sm text-slate-500">
          <span>{text.dontHaveCode} </span>
          <button
            type="button"
            onClick={resendOtp}
            disabled={isLoading}
            className="font-semibold text-primary hover:text-primary-dark transition disabled:opacity-60"
          >
            {text.resendOtp}
          </button>
        </div>

        <div className="text-center">
          <button
            type="button"
            onClick={cancelOtp}
            disabled={isLoading}
            className="text-sm font-semibold text-gray-500 hover:text-navy transition disabled:opacity-60"
          >
            ← {text.editInfo}
          </button>
        </div>
      </form>
    );
  }

  return (
    <form onSubmit={handleSubmit(requestOtp)} noValidate className="space-y-3.5 md:space-y-4">

      {/* Name + Phone — 2 columns on sm+ */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 md:gap-4">
        <Field id="reg-name" label={text.fullName} error={errors.name?.message}>
          <div className="relative">
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
              <Icon icon="user" className="text-sm" />
            </span>
            <input
              id="reg-name"
              type="text"
              autoComplete="name"
              placeholder={text.fullNamePlaceholder}
              disabled={isLoading}
              {...register("name")}
              className={`input pl-10 ${errors.name ? "input-error" : ""} disabled:bg-gray-50 disabled:cursor-not-allowed`}
            />
          </div>
        </Field>

        <Field id="reg-phone" label={text.phone} error={errors.phone?.message}>
          <div className="relative">
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
              <Icon icon="phone" className="text-sm" />
            </span>
            <input
              id="reg-phone"
              type="tel"
              autoComplete="tel"
              placeholder="0987 654 321"
              disabled={isLoading}
              {...register("phone")}
              className={`input pl-10 ${errors.phone ? "input-error" : ""} disabled:bg-gray-50 disabled:cursor-not-allowed`}
            />
          </div>
        </Field>
      </div>

      {/* Email */}
      <Field id="reg-email" label="Email" error={errors.email?.message}>
        <div className="relative">
          <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
            <Icon icon="envelope" className="text-sm" />
          </span>
          <input
            id="reg-email"
            type="email"
            autoComplete="email"
            placeholder="you@example.com"
            disabled={isLoading}
            {...register("email")}
            className={`input pl-10 ${errors.email ? "input-error" : ""} disabled:bg-gray-50 disabled:cursor-not-allowed`}
          />
        </div>
      </Field>

      {/* Password + Confirm — 2 columns on sm+ */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 md:gap-4">
        <Field id="reg-password" label={text.password} error={errors.password?.message}>
          <div className="relative">
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
              <Icon icon="lock" className="text-sm" />
            </span>
            <input
              id="reg-password"
              type={showPassword ? "text" : "password"}
              autoComplete="new-password"
              placeholder="••••••••"
              disabled={isLoading}
              {...register("password")}
              className={`input pl-10 pr-12 ${errors.password ? "input-error" : ""} disabled:bg-gray-50 disabled:cursor-not-allowed`}
            />
            <button
              type="button"
              onClick={() => setShowPassword((v) => !v)}
              className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-navy transition"
              aria-label={showPassword ? text.hidePassword : text.showPassword}
            >
              <Icon icon={showPassword ? "eye-slash" : "eye"} className="text-sm" />
            </button>
          </div>
        </Field>

        <Field id="reg-confirm" label={text.confirmPassword} error={errors.confirmPassword?.message}>
          <div className="relative">
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
              <Icon icon="lock" className="text-sm" />
            </span>
            <input
              id="reg-confirm"
              type={showConfirm ? "text" : "password"}
              autoComplete="new-password"
              placeholder="••••••••"
              disabled={isLoading}
              {...register("confirmPassword")}
              className={`input pl-10 pr-12 ${errors.confirmPassword ? "input-error" : ""} disabled:bg-gray-50 disabled:cursor-not-allowed`}
            />
            <button
              type="button"
              onClick={() => setShowConfirm((v) => !v)}
              className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-navy transition"
              aria-label={showConfirm ? text.hidePassword : text.showPassword}
            >
              <Icon icon={showConfirm ? "eye-slash" : "eye"} className="text-sm" />
            </button>
          </div>
        </Field>
      </div>

      {/* Submit */}
      <button
        type="submit"
        disabled={isLoading}
        aria-busy={isLoading}
        className="btn btn-primary w-full py-3 text-base mt-1 disabled:opacity-60 disabled:cursor-not-allowed active:scale-[0.98]"
      >
        {isLoading ? (
          <>
            <svg
              className="animate-spin h-4 w-4"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              aria-hidden
            >
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
              <path
                className="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
              />
            </svg>
            {text.sendingOtp}
          </>
        ) : (
          <>
            {text.continueOtp}
            <Icon icon="arrow-right" className="text-sm" />
          </>
        )}
      </button>

      {/* Divider */}
      <div className="relative flex items-center gap-3 py-1">
        <div className="flex-1 h-px bg-gray-200" />
        <span className="text-xs text-gray-400 whitespace-nowrap">{text.haveAccount}</span>
        <div className="flex-1 h-px bg-gray-200" />
      </div>

      {/* Login link */}
      <Link
        href="/login"
        className="btn btn-outline w-full py-2.5 text-sm hover:shadow-btn transition"
      >
        {text.login}
      </Link>
    </form>
  );
}
