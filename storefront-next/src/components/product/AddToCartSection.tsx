"use client";

import { useState, useCallback, useMemo, useRef } from "react";
import { useRouter } from "next/navigation";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCartPlus, faBolt, faMinus, faPlus, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { useCartStore } from "@/lib/cartStore";
import { useCartAnimation } from "@/components/ui/CartAnimation";
import { isAuthenticated } from "@/lib/authStore";

interface Props {
  productId: string;
  productName: string;
  productSlug: string;
  productImage: string;
  productPrice: number;
  stock: number;
  inStock: boolean;
}

export default function AddToCartSection({
  productId,
  productName,
  productSlug,
  productImage,
  productPrice,
  stock,
  inStock,
}: Props) {
  const router = useRouter();
  const addToCart = useCartStore((s) => s.addToCart);
  const { triggerFlyToCart } = useCartAnimation();
  const addButtonRef = useRef<HTMLButtonElement>(null);
  const [quantity, setQuantity] = useState(1);
  const [isAddingToCart, setIsAddingToCart] = useState(false);
  const [isBuyingNow, setIsBuyingNow] = useState(false);

  const decrease = () => setQuantity((q) => Math.max(1, q - 1));
  const increase = () => setQuantity((q) => Math.min(stock, q + 1));

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = Number(e.target.value);
    if (isNaN(val) || val < 1) setQuantity(1);
    else if (val > stock) setQuantity(stock);
    else setQuantity(val);
  };

  const cartItem = useMemo(
    () => ({
      id: productId,
      name: productName,
      slug: productSlug,
      image: productImage,
      price: productPrice,
      quantity,
      stock,
    }),
    [productId, productName, productSlug, productImage, productPrice, quantity, stock]
  );

  const handleAddToCart = useCallback(async () => {
    if (!inStock || isAddingToCart) return;
    if (!isAuthenticated()) {
      router.push(`/login?returnUrl=/esim-du-lich/${productSlug}`);
      return;
    }
    setIsAddingToCart(true);
    try {
      addToCart(cartItem);
      triggerFlyToCart(productImage, addButtonRef.current);
    } finally {
      setIsAddingToCart(false);
    }
  }, [inStock, isAddingToCart, addToCart, cartItem, triggerFlyToCart, productImage, productSlug, router]);

  const handleBuyNow = useCallback(async () => {
    if (!inStock || isBuyingNow) return;
    if (!isAuthenticated()) {
      router.push(`/login?returnUrl=/esim-du-lich/${productSlug}`);
      return;
    }
    setIsBuyingNow(true);
    try {
      addToCart(cartItem);
      triggerFlyToCart(productImage, addButtonRef.current);
      router.push("/cart");
    } finally {
      setIsBuyingNow(false);
    }
  }, [inStock, isBuyingNow, addToCart, cartItem, triggerFlyToCart, productImage, productSlug, router]);

  if (!inStock) {
    return (
      <div className="mt-6 p-4 bg-gray-50 rounded-lg text-center">
        <p className="text-gray-500 font-medium">Sản phẩm hiện đang hết hàng</p>
        <p className="text-gray-400 text-sm mt-1">Vui lòng quay lại sau</p>
      </div>
    );
  }

  return (
    <div className="space-y-4 mt-6">
      {/* Quantity selector */}
      <div>
        <label className="text-sm font-medium text-navy mb-2 block">Số lượng</label>
        <div className="flex items-center gap-1">
          <button
            type="button"
            onClick={decrease}
            disabled={quantity <= 1}
            aria-label="Giảm số lượng"
            className="w-10 h-10 flex items-center justify-center rounded-lg border border-gray-200 text-gray-600 hover:bg-gray-100 disabled:opacity-40 disabled:cursor-not-allowed transition"
          >
            <FontAwesomeIcon icon={faMinus} className="text-xs" />
          </button>
          <input
            type="number"
            value={quantity}
            onChange={handleInputChange}
            min={1}
            max={stock}
            aria-label="Số lượng sản phẩm"
            className="w-16 h-10 text-center border border-gray-200 rounded-lg text-sm font-medium outline-none focus:border-primary"
          />
          <button
            type="button"
            onClick={increase}
            disabled={quantity >= stock}
            aria-label="Tăng số lượng"
            className="w-10 h-10 flex items-center justify-center rounded-lg border border-gray-200 text-gray-600 hover:bg-gray-100 disabled:opacity-40 disabled:cursor-not-allowed transition"
          >
            <FontAwesomeIcon icon={faPlus} className="text-xs" />
          </button>
          <span className="text-xs text-gray-400 ml-2">({stock} có sẵn)</span>
        </div>
      </div>

      {/* Action buttons */}
      <div className="flex flex-col sm:flex-row gap-3">
        <button
          ref={addButtonRef}
          type="button"
          onClick={handleAddToCart}
          disabled={isAddingToCart}
          className="flex-1 btn-outline disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isAddingToCart ? (
            <FontAwesomeIcon icon={faSpinner} className="animate-spin" />
          ) : (
            <FontAwesomeIcon icon={faCartPlus} />
          )}
          <span>{isAddingToCart ? "Đang thêm..." : "Thêm vào giỏ"}</span>
        </button>
        <button
          type="button"
          onClick={handleBuyNow}
          disabled={isBuyingNow}
          className="flex-1 btn-primary btn-lg disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isBuyingNow ? (
            <FontAwesomeIcon icon={faSpinner} className="animate-spin" />
          ) : (
            <FontAwesomeIcon icon={faBolt} />
          )}
          <span>{isBuyingNow ? "Đang xử lý..." : "Mua ngay"}</span>
        </button>
      </div>
    </div>
  );
}
