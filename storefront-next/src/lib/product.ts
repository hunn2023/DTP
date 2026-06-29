export function formatPrice(price: number): string {
  return price.toLocaleString("vi-VN") + "đ";
}

export function calcDiscountPercent(originalPrice: number, salePrice: number): number {
  if (originalPrice <= 0) return 0;
  return Math.round(((originalPrice - salePrice) / originalPrice) * 100);
}
