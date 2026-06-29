"use client";

import { useState, useCallback } from "react";
import Image from "next/image";
import Link from "next/link";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCartPlus, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { Product, ProductCardProps } from "@/types/product";
import { formatPrice, calcDiscountPercent } from "@/lib/product";

const FALLBACK_IMAGE = "/images/product-placeholder.svg";

export default function ProductCard({ product, onAddToCart, hidePrice = false }: ProductCardProps & { hidePrice?: boolean }) {
  const [imgError, setImgError] = useState(false);
  const [isAdding, setIsAdding] = useState(false);

  const { name, slug, image, price, originalPrice, inStock, badge } = product;
  const isOnSale = !!originalPrice && originalPrice > price;
  const discountPercent = isOnSale
    ? calcDiscountPercent(originalPrice, price)
    : 0;

  const handleAddToCart = useCallback(
    async (e: React.MouseEvent<HTMLButtonElement>) => {
      e.preventDefault();
      e.stopPropagation();
      if (!inStock || isAdding || !onAddToCart) return;

      setIsAdding(true);
      try {
        await onAddToCart(product);
      } finally {
        setIsAdding(false);
      }
    },
    [inStock, isAdding, onAddToCart, product]
  );

  return (
    <Link
      href={`/esim-du-lich/${slug}`}
      className="group product-card relative focus-visible:outline-2 focus-visible:outline-primary focus-visible:outline-offset-2"
      aria-label={`Xem chi tiết ${name}`}
    >
      {/* Image */}
      <div className="relative aspect-[4/3] overflow-hidden bg-gray-100 rounded-t-[20px]">
        {imgError ? (
          <Image
            src={FALLBACK_IMAGE}
            alt={name}
            fill
            className="object-cover"
          />
        ) : (
          <Image
            src={image}
            alt={name}
            fill
            sizes="(max-width: 640px) 50vw, (max-width: 1024px) 33vw, 25vw"
            className="object-cover transition-transform duration-300 group-hover:scale-105"
            onError={() => setImgError(true)}
          />
        )}

        {/* Out of stock overlay */}
        {!inStock && (
          <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
            <span className="text-white font-semibold text-sm bg-black/70 px-3 py-1 rounded">
              Hết hàng
            </span>
          </div>
        )}

        {/* Badges hidden in listing view per mockup - only show on hover or detail page */}
        {!hidePrice && (
          <>
            {/* Sale badge */}
            {isOnSale && inStock && (
              <span className="absolute top-2 left-2 bg-danger text-white text-xs font-bold px-2 py-0.5 rounded opacity-0 group-hover:opacity-100 transition-opacity">
                -{discountPercent}%
              </span>
            )}

            {/* Custom badge */}
            {badge && !isOnSale && (
              <span className="absolute top-2 left-2 bg-secondary text-white text-xs font-bold px-2 py-0.5 rounded opacity-0 group-hover:opacity-100 transition-opacity">
                {badge}
              </span>
            )}
          </>
        )}
      </div>

      {/* Body - mockup exact: 24px padding */}
      <div className="p-6">
        <h3 className="product-card-title">{name}</h3>

        {/* Price - hidden in listing per mockup, show on hover */}
        {!hidePrice && (
          <div className="flex items-baseline gap-2 mt-3 opacity-0 group-hover:opacity-100 transition-opacity">
            <span className="product-card-price">{formatPrice(price)}</span>
            {isOnSale && (
              <span className="text-gray-400 text-xs line-through">
                {formatPrice(originalPrice)}
              </span>
            )}
          </div>
        )}

        {/* Add to cart button - show on hover */}
        {!hidePrice && (
          <button
            type="button"
            onClick={handleAddToCart}
            disabled={!inStock || isAdding}
            aria-label={
              !inStock
                ? "Sản phẩm hết hàng"
                : isAdding
                ? "Đang thêm vào giỏ"
                : `Thêm ${name} vào giỏ hàng`
            }
            className="mt-3 w-full btn-primary btn-sm disabled:opacity-50 disabled:cursor-not-allowed opacity-0 group-hover:opacity-100 transition-opacity"
          >
            {isAdding ? (
              <FontAwesomeIcon icon={faSpinner} className="animate-spin" />
            ) : (
              <FontAwesomeIcon icon={faCartPlus} />
            )}
            <span>{isAdding ? "Đang thêm..." : "Thêm vào giỏ"}</span>
          </button>
        )}
      </div>
    </Link>
  );
}
