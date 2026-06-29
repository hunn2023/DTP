"use client";

import { Suspense, useEffect, useMemo, useRef, useState, useTransition } from "react";
import Icon from "@/components/ui/Icon";
import Link from "next/link";
import Image from "next/image";
import dynamic from "next/dynamic";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import Logo from "./Logo";
import CartIcon from "./CartIcon";
import { useCartAnimation } from "@/components/ui/CartAnimation";
import { useAuth } from "@/hooks/useAuth";
import { useLanguage } from "@/hooks/useLanguage";
import { languageFlagCodes, languageNames, languageShortLabels, type Language } from "@/lib/i18n";
import type { IconName } from "@fortawesome/fontawesome-svg-core";
import CountrySearchBox from "@/components/common/CountrySearchBox";

const MobileMenu = dynamic(() => import("./MobileMenu"), {
  ssr: false,
  loading: () => <div className="lg:hidden w-10 h-10" aria-hidden="true" />,
});

const HeaderUserMenu = dynamic(() => import("./HeaderUserMenu"), {
  ssr: false,
  loading: () => <div className="hidden lg:block w-20 h-8" aria-hidden="true" />,
});

interface MainMenuItem {
  labels: Record<Language, string>;
  href: string;
  icon: IconName;
  matchPath: string;
  matchTab?: string;
  highlight?: boolean;
}

const MAIN_MENU: MainMenuItem[] = [
  { labels: { vi: "Trang chủ", en: "Home" }, icon: "home", href: "/", matchPath: "/" },
  {
    labels: { vi: "eSIM Du lịch", en: "Travel eSIM" },
    icon: "globe-asia",
    href: "/esim-du-lich",
    matchPath: "/esim-du-lich",
  },
  {
    labels: { vi: "Blog", en: "Blog" },
    icon: "blog",
    href: "/blog",
    matchPath: "/blog",
  },
  {
    labels: { vi: "Hỗ trợ", en: "Support" },
    icon: "headset",
    href: "/support",
    matchPath: "/support",
  },
];

function isMenuActive(item: MainMenuItem, pathname: string, currentTab: string | null): boolean {
  if (item.matchPath === "/") return pathname === "/";

  const pathMatches = pathname === item.matchPath || pathname.startsWith(item.matchPath + "/");
  if (!pathMatches) return false;

  if (item.matchTab) {
    const effectiveTab = currentTab ?? (item.matchPath === "/the-nap" ? "telecom" : null);
    return effectiveTab === item.matchTab;
  }

  return true;
}

export default function Header() {
  return (
    <Suspense fallback={<div className="h-16" />}>
      <HeaderInner />
    </Suspense>
  );
}

