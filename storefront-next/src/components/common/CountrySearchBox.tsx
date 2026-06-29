"use client";

import { FormEvent, useCallback, useDeferredValue, useEffect, useMemo, useRef, useState } from "react";
import Image from "next/image";
import { useRouter } from "next/navigation";
import Icon from "@/components/ui/Icon";
import type { Language } from "@/lib/i18n";
import { removeDiacritics } from "@/lib/text";
import { getEsimCountries } from "@/lib/api/esimApi";

type CountrySearchItem = {
  slug: string;
  name: string;
  flagUrl: string | null;
  region: string | null;
  priceFrom: number;
};

interface CountrySearchBoxProps {
  language: Language;
  placeholder: string;
  notFoundText: string;
  fromLabel: string;
  submitLabel?: string;
  variant?: "header" | "hero";
  autoFocus?: boolean;
  showCloseButton?: boolean;
  onClose?: () => void;
}

export default function CountrySearchBox({
  language,
  placeholder,
  notFoundText,
  fromLabel,
  submitLabel,
  variant = "header",
  autoFocus = false,
  showCloseButton = false,
  onClose,
}: CountrySearchBoxProps) {
  const router = useRouter();
  const [keyword, setKeyword] = useState("");
  const [open, setOpen] = useState(false);
  const [countries, setCountries] = useState<CountrySearchItem[]>([]);
  const deferredKeyword = useDeferredValue(keyword);
  const rootRef = useRef<HTMLDivElement | null>(null);
  const fetchedRef = useRef(false);

  const fetchCountries = useCallback(async () => {
    if (fetchedRef.current) return;
    fetchedRef.current = true;
    try {
      const summaries = await getEsimCountries();
      setCountries(
        summaries.map((item) => ({
          slug: item.slug,
          name: item.name,
          flagUrl: item.flag || null,
          region: item.region,
          priceFrom: item.startingPrice ?? 0,
        }))
      );
    } catch { /* ignore */ }
  }, []);

  useEffect(() => {
    fetchCountries();
  }, [fetchCountries]);

  useEffect(() => {
    if (!open) return;

    const handleOutsideClick = (event: MouseEvent) => {
      if (!rootRef.current) return;
      if (!rootRef.current.contains(event.target as Node)) {
        setOpen(false);
        onClose?.();
      }
    };

    document.addEventListener("mousedown", handleOutsideClick);
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, [open, onClose]);

  const filteredResults = useMemo(() => {
    const normalizedKeyword = removeDiacritics(deferredKeyword.trim());
    const baseList = normalizedKeyword
      ? countries.filter((country) =>
          removeDiacritics(country.name).includes(normalizedKeyword) ||
          removeDiacritics(country.region ?? "").includes(normalizedKeyword)
        )
      : countries;

    return baseList.slice(0, 6);
  }, [deferredKeyword, countries]);

  const goToCountry = (slug: string) => {
    setOpen(false);
    setKeyword("");
    router.push(`/esim-du-lich/${slug}`);
    onClose?.();
  };

  const handleSearchSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const search = keyword.trim();

    if (!search) {
      router.push("/esim-du-lich");
      setOpen(false);
      onClose?.();
      return;
    }

    const normalizedSearch = removeDiacritics(search);
    const exactMatch = countries.find(
      (country) => removeDiacritics(country.name) === normalizedSearch
    );

    if (exactMatch) {
      goToCountry(exactMatch.slug);
      return;
    }

    router.push(`/esim-du-lich?q=${encodeURIComponent(search)}`);
    setOpen(false);
    onClose?.();
  };

  const getFlagSrc = (country: CountrySearchItem) => country.flagUrl;

  return (
    <div ref={rootRef} className="relative">
      <form
        onSubmit={handleSearchSubmit}
        className={variant === "hero" ? "bg-white flex flex-col sm:flex-row gap-2" : "relative"}
        style={
          variant === "hero"
            ? {
                borderRadius: "16px",
                padding: "6px",
                boxShadow: "0 20px 60px rgba(0,0,0,0.2)",
              }
            : undefined
        }
      >
        {variant === "header" && (
          <Icon icon="search" className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500" />
        )}
        <input
          type="text"
          value={keyword}
          onFocus={() => setOpen(true)}
          onChange={(event) => {
            setKeyword(event.target.value);
            setOpen(true);
          }}
          autoFocus={autoFocus}
          placeholder={placeholder}
          className={
            variant === "hero"
              ? "flex-1 border-none outline-none text-navy font-sans text-[16px] sm:text-[14px]"
              : "w-full py-3 pl-11 pr-10 border-[1.5px] border-gray-200 rounded-xl text-[16px] sm:text-[13px] font-sans outline-none focus:border-primary transition"
          }
          style={variant === "hero" ? { padding: "13px 16px" } : undefined}
        />

        {variant === "hero" && submitLabel && (
          <button
            type="submit"
            className="bg-navy text-white font-semibold hover:opacity-90 transition flex items-center justify-center gap-2 w-full sm:w-auto"
            style={{
              padding: "11px 16px",
              borderRadius: "10px",
              fontSize: "14px",
            }}
          >
            <Icon icon="search" /> {submitLabel}
          </button>
        )}

        {variant === "header" && showCloseButton && (
          <button
            type="button"
            onClick={() => {
              setKeyword("");
              setOpen(false);
              onClose?.();
            }}
            className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-navy transition"
            aria-label={language === "vi" ? "Đóng tìm kiếm" : "Close search"}
          >
            <Icon icon="times" className="text-sm" />
          </button>
        )}
      </form>

      {open && (
        <div
          className={`absolute left-0 right-0 bg-white border border-gray-200 rounded-xl shadow-lg p-2 max-h-[320px] overflow-y-auto ${
            variant === "header" ? "z-10" : "z-30"
          }`}
          style={{ top: "calc(100% + 8px)" }}
        >
          {filteredResults.length > 0 ? (
            filteredResults.map((country) => {
              const flagSrc = getFlagSrc(country);
              return (
                <button
                  key={country.slug}
                  type="button"
                  onClick={() => goToCountry(country.slug)}
                  className="w-full text-left flex items-center gap-3 px-2 py-2.5 rounded-lg hover:bg-slate-50 transition"
                >
                  <span
                    className="flex h-6 w-9 items-center justify-center rounded-md bg-slate-100 shrink-0 overflow-hidden border border-slate-200"
                    aria-hidden
                  >
                    {flagSrc && (
                      <Image
                        src={flagSrc}
                        alt={country.name}
                        width={36}
                        height={24}
                        className="h-full w-full object-cover"
                      />
                    )}
                  </span>
                  <span className="flex-1 min-w-0">
                    <span className="block text-sm font-semibold text-navy truncate">
                      eSIM {country.name}
                    </span>
                    <span className="block text-xs text-gray-500 truncate">
                      {country.region}
                    </span>
                  </span>
                  {country.priceFrom > 0 && (
                    <span className="text-xs text-primary font-semibold whitespace-nowrap">
                      {fromLabel} {country.priceFrom.toLocaleString("vi-VN")}đ
                    </span>
                  )}
                </button>
              );
            })
          ) : (
            <p className="text-sm text-gray-500 px-2 py-3">{notFoundText}</p>
          )}
        </div>
      )}
    </div>
  );
}
