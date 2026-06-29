export interface ProductContent {
  id: string;
  productId: string;
  contentType: number;
  contentTypeName?: string | null;
  title?: string | null;
  summary?: string | null;
  bodyHtml?: string | null;
  sortOrder: number;
  isActive: boolean;
}

export interface ProductFaq {
  id: string;
  productId: string;
  question: string;
  answer: string;
  sortOrder: number;
  isActive: boolean;
}