function HeaderInner() {
  const { cartIconRef } = useCartAnimation();
  const { initialized, isAuthenticated } = useAuth();
  const { language, setLanguage } = useLanguage();
  const router = useRouter();
  const pathname = usePathname() ?? "";
  const searchParams = useSearchParams();
  const currentTab = searchParams.get("tab");
  const [searchOpen, setSearchOpen] = useState(false);
  const [languageOpen, setLanguageOpen] = useState(false);
  const [headerHeight, setHeaderHeight] = useState(0);
  const [, startTransition] = useTransition();
  const languageContainerRef = useRef<HTMLDivElement | null>(null);
  const headerRootRef = useRef<HTMLDivElement | null>(null);

  const text = useMemo(
    () => ({
      appDownload: language === "vi" ? "Tải app EZSIM" : "Download EZSIM app",
      login: language === "vi" ? "Đăng nhập" : "Login",
      register: language === "vi" ? "Đăng ký" : "Register",
      searchPlaceholder: language === "vi" ? "Tìm quốc gia eSIM..." : "Search eSIM countries...",
      searchOpen: language === "vi" ? "Mở tìm kiếm eSIM" : "Open eSIM search",
      searchClose: language === "vi" ? "Đóng tìm kiếm" : "Close search",
      searchNotFound: language === "vi" ? "Không tìm thấy quốc gia phù hợp." : "No matching country found.",
      from: language === "vi" ? "Từ" : "From",
    }),
    [language]
  );

  useEffect(() => {
    if (!languageOpen) return;

    const handleOutsideClick = (event: MouseEvent) => {
      if (!languageContainerRef.current) return;
      if (!languageContainerRef.current.contains(event.target as Node)) {
        setLanguageOpen(false);
      }
    };

    document.addEventListener("mousedown", handleOutsideClick);
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, [languageOpen]);

  useEffect(() => {
    if (!headerRootRef.current) return;

    const updateHeight = () => {
      if (!headerRootRef.current) return;
      setHeaderHeight(headerRootRef.current.getBoundingClientRect().height);
    };

    updateHeight();

    const observer = new ResizeObserver(() => updateHeight());
    observer.observe(headerRootRef.current);
    window.addEventListener("resize", updateHeight);

    return () => {
      observer.disconnect();
      window.removeEventListener("resize", updateHeight);
    };
  }, []);

  const handleLanguageChange = (nextLanguage: Language) => {
    if (nextLanguage === language) {
      setLanguageOpen(false);
      return;
    }

    setLanguage(nextLanguage);
    setLanguageOpen(false);
    startTransition(() => {
      router.refresh();
    });
  };

  return (
    <>
      <div style={{ height: headerHeight }} aria-hidden />

      <div ref={headerRootRef} className="fixed top-0 left-0 right-0 z-[300] bg-white">
        <div className="bg-navy" style={{ color: "#94A3B8", fontSize: "13px", padding: "8px 0" }}>
          <div className="max-w-container mx-auto px-6 flex justify-end items-center">
            <div className="flex items-center gap-5">
              <a href="#" className="hover:text-cyan transition hidden md:flex items-center gap-1.5">
                <Icon icon="mobile-alt" />
                {text.appDownload}
              </a>
              <div ref={languageContainerRef} className="relative hidden md:block">
                <button
                  type="button"
                  onClick={() => setLanguageOpen((open) => !open)}
                  className="hover:text-cyan transition flex items-center gap-1.5"
                  aria-expanded={languageOpen}
                  aria-haspopup="menu"
                >
                  <span className="inline-flex h-4 w-4 overflow-hidden rounded-full" aria-hidden>
                    <Image
                      src={`https://flagcdn.com/w80/${languageFlagCodes[language]}.png`}
                      alt={languageNames[language]}
                      width={16}
                      height={16}
                      quality={100}
                      unoptimized
                      className="h-full w-full object-cover"
                    />
                  </span>
                  {languageShortLabels[language]}
                  <Icon icon="chevron-down" className="text-[10px]" />
                </button>
                {languageOpen && (
                  <div className="absolute right-0 top-[calc(100%+8px)] min-w-[160px] rounded-xl border border-slate-700 bg-slate-900 p-1.5 shadow-xl z-[320]">
                    {(["vi", "en"] as Language[]).map((itemLanguage) => (
                      <button
                        key={itemLanguage}
                        type="button"
                        onClick={() => handleLanguageChange(itemLanguage)}
                        className={`w-full rounded-lg px-2.5 py-2 text-left text-sm flex items-center gap-2.5 transition ${
                          language === itemLanguage ? "bg-white/15 text-white" : "text-slate-200 hover:bg-white/10"
                        }`}
                      >
                        <span className="inline-flex h-4 w-4 overflow-hidden rounded-full" aria-hidden>
                          <Image
                            src={`https://flagcdn.com/w80/${languageFlagCodes[itemLanguage]}.png`}
                            alt={languageNames[itemLanguage]}
                            width={16}
                            height={16}
                            quality={100}
                            unoptimized
                            className="h-full w-full object-cover"
                          />
                        </span>
                        {languageNames[itemLanguage]}
                      </button>
                    ))}
                  </div>
                )}
              </div>
              {initialized && !isAuthenticated && (
                <div className="flex items-center gap-3">
                  <Link href="/login" className="hover:text-cyan transition flex items-center gap-1.5">
                    <Icon icon="user" />
                    {text.login}
                  </Link>
                  <span className="text-gray-500">/</span>
                  <Link href="/register" className="hover:text-cyan transition">
                    {text.register}
                  </Link>
                </div>
              )}
            </div>
          </div>
        </div>

        <header className="bg-white border-b border-gray-200">
          <div className="max-w-container mx-auto px-6 py-4 flex items-center gap-4 lg:gap-6">
            <MobileMenu />
            <Logo />

            <nav className="hidden lg:flex items-center gap-6 xl:gap-8 ml-2">
              {MAIN_MENU.map((item) => {
                const active = isMenuActive(item, pathname, currentTab);
                return (
                  <Link
                    key={item.href}
                    href={item.href}
                    className="font-semibold text-[15px] flex items-center gap-2 transition"
                    style={{ color: active ? "#0066FF" : "#334155" }}
                  >
                    <Icon icon={item.icon} className="text-[15px]" />
                    {item.labels[language]}
                  </Link>
                );
              })}
            </nav>

            <div className="hidden md:flex items-center ml-auto">
              <div
                className={`relative origin-left transition-[width,transform,opacity] duration-700 ease-[cubic-bezier(0.22,1,0.36,1)] ${
                  searchOpen ? "w-[315px] lg:w-[360px] scale-100" : "w-11 scale-[0.98]"
                }`}
              >
                {!searchOpen ? (
                  <button
                    type="button"
                    onClick={() => setSearchOpen(true)}
                    className="w-11 h-11 rounded-xl border border-gray-200 text-gray-600 hover:text-primary hover:border-primary transition flex items-center justify-center"
                    aria-label={text.searchOpen}
                  >
                    <Icon icon="search" />
                  </button>
                ) : (
                  <CountrySearchBox
                    language={language}
                    placeholder={text.searchPlaceholder}
                    notFoundText={text.searchNotFound}
                    fromLabel={text.from}
                    variant="header"
                    autoFocus
                    showCloseButton
                    onClose={() => setSearchOpen(false)}
                  />
                )}
              </div>
            </div>

            <nav className="ml-auto md:ml-0 flex gap-3 lg:gap-4 items-center">
              <HeaderUserMenu />
              <CartIcon ref={cartIconRef} />
            </nav>
          </div>
        </header>
      </div>
    </>
  );
}
