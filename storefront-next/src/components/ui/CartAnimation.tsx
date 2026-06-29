"use client";

import { createContext, useContext, useState, useCallback, useRef, ReactNode, useEffect } from "react";
import Image from "next/image";

interface FlyAnimation {
  id: string;
  image: string;
  startX: number;
  startY: number;
}

interface CartAnimationContextType {
  triggerFlyToCart: (image: string, startElement: HTMLElement | null) => void;
  flyAnimations: FlyAnimation[];
  removeAnimation: (id: string) => void;
  cartIconRef: React.RefObject<HTMLAnchorElement>;
  cartImpactCount: number;
  markCartImpact: () => void;
}

const CartAnimationContext = createContext<CartAnimationContextType | null>(null);

export function useCartAnimation() {
  const context = useContext(CartAnimationContext);
  if (!context) {
    throw new Error("useCartAnimation must be used within CartAnimationProvider");
  }
  return context;
}

export function CartAnimationProvider({ children }: { children: ReactNode }) {
  const [flyAnimations, setFlyAnimations] = useState<FlyAnimation[]>([]);
  const [cartImpactCount, setCartImpactCount] = useState(0);
  const cartIconRef = useRef<HTMLAnchorElement>(null);

  const triggerFlyToCart = useCallback((image: string, startElement: HTMLElement | null) => {
    if (!startElement || !cartIconRef.current) return;

    const startRect = startElement.getBoundingClientRect();

    const startX = startRect.left + startRect.width / 2;
    const startY = startRect.top + startRect.height / 2;

    const animationId = `${Date.now()}-${Math.random()}`;

    setFlyAnimations((prev) => [...prev, { id: animationId, image, startX, startY }]);
  }, []);

  const removeAnimation = useCallback((id: string) => {
    setFlyAnimations((prev) => prev.filter((a) => a.id !== id));
  }, []);

  const markCartImpact = useCallback(() => {
    setCartImpactCount((count) => count + 1);
  }, []);

  return (
    <CartAnimationContext.Provider
      value={{ triggerFlyToCart, flyAnimations, removeAnimation, cartIconRef, cartImpactCount, markCartImpact }}
    >
      {children}
    </CartAnimationContext.Provider>
  );
}

// Component to render flying items
export function CartFlyAnimations() {
  const { flyAnimations, removeAnimation, cartIconRef, markCartImpact } = useCartAnimation();

  const handleComplete = useCallback(
    (id: string) => {
      removeAnimation(id);
      markCartImpact();
    },
    [removeAnimation, markCartImpact]
  );

  return (
    <>
      {flyAnimations.map((animation) => (
        <CartFlyingItem
          key={animation.id}
          animation={animation}
          cartIconRef={cartIconRef}
          onComplete={() => handleComplete(animation.id)}
        />
      ))}
    </>
  );
}

interface CartFlyingItemProps {
  animation: FlyAnimation;
  cartIconRef: React.RefObject<HTMLAnchorElement>;
  onComplete: () => void;
}

function CartFlyingItem({ animation, cartIconRef, onComplete }: CartFlyingItemProps) {
  const [style, setStyle] = useState({
    left: animation.startX,
    top: animation.startY,
    scale: 1,
    opacity: 1,
  });

  useEffect(() => {
    if (!cartIconRef.current) {
      onComplete();
      return;
    }

    const cartRect = cartIconRef.current.getBoundingClientRect();
    const targetX = cartRect.left + cartRect.width / 2;
    const targetY = cartRect.top + cartRect.height / 2;

    // Animation timeline
    const duration = 600; // ms
    const startTime = Date.now();

    const animate = () => {
      const elapsed = Date.now() - startTime;
      const progress = Math.min(elapsed / duration, 1);

      // Ease out cubic
      const eased = 1 - Math.pow(1 - progress, 3);

      const currentX = animation.startX + (targetX - animation.startX) * eased;
      const currentY = animation.startY + (targetY - animation.startY) * eased;

      // Shrink as it flies
      const scale = 1 - eased * 0.7;

      // Fade out at the end
      const opacity = progress > 0.8 ? 1 - (progress - 0.8) / 0.2 : 1;

      setStyle({ left: currentX, top: currentY, scale, opacity });

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        onComplete();
      }
    };

    requestAnimationFrame(animate);
  }, [animation, cartIconRef, onComplete]);

  return (
    <div
      className="fixed z-[9999] pointer-events-none"
      style={{
        left: style.left - 30,
        top: style.top - 30,
        transform: `scale(${style.scale})`,
        opacity: style.opacity,
        transition: "none",
      }}
    >
      <div className="w-[60px] h-[60px] rounded-lg overflow-hidden shadow-lg border-2 border-primary">
        <Image
          src={animation.image}
          alt="flying item"
          width={60}
          height={60}
          className="w-full h-full object-cover"
        />
      </div>
    </div>
  );
}