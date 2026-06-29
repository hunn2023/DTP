"use client";

import Link from "next/link";
import { useAuth } from "@/hooks/useAuth";
import { useLanguage } from "@/hooks/useLanguage";

export default function HeaderUserMenu() {
  const { user, isAuthenticated, initialized } = useAuth();
  const { language } = useLanguage();

  if (!initialized) {
    return null;
  }

  if (!isAuthenticated || !user) {
    return null;
  }

  const initials = user.name
    .split(" ")
    .slice(-2)
    .map((w) => w[0])
    .join("")
    .toUpperCase();

  const firstName = user.name.split(" ").at(-1) ?? user.name;

  return (
    <Link
      href="/account"
      className="flex items-center gap-2 hover:text-primary transition"
      title={user.name}
      aria-label={language === "vi" ? "Tài khoản của bạn" : "Your account"}
    >
      <div className="w-8 h-8 rounded-full bg-primary flex items-center justify-center text-white text-xs font-bold flex-shrink-0">
        {initials}
      </div>
      <span className="hidden lg:inline text-sm font-medium text-gray-700 max-w-[90px] truncate hover:text-primary transition">
        {firstName}
      </span>
    </Link>
  );
}
