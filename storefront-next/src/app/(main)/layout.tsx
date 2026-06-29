import { Suspense } from "react";
import { Footer, Header } from "@/components/layout";
import { CartAnimationProvider, CartFlyAnimations } from "@/components/ui/CartAnimation";

export default function MainLayout({ children }: { children: React.ReactNode }) {
  return (
    <CartAnimationProvider>
      <Suspense fallback={<div className="h-[129px] bg-white border-b border-gray-200" />}>
        <Header />
      </Suspense>
      <main className="flex-1 overflow-x-hidden">{children}</main>
      <CartFlyAnimations />
      <Footer />
    </CartAnimationProvider>
  );
}
