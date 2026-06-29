"use client";

import { useState } from "react";
import { useRouter, usePathname } from "next/navigation";
import Link from "next/link";
import { toast } from "sonner";
import Icon from "@/components/ui/Icon";
import { useAuth } from "@/hooks/useAuth";
import { useLanguage } from "@/hooks/useLanguage";

function getInitials(name: string): string {
  return name
    .trim()
    .split(/\s+/)
    .filter(Boolean)
    .map((w) => w[0])
    .slice(0, 2)
    .join("")
    .toUpperCase();
}

export default function AccountInfo() {
  const { user, logout } = useAuth();
  const { language } = useLanguage();
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const router = useRouter();
  const pathname = usePathname();

  const navLinks = [
    {
      href: "/account",
      label: language === "vi" ? "Thông tin tài khoản" : "Account information",
      icon: "user",
    },
    {
      href: "/account/orders",
      label: language === "vi" ? "Đơn hàng của tôi" : "My orders",
      icon: "list",
    },
  ] as const;

  const text = {
    member: language === "vi" ? "Thành viên" : "Member",
    accountNavigation: language === "vi" ? "Điều hướng tài khoản" : "Account navigation",
    logoutAria:
      language === "vi" ? "Đăng xuất khỏi tài khoản" : "Sign out of account",
    loggingOut: language === "vi" ? "Đang đăng xuất..." : "Signing out...",
    logout: language === "vi" ? "Đăng xuất" : "Sign out",
    logoutSuccess: language === "vi" ? "Đã đăng xuất thành công." : "Signed out successfully.",
  };

  const handleLogout = () => {
    setIsLoggingOut(true);
    logout();
    toast.success(text.logoutSuccess);
    router.push("/");
  };

  if (!user) return null;

  return (
    <div className="bg-white rounded-2xl shadow-card border border-gray-100 overflow-hidden">
      {/* Banner — avatar lives fully inside here */}
      <div className="relative gradient-primary overflow-hidden px-5 pt-5 pb-7 flex flex-col items-center">
        {/* Decorative rings */}
        <div aria-hidden className="absolute -top-8 -right-8 w-44 h-44 rounded-full bg-white/5 border border-white/10" />
        <div aria-hidden className="absolute top-6 -right-2 w-24 h-24 rounded-full bg-white/5 border border-white/10" />
        <div aria-hidden className="absolute -bottom-12 -left-8 w-40 h-40 rounded-full bg-white/[0.07]" />

        {/* Member badge */}
        <div className="self-start flex items-center gap-1.5 mb-5">
          <div className="w-1.5 h-1.5 rounded-full bg-white/70" />
          <span className="text-white/80 text-[11px] font-semibold uppercase tracking-widest">
            {text.member}
          </span>
        </div>

        {/* Avatar — fully on gradient background */}
        <div className="w-20 h-20 rounded-full bg-white/20 border-[3px] border-white/60 shadow-[0_4px_16px_rgba(0,0,0,0.2)] flex items-center justify-center text-white text-2xl font-bold select-none mb-3">
          {getInitials(user.name)}
        </div>

        {/* Name + email on banner */}
        <h2 className="text-white font-bold text-base leading-snug text-center">
          {user.name}
        </h2>
        <p className="text-white/60 text-xs mt-0.5 truncate max-w-[180px] text-center">
          {user.email}
        </p>

        {user.phone && (
          <div className="mt-3 w-full max-w-[220px] space-y-1.5">
            <div className="flex items-center gap-2 rounded-lg bg-white/12 px-2.5 py-1.5">
              <span className="inline-flex h-5 w-5 items-center justify-center rounded-md bg-white/20">
                <Icon icon="phone" className="text-[10px] text-white" />
              </span>
              <span className="text-[12px] text-white/90 font-medium truncate">{user.phone}</span>
            </div>
          </div>
        )}
      </div>

      {/* Content below banner */}
      <div className="px-5 pt-5 pb-5">
        {/* Nav */}
        <nav className="space-y-1 mb-4" aria-label={text.accountNavigation}>
          {navLinks.map(({ href, label, icon }) => {
            const isActive = pathname === href;
            return (
              <Link
                key={href}
                href={href}
                className={`flex items-center gap-3 px-3.5 py-2.5 rounded-xl text-sm font-medium transition-all ${
                  isActive
                    ? "bg-primary text-white shadow-btn"
                    : "text-gray-600 hover:bg-gray-50 hover:text-navy"
                }`}
              >
                <Icon
                  icon={icon as never}
                  className={`text-xs ${isActive ? "text-white" : "text-gray-400"}`}
                />
                {label}
              </Link>
            );
          })}
        </nav>

        <div className="h-px bg-gray-100 mb-4" />

        {/* Logout */}
        <button
          onClick={handleLogout}
          disabled={isLoggingOut}
          className="w-full flex items-center justify-center gap-2 text-sm font-semibold text-danger py-2.5 rounded-xl border border-danger/20 hover:bg-danger hover:text-white hover:border-danger transition disabled:opacity-60 disabled:cursor-not-allowed"
          aria-label={text.logoutAria}
        >
          <Icon icon="sign-out-alt" />
          {isLoggingOut ? text.loggingOut : text.logout}
        </button>
      </div>
    </div>
  );
}
