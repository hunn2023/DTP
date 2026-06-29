export type CardMarketplaceTab = "telecom" | "game" | "data" | "promo";
export type CardPaymentMethod = "Momo" | "ZaloPay" | "VNPay" | "Banking" | "QR Code" | "Số dư EZSIM";

export interface CardDenomination {
  face: number;
  pay: number;
  label?: string;
  description?: string;
  phoneCardId?: string;
  productVariantId?: string;
}

export interface CardProvider {
  id: string;
  category: CardMarketplaceTab;
  name: string;
  letter: string;
  bg: string;
  textColor?: string;
  discountPercent: number;
  discountLabel: string;
  description: string;
  deliveryTime: string;
  denominations: CardDenomination[];
}

export interface CardMarketplaceContent {
  tab: CardMarketplaceTab;
  breadcrumb: string;
  pageTitle: string;
  pageSubtitle: string;
  step1Title: string;
  step1Desc: string;
  step2Title: string;
  step2Desc: string;
  productLabel: string;
  itemUnit: string;
  providers: CardProvider[];
}
