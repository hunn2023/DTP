import type { Language } from "@/lib/i18n";

export function formatPublishedDate(iso: string, language: Language = "vi"): string {
  return new Date(iso).toLocaleDateString(language === "vi" ? "vi-VN" : "en-US", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}
