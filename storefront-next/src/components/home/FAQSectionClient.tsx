"use client";

import { useState } from "react";
import { useLanguage } from "@/hooks/useLanguage";
import type { FaqItem } from "@/lib/api/faqApi";

interface FAQSectionClientProps {
  items: FaqItem[];
}

export default function FAQSectionClient({ items }: FAQSectionClientProps) {
  const { language } = useLanguage();
  const [activeIndex, setActiveIndex] = useState<number | null>(0);

  const text = {
    badge: "FAQ",
    heading: language === "vi" ? "Câu hỏi thường gặp" : "Frequently Asked Questions",
    subtitle:
      language === "vi"
        ? "Giải đáp nhanh mọi thắc mắc trước khi mua eSIM du lịch."
        : "Quick answers before you buy your travel eSIM.",
    contact: language === "vi" ? "Còn câu hỏi nào khác?" : "Have more questions?",
    contactDesc:
      language === "vi"
        ? "Đội ngũ hỗ trợ của chúng tôi luôn sẵn sàng 24/7 để giúp bạn."
        : "Our support team is ready 24/7 to help you.",
    contactBtn: language === "vi" ? "Liên hệ hỗ trợ" : "Contact support",
    empty:
      language === "vi"
        ? "Chưa có câu hỏi nào. Nội dung sẽ được cập nhật sớm."
        : "No questions yet. Content will be updated soon.",
  };

  const toggle = (i: number) => {
    setActiveIndex((prev) => (prev === i ? null : i));
  };

  return (
    <section
      className="py-12 md:py-20"
      style={{ background: "linear-gradient(180deg, #fff 0%, #F0F6FF 100%)" }}
    >
      <div className="max-w-container mx-auto px-4 md:px-6">
        <div className="grid md:grid-cols-[320px_1fr] gap-8 lg:gap-16 items-start">

          {/* ── Left: sticky header ── */}
          <div className="md:sticky md:top-24 flex flex-col gap-6">
            <div>
              <span
                className="inline-block font-bold text-xs tracking-widest uppercase mb-4"
                style={{
                  background: "rgba(0,102,255,0.10)",
                  color: "#0066FF",
                  padding: "4px 14px",
                  borderRadius: "100px",
                }}
              >
                {text.badge}
              </span>
              <h2 className="text-2xl md:text-3xl font-bold text-navy leading-tight mb-3">
                {text.heading}
              </h2>
              <p className="text-gray-500 leading-relaxed text-sm md:text-base">
                {text.subtitle}
              </p>
            </div>

            {/* Contact CTA card */}
            <div
              className="rounded-2xl p-6 text-white"
              style={{
                background: "linear-gradient(135deg, #0050CC 0%, #0066FF 60%, #338FFF 100%)",
              }}
            >
              <div className="text-3xl mb-3" aria-hidden>💬</div>
              <p className="font-bold text-base mb-1">{text.contact}</p>
              <p className="text-white/75 text-sm leading-relaxed mb-5">{text.contactDesc}</p>
              <a
                href="/support"
                className="inline-flex items-center gap-1.5 font-bold text-sm no-underline transition hover:brightness-95"
                style={{
                  background: "#fff",
                  color: "#0066FF",
                  padding: "9px 20px",
                  borderRadius: "10px",
                }}
              >
                {text.contactBtn} →
              </a>
            </div>
          </div>

          {/* ── Right: accordion ── */}
          <div className="flex flex-col gap-3">
            {items.length === 0 ? (
              <div
                className="rounded-2xl border border-slate-200 bg-white px-6 py-10 text-center text-sm text-slate-500"
              >
                {text.empty}
              </div>
            ) : (
              items.map((item, i) => {
                const open = i === activeIndex;
                return (
                  <div
                    key={item.id}
                    className="bg-white rounded-2xl overflow-hidden"
                    style={{
                      border: open ? "2px solid #0066FF" : "1.5px solid #E2E8F0",
                      boxShadow: open
                        ? "0 4px 24px rgba(0,102,255,0.10)"
                        : "0 1px 4px rgba(0,0,0,0.04)",
                      transition: "border-color 0.2s, box-shadow 0.2s",
                    }}
                  >
                    <button
                      type="button"
                      onClick={() => toggle(i)}
                      className="w-full flex items-start gap-4 text-left hover:bg-slate-50/60 transition-colors"
                      style={{ padding: "18px 20px" }}
                    >
                      {/* Number badge */}
                      <span
                        className="shrink-0 font-bold text-xs leading-none flex items-center justify-center"
                        style={{
                          width: "30px",
                          height: "30px",
                          borderRadius: "8px",
                          background: open ? "#0066FF" : "#F1F5F9",
                          color: open ? "#fff" : "#94A3B8",
                          transition: "background 0.2s, color 0.2s",
                          marginTop: "1px",
                          flexShrink: 0,
                        }}
                        aria-hidden
                      >
                        {String(i + 1).padStart(2, "0")}
                      </span>

                      {/* Question */}
                      <span className="flex-1 font-semibold text-sm md:text-[15px] text-navy leading-snug">
                        {item.question}
                      </span>

                      {/* Chevron */}
                      <span
                        className="shrink-0 flex items-center justify-center font-bold text-base"
                        style={{
                          width: "28px",
                          height: "28px",
                          borderRadius: "8px",
                          background: open ? "rgba(0,102,255,0.10)" : "#F1F5F9",
                          color: open ? "#0066FF" : "#CBD5E1",
                          transform: open ? "rotate(180deg)" : "rotate(0deg)",
                          transition: "transform 0.25s, background 0.2s, color 0.2s",
                          marginTop: "1px",
                        }}
                        aria-hidden
                      >
                        ▾
                      </span>
                    </button>

                    {/* Answer */}
                    {open && (
                      <div
                        className="text-sm md:text-[15px] leading-7 text-slate-600"
                        style={{
                          padding: `0 20px 20px calc(20px + 30px + 16px)`,
                        }}
                      >
                        {item.answer}
                      </div>
                    )}
                  </div>
                );
              })
            )}
          </div>
        </div>
      </div>
    </section>
  );
}
