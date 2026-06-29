import { fetchWithAuth } from "@/lib/fetchWithAuth";

export interface ChatbotProductSuggestion {
  productId: string;
  productVariantId: string;
  esimPackageId: string;
  productName: string | null;
  productSlug: string | null;
  countryName: string | null;
  flagUrl: string | null;
  packageName: string | null;
  providerPackageCode: string | null;
  dataAmount: number | null;
  dataUnit: string | null;
  isUnlimited: boolean;
  validityDays: number;
  salePrice: number;
  currency: string | null;
  coverageDescription: string | null;
  activationPolicy: string | null;
  speedPolicy: string | null;
  hotspotSupported: boolean;
  phoneNumberSupported: boolean;
  smsSupported: boolean;
  score: number;
  buyUrl: string | null;
}

export interface ChatbotResponse {
  sessionId: string | null;
  message: string | null;
  needMoreInfo: boolean;
  missingFields: string[] | null;
  suggestions: ChatbotProductSuggestion[] | null;
}

interface ChatbotRequest {
  sessionId: string | null;
  message: string;
}

interface ApiEnvelope<T> {
  data?: T;
  message?: string;
  error?: string;
}

export class ChatbotApiError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "ChatbotApiError";
  }
}

function getApiErrorMessage(json: ApiEnvelope<ChatbotResponse> | ChatbotResponse | null): string | null {
  if (!json) return null;
  if ("error" in json && typeof json.error === "string") return json.error;
  if ("message" in json && typeof json.message === "string") return json.message;
  return null;
}

export async function sendChatbotMessage(request: ChatbotRequest): Promise<ChatbotResponse> {
  const response = await fetchWithAuth("/api/public/chatbot/message", {
    method: "POST",
    body: JSON.stringify(request),
  });

  const json = (await response.json().catch(() => null)) as ApiEnvelope<ChatbotResponse> | ChatbotResponse | null;

  if (!response.ok) {
    const errorMessage = getApiErrorMessage(json);
    throw new ChatbotApiError(errorMessage ?? "Chatbot đang bận. Vui lòng thử lại sau.");
  }

  if (!json) {
    throw new ChatbotApiError("Chatbot chưa trả về nội dung. Vui lòng thử lại.");
  }

  return "data" in json && json.data ? json.data : (json as ChatbotResponse);
}
