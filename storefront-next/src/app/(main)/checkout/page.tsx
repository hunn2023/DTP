"use client";

import { useEffect, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import Breadcrumb from "@/components/ui/Breadcrumb";
import CheckoutForm from "@/components/checkout/CheckoutForm";
import { useCartStore } from "@/lib/cartStore";
import { useLanguage } from "@/hooks/useLanguage";
import { useAuth } from "@/hooks/useAuth";

export default function CheckoutPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const items = useCartStore((s) => s.items);
  const buyNowItem = useCartStore((s) => s.buyNowItem);
  const { language } = useLanguage();
  const { isAuthenticated, initialized } = useAuth();
  const isBuyNow = searchParams.get("buyNow") === "1";
  const checkoutItems = isBuyNow && buyNowItem ? [buyNowItem] : items;

  const text = {
    cart: language === "vi" ? "Giỏ hàng" : "Cart",
    checkout: language === "vi" ? "Thanh toán" : "Checkout",
  };

  const [hydrated, setHydrated] = useState(false);

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

  // Show skeleton only while persisted cart state is hydrating.
  if (!hydrated) {
    return (
      <>
        <Breadcrumb
          items={[
            { label: text.cart, href: "/cart" },
            { label: text.checkout },
          ]}
        />
        <section className="max-w-container mx-auto px-4 md:px-6 py-8">
          <div className="animate-pulse space-y-4">
            <div className="h-8 bg-gray-200 rounded w-48" />
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
              <div className="lg:col-span-2 space-y-4">
                <div className="h-48 bg-gray-100 rounded-xl" />
                <div className="h-32 bg-gray-100 rounded-xl" />
              </div>
              <div className="h-64 bg-gray-100 rounded-xl" />
            </div>
          </div>
        </section>
      </>
    );
  }

  // Cart empty guard - redirect to order history
  if (checkoutItems.length === 0) {
    router.push("/account/orders");
    return null;
  }

  // Auth guard — redirect to login then come back to checkout
  if (initialized && !isAuthenticated) {
    router.replace(`/login?returnUrl=${encodeURIComponent(isBuyNow ? "/checkout?buyNow=1" : "/checkout")}`);
    return null;
  }

  return (
    <>
      <Breadcrumb
        items={[{ label: text.cart, href: "/cart" }, { label: text.checkout }]}
      />

      <section className="max-w-container mx-auto px-4 md:px-6 py-6 md:py-10">
        <h1 className="text-xl md:text-2xl font-bold text-navy mb-6">
          {text.checkout}
        </h1>
        <CheckoutForm checkoutItems={checkoutItems} isBuyNow={isBuyNow} />
      </section>
    </>
  );
}
