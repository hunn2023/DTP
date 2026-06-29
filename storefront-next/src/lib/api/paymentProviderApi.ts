import { fetchWithAuth } from "@/lib/fetchWithAuth";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "";

export interface PaymentProvider {
  code: string;
  name: string;
  paymentMethod: string;
  currency: string;
  isDefault: boolean;
  minAmount: number;
  maxAmount: number;
  logoUrl: string | null;
  description: string | null;
}

interface ApiResult<T> {
  isSuccess: boolean;
  data: T | null;
  error?: unknown;
}

export function resolvePaymentProviderLogoUrl(logoUrl: string | null | undefined): string | null {
  if (!logoUrl) return null;
  if (logoUrl.startsWith("http://") || logoUrl.startsWith("https://")) return logoUrl;
  return `${API_BASE_URL}${logoUrl.startsWith("/") ? logoUrl : `/${logoUrl}`}`;
}

/** Active payment providers from GET /api/public/payment-providers */
export async function getPaymentProviders(): Promise<PaymentProvider[]> {
  try {
    const response = await fetchWithAuth("/api/public/payment-providers");
    if (!response.ok) return [];

    const json = (await response.json()) as ApiResult<PaymentProvider[]>;
    const items = Array.isArray(json?.data) ? json.data : [];

    return items
      .slice()
      .sort((a, b) => Number(b.isDefault) - Number(a.isDefault));
  } catch {
    return [];
  }
}

export function isProviderAmountEligible(provider: PaymentProvider, amount: number): boolean {
  if (!Number.isFinite(amount) || amount <= 0) return true;
  return amount >= provider.minAmount && amount <= provider.maxAmount;
}
