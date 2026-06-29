export interface Product {
  id: string;
  name: string;
  slug: string;
  image: string;
  price: number;
  originalPrice?: number;
  category: string;
  inStock: boolean;
  badge?: string;
  description?: string;
}

export interface ProductDetail extends Product {
  images: string[];
  sku: string;
  salePrice?: number;
  stock: number;
  categoryId: string;
  longDescription?: string;
}

export interface ProductCardProps {
  product: Product;
  onAddToCart?: (product: Product) => void;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  description: string;
  image?: string;
}

export interface PaginatedProducts {
  products: Product[];
  totalPages: number;
}

export interface ProductQueryParams {
  sort?: string;
  minPrice?: string;
  maxPrice?: string;
  pageIndex?: string;
}
