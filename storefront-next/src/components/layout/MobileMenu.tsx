"use client";

import { Suspense, useState } from "react";
import Icon from "@/components/ui/Icon";
import Link from "next/link";
import Image from "next/image";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import type { IconName } from "@fortawesome/fontawesome-svg-core";
import { useAuth } from "@/hooks/useAuth";
import { useLanguage } from "@/hooks/useLanguage";
import { languageFlagCodes, languageNames, languageShortLabels, type Language } from "@/lib/i18n";

interface MobileMenuItem {
  labels: Record<Language, string>;
  icon: IconName;
  href: string;
  matchPath: string;
  matchTab?: string;
}

const menuItems: MobileMenuItem[] = [
  { labels: { vi: "Trang chủ", en: "Home" }, icon: "home", href: "/", matchPath: "/" },
  { labels: { vi: "eSIM Du lịch", en: "Travel eSIM" }, icon: "globe-asia", href: "/esim-du-lich", matchPath: "/esim-du-lich" },
  { labels: { vi: "Blog", en: "Blog" }, icon: "blog", href: "/blog", matchPath: "/blog" },
  { labels: { vi: "Hỗ trợ", en: "Support" }, icon: "headset", href: "/support", matchPath: "/support" },
];

function isMenuActive(item: MobileMenuItem, pathname: string, currentTab: string | null): boolean {
  if (item.matchPath === "/") return pathname === "/";

  const pathMatches = pathname === item.matchPath || pathname.startsWith(item.matchPath + "/");
  if (!pathMatches) return false;

  if (item.matchTab) {
    const effectiveTab = currentTab ?? (item.matchPath === "/the-nap" ? "telecom" : null);
    return effectiveTab === item.matchTab;
  }

  return true;
}

export default function MobileMenu() {
  return (
    <Suspense fallback={<div className="lg:hidden w-10 h-10" />}>
      <MobileMenuInner />
    </Suspense>
  );
}

