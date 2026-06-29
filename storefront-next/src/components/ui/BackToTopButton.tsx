"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { useLanguage } from "@/hooks/useLanguage";

function smoothScrollToTop(duration = 650) {
  const start = window.scrollY;
  if (start <= 0) return;

  const startTime = performance.now();
  const easeOutCubic = (t: number) => 1 - (1 - t) ** 3;

  const step = (currentTime: number) => {
    const progress = Math.min((currentTime - startTime) / duration, 1);
    const nextY = start * (1 - easeOutCubic(progress));
    window.scrollTo(0, nextY);
    if (progress < 1) {
      requestAnimationFrame(step);
    }
  };

  requestAnimationFrame(step);
}

export default function BackToTopButton() {
  const { language } = useLanguage();
  const [visible, setVisible] = useState(false);
  const isScrollingRef = useRef(false);

  useEffect(() => {
    const onScroll = () => {
      const threshold = Math.max(220, window.innerHeight * 0.45);
      setVisible(window.scrollY > threshold);
    };

    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  const scrollToTop = useCallback(() => {
    if (isScrollingRef.current) return;
    isScrollingRef.current = true;
    smoothScrollToTop();
    window.setTimeout(() => {
      isScrollingRef.current = false;
    }, 700);
  }, []);

  if (!visible) return null;

  return (
    <button
      type="button"
      onClick={scrollToTop}
      className="fixed right-[72px] bottom-5 md:right-[84px] md:bottom-8 z-[100] h-11 w-11 rounded-full gradient-primary text-white shadow-[0_10px_30px_rgba(0,102,255,0.35)] transition-all duration-300 hover:scale-105 active:scale-95"
      aria-label={language === "vi" ? "Lên đầu trang" : "Back to top"}
    >
      <span aria-hidden="true" className="text-lg font-black leading-none">↑</span>
    </button>
  );
}
