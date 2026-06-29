import type { Language } from "@/lib/i18n";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

export interface FaqItem {
  id: string;
  question: string;
  answer: string;
}

/** Public FAQ as returned by /api/content/faqs (PublicContentFaqs). */
interface ApiContentFaq {
  id: string;
  question: string | null;
  answer: string | null;
  categoryCode: string | null;
  sortOrder?: number;
  isActive?: boolean;
}

interface ApiResult<T> {
  isSuccess: boolean;
  data: T | null;
  error: unknown;
}

function mapFaq(faq: ApiContentFaq): FaqItem {
  return {
    id: faq.id,
    question: faq.question ?? "",
    answer: faq.answer ?? "",
  };
}

/**
 * Fetch published FAQs. The backend has a single language, so the `language`
 * argument is accepted for call-site symmetry but does not change the request.
 */
export async function getFaqs(_language: Language = "vi"): Promise<FaqItem[]> {
  try {
    const response = await fetchWithAuth("/api/content/faqs");
    if (!response.ok) return [];

    const json = (await response.json()) as ApiResult<ApiContentFaq[]>;
    const items = Array.isArray(json?.data) ? json.data : [];

    return items
      .slice()
      .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0))
      .map(mapFaq)
      .filter((faq) => faq.question);
  } catch {
    return [];
  }
}