function MobileMenuInner() {
  const [open, setOpen] = useState(false);
  const { language, setLanguage } = useLanguage();
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const currentTab = searchParams.get("tab");
  const { user, isAuthenticated, initialized, logout } = useAuth();

  const text = {
    menu: language === "vi" ? "Menu" : "Menu",
    openMenu: language === "vi" ? "Mở menu" : "Open menu",
    closeMenu: language === "vi" ? "Đóng menu" : "Close menu",
    logout: language === "vi" ? "Đăng xuất" : "Logout",
    login: language === "vi" ? "Đăng nhập" : "Login",
    register: language === "vi" ? "Đăng ký" : "Register",
    language: language === "vi" ? "Ngôn ngữ" : "Language",
  };

  const handleLanguageChange = (nextLanguage: Language) => {
    if (nextLanguage === language) return;
    setLanguage(nextLanguage);
    router.refresh();
  };

  return (
    <>
      <button
        onClick={() => setOpen(true)}
        className="lg:hidden w-10 h-10 flex items-center justify-center text-navy"
        aria-label={text.openMenu}
      >
        <Icon icon="bars" className="text-xl" />
      </button>

      {/* Overlay */}
      {open && (
        <div className="fixed inset-0 bg-black/50 z-[200] lg:hidden" onClick={() => setOpen(false)} />
      )}

      {/* Drawer */}
      <div
        className={`fixed top-0 left-0 h-[100dvh] w-[280px] bg-white z-[201] transform transition-transform duration-300 lg:hidden flex flex-col overflow-hidden ${
          open ? "translate-x-0" : "-translate-x-full"
        }`}
      >
        <div className="flex items-center justify-between p-4 border-b border-gray-200">
          <span className="text-lg font-bold text-navy">{text.menu}</span>
          <button onClick={() => setOpen(false)} aria-label={text.closeMenu}>
            <Icon icon="times" className="text-xl text-gray-600" />
          </button>
        </div>

        <nav className="p-4 space-y-1">
          {menuItems.map((item) => {
            const isActive = isMenuActive(item, pathname ?? "", currentTab);
            return (
              <Link
                key={item.href}
                href={item.href}
                onClick={() => setOpen(false)}
                className={`flex items-center gap-3 px-3 py-3 rounded-lg text-sm font-medium transition ${
                  isActive
                    ? "bg-primary/10 text-primary"
                    : "text-gray-700 hover:bg-gray-100"
                }`}
              >
                <Icon icon={item.icon} className="w-4" />
                {item.labels[language]}
              </Link>
            );
          })}
        </nav>

        <div className="mt-auto px-4 pt-2 pb-[max(1rem,env(safe-area-inset-bottom))] border-t border-gray-200 space-y-2">
          {!initialized ? null : isAuthenticated && user ? (
            <>
              <Link
                href="/account"
                onClick={() => setOpen(false)}
                className="flex items-center gap-3 px-3 py-3 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-100 transition"
              >
                <div className="w-7 h-7 rounded-full bg-primary flex items-center justify-center text-white text-xs font-bold flex-shrink-0">
                  {user.name.split(" ").slice(-2).map((w) => w[0]).join("").toUpperCase()}
                </div>
                <div className="min-w-0 flex-1">
                  <span className="truncate block">{user.name}</span>
                </div>
              </Link>

              <div className="rounded-lg border border-gray-200 p-2.5 bg-gray-50">
                <p className="text-[11px] font-semibold uppercase tracking-wide text-gray-500 mb-2">{text.language}</p>
                <div className="grid grid-cols-2 gap-2">
                  <button
                    type="button"
                    onClick={() => handleLanguageChange("vi")}
                    className={`px-2 py-2 rounded-lg text-xs font-semibold border transition flex items-center justify-center gap-1.5 ${
                      language === "vi"
                        ? "bg-primary text-white border-primary"
                        : "bg-white text-gray-700 border-gray-200 hover:border-primary hover:text-primary"
                    }`}
                  >
                    <span className="inline-flex h-3.5 w-3.5 overflow-hidden rounded-full" aria-hidden>
                      <Image
                        src={`https://flagcdn.com/w80/${languageFlagCodes.vi}.png`}
                        alt={languageNames.vi}
                        width={14}
                        height={14}
                        quality={100}
                        unoptimized
                        className="h-full w-full object-cover"
                      />
                    </span>
                    {languageShortLabels.vi}
                  </button>
                  <button
                    type="button"
                    onClick={() => handleLanguageChange("en")}
                    className={`px-2 py-2 rounded-lg text-xs font-semibold border transition flex items-center justify-center gap-1.5 ${
                      language === "en"
                        ? "bg-primary text-white border-primary"
                        : "bg-white text-gray-700 border-gray-200 hover:border-primary hover:text-primary"
                    }`}
                  >
                    <span className="inline-flex h-3.5 w-3.5 overflow-hidden rounded-full" aria-hidden>
                      <Image
                        src={`https://flagcdn.com/w80/${languageFlagCodes.en}.png`}
                        alt={languageNames.en}
                        width={14}
                        height={14}
                        quality={100}
                        unoptimized
                        className="h-full w-full object-cover"
                      />
                    </span>
                    {languageShortLabels.en}
                  </button>
                </div>
              </div>

              <button
                type="button"
                onClick={() => {
                  logout();
                  setOpen(false);
                }}
                className="w-full flex items-center justify-center gap-2 px-3 py-3 rounded-lg text-sm font-medium text-red-600 bg-red-50 hover:bg-red-100 border border-red-200 transition"
              >
                <Icon icon="sign-out-alt" className="w-4" />
                {text.logout}
              </button>
            </>
          ) : (
            <>
              <div className="rounded-lg border border-gray-200 p-2.5 bg-gray-50">
                <p className="text-[11px] font-semibold uppercase tracking-wide text-gray-500 mb-2">{text.language}</p>
                <div className="grid grid-cols-2 gap-2">
                  <button
                    type="button"
                    onClick={() => handleLanguageChange("vi")}
                    className={`px-2 py-2 rounded-lg text-xs font-semibold border transition flex items-center justify-center gap-1.5 ${
                      language === "vi"
                        ? "bg-primary text-white border-primary"
                        : "bg-white text-gray-700 border-gray-200 hover:border-primary hover:text-primary"
                    }`}
                  >
                    <span className="inline-flex h-3.5 w-3.5 overflow-hidden rounded-full" aria-hidden>
                      <Image
                        src={`https://flagcdn.com/w80/${languageFlagCodes.vi}.png`}
                        alt={languageNames.vi}
                        width={14}
                        height={14}
                        quality={100}
                        unoptimized
                        className="h-full w-full object-cover"
                      />
                    </span>
                    {languageShortLabels.vi}
                  </button>
                  <button
                    type="button"
                    onClick={() => handleLanguageChange("en")}
                    className={`px-2 py-2 rounded-lg text-xs font-semibold border transition flex items-center justify-center gap-1.5 ${
                      language === "en"
                        ? "bg-primary text-white border-primary"
                        : "bg-white text-gray-700 border-gray-200 hover:border-primary hover:text-primary"
                    }`}
                  >
                    <span className="inline-flex h-3.5 w-3.5 overflow-hidden rounded-full" aria-hidden>
                      <Image
                        src={`https://flagcdn.com/w80/${languageFlagCodes.en}.png`}
                        alt={languageNames.en}
                        width={14}
                        height={14}
                        quality={100}
                        unoptimized
                        className="h-full w-full object-cover"
                      />
                    </span>
                    {languageShortLabels.en}
                  </button>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-2">
                <Link
                  href="/login"
                  onClick={() => setOpen(false)}
                  className="flex items-center justify-center gap-2 px-3 py-3 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-100 border border-gray-200 transition"
                >
                  <Icon icon="user" className="w-4" />
                  {text.login}
                </Link>
                <Link
                  href="/register"
                  onClick={() => setOpen(false)}
                  className="flex items-center justify-center px-3 py-3 rounded-lg text-sm font-medium text-white gradient-primary transition"
                >
                  {text.register}
                </Link>
              </div>
            </>
          )}
        </div>
      </div>
    </>
  );
}
