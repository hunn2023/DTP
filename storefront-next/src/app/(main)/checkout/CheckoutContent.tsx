"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Breadcrumb from "@/components/ui/Breadcrumb";
import CheckoutForm from "@/components/checkout/CheckoutForm";
import { useCartStore } from "@/lib/cartStore";
import { useLanguage } from "@/hooks/useLanguage";
import { useAuth } from "@/hooks/useAuth";

export default function CheckoutContent() {
  const router = useRouter();
  const items = useCartStore((s) => s.items);
  const { language } = useLanguage();
  const { isAuthenticated, initialized } = useAuth();

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
    const unsubscribeFinishHydration = persistApi.onFinishHydration(() =>
      setHydrated(true)
    );

    setHydrated(persistApi.hasHydrated());

    return () => {
      unsubscribeHydrate();
      unsubscribeFinishHydration();
    };
  }, []);

  useEffect(() => {
    if (!hydrated) return;

    if (items.length === 0) {
      router.replace("/account/orders");
      return;
    }

    if (initialized && !isAuthenticated) {
      router.replace("/login?returnUrl=/checkout");
    }
  }, [hydrated, items.length, initialized, isAuthenticated, router]);

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

  if (items.length === 0) {
    return null;
  }

  if (initialized && !isAuthenticated) {
    return null;
  }

  return (
    <>
      <Breadcrumb
        items={[
          { label: text.cart, href: "/cart" },
          { label: text.checkout },
        ]}
      />

      <section className="max-w-container mx-auto px-4 md:px-6 py-6 md:py-10">
        <h1 className="text-xl md:text-2xl font-bold text-navy mb-6">
          {text.checkout}
        </h1>

        <CheckoutForm />
      </section>
    </>
  );
}

