"use client";

import { Suspense, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";
import Icon from "@/components/ui/Icon";
import { getLoginSchema, type LoginFormData } from "@/lib/schemas/loginSchema";
import { useLogin } from "@/hooks/useLogin";
import { useLanguage } from "@/hooks/useLanguage";

export default function LoginForm() {
  return (
    <Suspense fallback={<div className="min-h-[300px]" />}>
      <LoginFormInner />
    </Suspense>
  );
}

function LoginFormInner() {
  const [showPassword, setShowPassword] = useState(false);
  const { handleLogin, isLoading } = useLogin();
  const { language } = useLanguage();
  const loginSchema = useMemo(() => getLoginSchema(language), [language]);

  const text = {
    password: language === "vi" ? "Mật khẩu" : "Password",
    forgotPassword: language === "vi" ? "Quên mật khẩu?" : "Forgot password?",
    hidePassword: language === "vi" ? "Ẩn mật khẩu" : "Hide password",
    showPassword: language === "vi" ? "Hiện mật khẩu" : "Show password",
    loggingIn: language === "vi" ? "Đang đăng nhập..." : "Signing in...",
    login: language === "vi" ? "Đăng nhập" : "Sign in",
    noAccount: language === "vi" ? "chưa có tài khoản?" : "don't have an account?",
    createAccount: language === "vi" ? "Tạo tài khoản mới" : "Create a new account",
  };

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    mode: "onBlur",
  });

  return (
    <form
      onSubmit={handleSubmit(handleLogin)}
      noValidate
      className="space-y-4 md:space-y-5"
    >
      {/* Email */}
      <div>
        <label
          htmlFor="login-email"
          className="block text-sm font-medium text-navy mb-1.5"
        >
          Email
        </label>
        <div className="relative">
          <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
            <Icon icon="envelope" className="text-sm" />
          </span>
          <input
            id="login-email"
            type="email"
            autoComplete="email"
            placeholder="you@example.com"
            disabled={isLoading}
            {...register("email")}
            className={`input pl-10 ${errors.email ? "input-error" : ""} disabled:bg-gray-50 disabled:cursor-not-allowed`}
          />
        </div>
        {errors.email && (
          <p role="alert" className="text-danger text-xs mt-1.5 flex items-center gap-1">
            <span>⚠</span> {errors.email.message}
          </p>
        )}
      </div>

      {/* Password */}
      <div>
        <div className="flex items-center justify-between mb-1.5">
          <label
            htmlFor="login-password"
            className="block text-sm font-medium text-navy"
          >
            {text.password}
          </label>
          <Link
            href="/forgot-password"
            className="text-xs text-primary hover:text-primary-dark hover:underline transition"
            tabIndex={0}
          >
            {text.forgotPassword}
          </Link>
        </div>
        <div className="relative">
          <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
            <Icon icon="lock" className="text-sm" />
          </span>
          <input
            id="login-password"
            type={showPassword ? "text" : "password"}
            autoComplete="current-password"
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
            tabIndex={0}
          >
            <Icon icon={showPassword ? "eye-slash" : "eye"} className="text-sm" />
          </button>
        </div>
        {errors.password && (
          <p role="alert" className="text-danger text-xs mt-1.5 flex items-center gap-1">
            <span>⚠</span> {errors.password.message}
          </p>
        )}
      </div>

      {/* Submit */}
      <button
        type="submit"
        disabled={isLoading}
        aria-busy={isLoading}
        className="btn btn-primary w-full py-3 text-base mt-1.5 disabled:opacity-60 disabled:cursor-not-allowed active:scale-[0.98]"
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
              <circle
                className="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                strokeWidth="4"
              />
              <path
                className="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
              />
            </svg>
            {text.loggingIn}
          </>
        ) : (
          <>
            {text.login}
            <Icon icon="arrow-right" className="text-sm" />
          </>
        )}
      </button>

      {/* Divider */}
      <div className="relative flex items-center gap-3 py-1">
        <div className="flex-1 h-px bg-gray-200" />
        <span className="text-xs text-gray-400 whitespace-nowrap">{text.noAccount}</span>
        <div className="flex-1 h-px bg-gray-200" />
      </div>

      {/* Register link */}
      <Link
        href="/register"
        className="btn btn-outline w-full py-2.5 text-sm hover:shadow-btn transition"
      >
        {text.createAccount}
      </Link>
    </form>
  );
}
