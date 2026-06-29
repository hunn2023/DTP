export type Language = "vi" | "en";

export const LANGUAGE_COOKIE = "ezsim-lang";

export const SUPPORTED_LANGUAGES: Language[] = ["vi", "en"];

export function isSupportedLanguage(value: string | null | undefined): value is Language {
  return value === "vi" || value === "en";
}

export function normalizeLanguage(value: string | null | undefined): Language {
  return isSupportedLanguage(value) ? value : "vi";
}

export function getLanguageCookieValue(cookieString: string): Language {
  const match = cookieString.match(/(?:^|;\s*)ezsim-lang=([^;]+)/);
  const rawValue = match ? decodeURIComponent(match[1]) : undefined;
  return normalizeLanguage(rawValue);
}

export const languageNames: Record<Language, string> = {
  vi: "Tiếng Việt",
  en: "English",
};

export const languageShortLabels: Record<Language, string> = {
  vi: "VI",
  en: "EN",
};

export const languageFlagCodes: Record<Language, string> = {
  vi: "vn",
  en: "gb",
};
