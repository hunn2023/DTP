import { formatPrice, calcDiscountPercent } from "@/lib/product";

interface Props {
  price: number;
  salePrice?: number;
  stock: number;
  inStock: boolean;
}

export default function ProductPrice({ price, salePrice, stock, inStock }: Props) {
  const displayPrice = salePrice ?? price;
  const isOnSale = !!salePrice && salePrice < price;
  const discountPercent = isOnSale ? calcDiscountPercent(price, salePrice) : 0;

  return (
    <div className="space-y-3">
      {/* Price */}
      <div className="flex items-baseline gap-3">
        <span className="text-2xl md:text-3xl font-bold text-primary">
          {formatPrice(displayPrice)}
        </span>
        {isOnSale && (
          <>
            <span className="text-gray-400 text-base line-through">
              {formatPrice(price)}
            </span>
            <span className="bg-danger text-white text-xs font-bold px-2 py-0.5 rounded">
              -{discountPercent}%
            </span>
          </>
        )}
      </div>

      {/* Stock status */}
      <div className="flex items-center gap-2">
        {inStock ? (
          <>
            <span className="w-2 h-2 rounded-full bg-success" />
            <span className="text-sm text-success font-medium">
              Còn hàng ({stock} sản phẩm)
            </span>
          </>
        ) : (
          <>
            <span className="w-2 h-2 rounded-full bg-danger" />
            <span className="text-sm text-danger font-medium">Hết hàng</span>
          </>
        )}
      </div>
    </div>
  );
}
