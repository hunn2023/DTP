"use client";

import { useCallback, useEffect, useState } from "react";
import { LANGUAGE_COOKIE, Language, normalizeLanguage, getLanguageCookieValue } from "@/lib/i18n";

const LANGUAGE_CHANGE_EVENT = "ezsim-language-change";

export function useLanguage() {
  // Keep the first client render equal to server-rendered fallback to avoid hydration mismatch.
  const [language, setLanguageState] = useState<Language>("vi");

  useEffect(() => {
    const fromCookie = getLanguageCookieValue(document.cookie);
    const fromStorage = normalizeLanguage(window.localStorage.getItem(LANGUAGE_COOKIE));
    const nextLanguage = fromCookie || fromStorage;
    setLanguageState(nextLanguage);

    const handleStorage = (event: StorageEvent) => {
      if (event.key !== LANGUAGE_COOKIE) return;
      setLanguageState(normalizeLanguage(event.newValue));
    };

    const handleCustomChange = (event: Event) => {
      const detail = (event as CustomEvent<Language>).detail;
      if (!detail) return;
      setLanguageState(normalizeLanguage(detail));
    };

    window.addEventListener("storage", handleStorage);
    window.addEventListener(LANGUAGE_CHANGE_EVENT, handleCustomChange as EventListener);

    return () => {
      window.removeEventListener("storage", handleStorage);
      window.removeEventListener(LANGUAGE_CHANGE_EVENT, handleCustomChange as EventListener);
    };
  }, []);

  const setLanguage = useCallback((nextLanguage: Language) => {
    const expires = new Date();
    expires.setFullYear(expires.getFullYear() + 1);

    document.cookie = `${LANGUAGE_COOKIE}=${encodeURIComponent(nextLanguage)}; path=/; expires=${expires.toUTCString()}; SameSite=Lax`;
    window.localStorage.setItem(LANGUAGE_COOKIE, nextLanguage);
    setLanguageState(nextLanguage);
    window.dispatchEvent(new CustomEvent<Language>(LANGUAGE_CHANGE_EVENT, { detail: nextLanguage }));
  }, []);

  return { language, setLanguage };
}
