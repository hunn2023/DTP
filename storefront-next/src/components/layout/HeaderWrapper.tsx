"use client";

import { CartAnimationProvider, CartFlyAnimations } from "@/components/ui/CartAnimation";
import Header from "./Header";

export default function HeaderWrapper() {
  return (
    <CartAnimationProvider>
      <Header />
      <CartFlyAnimations />
    </CartAnimationProvider>
  );
}