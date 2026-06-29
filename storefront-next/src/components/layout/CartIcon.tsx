"use client";

import { forwardRef } from "react";
import { useEffect, useState } from "react";
import Icon from "@/components/ui/Icon";
import Link from "next/link";
import { useCartStore } from "@/lib/cartStore";
import { useCartAnimation } from "@/components/ui/CartAnimation";

const CartIcon = forwardRef<HTMLAnchorElement, object>(function CartIcon(_props, ref) {
  const items = useCartStore((s) => s.items);
  const { cartImpactCount } = useCartAnimation();
  const [hydrated, setHydrated] = useState(false);
  const [isImpacting, setIsImpacting] = useState(false);

  useEffect(() => {
    const persistApi = useCartStore.persist;
    if (!persistApi) {
      setHydrated(true);
      return;
    }

    const unsubscribeHydrate = persistApi.onHydrate(() => setHydrated(false));
    const unsubscribeFinishHydration = persistApi.onFinishHydration(() => setHydrated(true));
    setHydrated(persistApi.hasHydrated());

    return () => {
      unsubscribeHydrate();
      unsubscribeFinishHydration();
    };
  }, []);

  // Compute count from items - this will re-render when items change
  const count = hydrated ? items.reduce((sum, i) => sum + i.quantity, 0) : 0;

  useEffect(() => {
    if (!cartImpactCount) return;

    setIsImpacting(true);
    const timeoutId = window.setTimeout(() => {
      setIsImpacting(false);
    }, 520);

    return () => window.clearTimeout(timeoutId);
  }, [cartImpactCount]);

  return (
    <Link
      ref={ref}
      href="/cart"
      className={`relative w-10 h-10 rounded-[10px] bg-gray-100 flex items-center justify-center text-navy hover:bg-primary/10 transition ${
        isImpacting ? "cart-impact" : ""
      }`}
      aria-label={`Giỏ hàng (${count} sản phẩm)`}
    >
      <Icon icon="shopping-cart" />
      {count > 0 && (
        <span className="absolute -top-1 -right-1 bg-red-500 text-white w-[18px] h-[18px] rounded-full text-[11px] font-bold flex items-center justify-center">
          {count > 99 ? "99+" : count}
        </span>
      )}
    </Link>
  );
});

export default CartIcon;