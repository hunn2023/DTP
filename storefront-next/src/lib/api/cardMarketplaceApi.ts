import type {
  CardMarketplaceContent,
  CardMarketplaceTab,
  CardProvider,
  CardDenomination,
} from "@/types/cardMarketplace";
import type { ApiPhoneCard, PaginatedResponse } from "@/types/api";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

// ─── Content Metadata (static — not from API) ────────────────────────────────

const contentMeta: Record<CardMarketplaceTab, Omit<CardMarketplaceContent, "providers">> = {
  telecom: {
    tab: "telecom",
    breadcrumb: "Mua Thẻ Viễn Thông",
    pageTitle: "Mua thẻ điện thoại & thẻ game online",
    pageSubtitle:
      "Nạp tiền điện thoại tất cả nhà mạng, mua thẻ game Garena, Zing, Steam, Vcoin... Chiết khấu cao - Nhận mã trong 30 giây.",
    step1Title: "Bước 1: Chọn nhà mạng",
    step1Desc: "Chọn nhà mạng bạn muốn nạp tiền với chiết khấu hấp dẫn.",
    step2Title: "Bước 2: Chọn mệnh giá",
    step2Desc: "Mức chiết khấu và giá thanh toán thay đổi theo từng nhà mạng.",
    productLabel: "Thẻ",
    itemUnit: "thẻ",
  },
  game: {
    tab: "game",
    breadcrumb: "Mua Thẻ Game",
    pageTitle: "Mua thẻ điện thoại & thẻ game online",
    pageSubtitle:
      "Nạp tiền điện thoại tất cả nhà mạng, mua thẻ game Garena, Zing, Steam, Vcoin... Chiết khấu cao - Nhận mã trong 30 giây.",
    step1Title: "Bước 1: Chọn nhà cung cấp game",
    step1Desc: "Nhiều nhà cung cấp game phổ biến tại Việt Nam.",
    step2Title: "Bước 2: Chọn mệnh giá thẻ",
    step2Desc: "Mức chiết khấu và giá thanh toán thay đổi theo từng nhà phát hành.",
    productLabel: "Thẻ",
    itemUnit: "thẻ",
  },
  data: {
    tab: "data",
    breadcrumb: "Mua Data 4G/5G",
    pageTitle: "Mua gói Data 4G/5G online - Kích hoạt siêu nhanh",
    pageSubtitle:
      "Chọn gói data Viettel, Vinaphone, Mobifone, Vietnamobile theo ngày hoặc theo nhu cầu.",
    step1Title: "Bước 1: Chọn nhà mạng data",
    step1Desc: "Các nhóm gói data phổ biến, phù hợp từ lướt web cơ bản đến xem video cả tháng.",
    step2Title: "Bước 2: Chọn gói phù hợp",
    step2Desc: "Mỗi gói hiển thị rõ dung lượng, thời hạn và giá thanh toán sau ưu đãi.",
    productLabel: "Gói",
    itemUnit: "gói",
  },
  promo: {
    tab: "promo",
    breadcrumb: "Khuyến mãi EZSIM",
    pageTitle: "Khuyến mãi hot mỗi ngày - Deal tốt cho eSIM, thẻ và data",
    pageSubtitle:
      "Tổng hợp deal giờ vàng, voucher cho khách mới, combo gaming và các ưu đãi theo mùa.",
    step1Title: "Bước 1: Chọn chương trình ưu đãi",
    step1Desc: "Các deal đang hoạt động, tối ưu cho khách mua thẻ, data và eSIM.",
    step2Title: "Bước 2: Chọn mức ưu đãi",
    step2Desc: "Mỗi deal có nhiều mức giảm khác nhau theo giá trị đơn hàng hoặc gói combo.",
    productLabel: "Deal",
    itemUnit: "deal",
  },
};

// ─── Mapper ───────────────────────────────────────────────────────────────────

function groupPhoneCardsToProviders(cards: ApiPhoneCard[]): CardProvider[] {
  const providerMap = new Map<string, { name: string; cards: ApiPhoneCard[] }>();

  for (const card of cards) {
    const providerName = card.provider?.name || "Khác";
    const providerId = card.providerId;
    if (!providerMap.has(providerId)) {
      providerMap.set(providerId, { name: providerName, cards: [] });
    }
    providerMap.get(providerId)!.cards.push(card);
  }

  const providers: CardProvider[] = [];
  const entries = Array.from(providerMap.entries());
  for (const [id, { name, cards: provCards }] of entries) {
    const denominations: CardDenomination[] = provCards.map((c: ApiPhoneCard) => ({
      face: c.faceValue,
      pay: c.price,
      label: c.name,
      phoneCardId: c.id,
      productVariantId: c.productVariantId,
    }));

    // Calculate discount from first card
    const firstCard = provCards[0];
    const discountPercent = firstCard
      ? Math.round((1 - firstCard.price / firstCard.faceValue) * 100)
      : 0;

    providers.push({
      id,
      category: "telecom", // Will be overridden by caller context
      name,
      letter: name.charAt(0).toUpperCase(),
      bg: getProviderColor(name),
      discountPercent: Math.abs(discountPercent),
      discountLabel: `Giảm ${Math.abs(discountPercent)}%`,
      description: `Thẻ ${name} - Chiết khấu ${Math.abs(discountPercent)}%`,
      deliveryTime: "30 giây",
      denominations,
    });
  }

  return providers;
}

function getProviderColor(name: string): string {
  const colors: Record<string, string> = {
    Viettel: "bg-red-500",
    Vinaphone: "bg-blue-600",
    Mobifone: "bg-green-600",
    Vietnamobile: "bg-yellow-500",
    Garena: "bg-orange-500",
    Steam: "bg-gray-700",
  };
  return colors[name] || "bg-indigo-500";
}

// ─── API Calls ────────────────────────────────────────────────────────────────

export async function getCardMarketplaceContent(tab: CardMarketplaceTab): Promise<CardMarketplaceContent> {
  const meta = contentMeta[tab] ?? contentMeta.telecom;

  try {
    const response = await fetchWithAuth(
      "/api/catalog/phone-cards?PageIndex=1&PageSize=100"
    );
    if (!response.ok) {
      return { ...meta, providers: [] };
    }
    const json = await response.json();
    const payload = json.data ?? json;
    const items: ApiPhoneCard[] = Array.isArray(payload) ? payload : payload.items ?? [];

    const providers = groupPhoneCardsToProviders(items);
    // Set category on each provider
    providers.forEach((p) => { p.category = tab; });

    return { ...meta, providers };
  } catch {
    return { ...meta, providers: [] };
  }
}

export async function getCardTabCounts(): Promise<Record<CardMarketplaceTab, number>> {
  try {
    const response = await fetchWithAuth(
      "/api/catalog/phone-cards?PageIndex=1&PageSize=200"
    );
    if (!response.ok) return { telecom: 0, game: 0, data: 0, promo: 0 };
    const json = await response.json();
    const payload = json.data ?? json;
    const items: ApiPhoneCard[] = Array.isArray(payload) ? payload : payload.items ?? [];

    // For now, all cards count as telecom — backend may add category field later
    return {
      telecom: items.length,
      game: 0,
      data: 0,
      promo: 0,
    };
  } catch {
    return { telecom: 0, game: 0, data: 0, promo: 0 };
  }
}
