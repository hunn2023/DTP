"use client";

import { useMemo } from "react";
import Icon from "@/components/ui/Icon";
import { useLanguage } from "@/hooks/useLanguage";
import CountrySearchBox from "@/components/common/CountrySearchBox";

export default function HeroBanner() {
  const { language } = useLanguage();

  const bestsellers =
    language === "en"
      ? ["Japan 7 days 5GB", "Korea 10 days 10GB", "Thailand 15GB", "Europe 30 countries", "US 30 days Unl.", "Global 100+"]
      : ["Nhật 7 ngày 5GB", "Hàn 10 ngày 10GB", "Thái Lan 15GB", "Châu Âu 30 nước", "Mỹ 30 ngày Unl.", "Toàn cầu 100+"];

  const text = {
    titleTop: language === "en" ? "Stay connected" : "Kết nối",
    titleHighlight: language === "en" ? "in a flash" : "dễ như chớp mắt",
    titleBottom: language === "en" ? "for every journey" : "cho mọi chuyến đi",
    subtitle:
      language === "en"
        ? "Travel eSIM for 200+ countries with high speed, activated in 30 seconds, ready as soon as you land."
        : "eSIM du lịch 200+ quốc gia với tốc độ cao, kích hoạt trong 30 giây, dùng ngay khi hạ cánh.",
    statCountry: language === "en" ? "Countries covered" : "Quốc gia phủ sóng",
    statCustomers: language === "en" ? "Trusted customers" : "Khách hàng tin dùng",
    statRating: language === "en" ? "Average rating" : "Đánh giá trung bình",
    searchPlaceholder:
      language === "en"
        ? "Where are you going? Japan, Korea, USA..."
        : "Bạn đang đi đâu? Nhật Bản, Hàn Quốc, Mỹ...",
    searchButton: language === "en" ? "Find my plan" : "Tìm gói cho tôi",
    searchNotFound: language === "en" ? "No matching destination found." : "Không tìm thấy điểm đến phù hợp.",
    from: language === "en" ? "From" : "Từ",
    bestSellerTitle: language === "en" ? "Best sellers this week" : "Bán chạy nhất tuần này",
  };

  return (
    <section className="gradient-primary text-white relative overflow-hidden" style={{ padding: "64px 0" }}>
      {/* Decorative radial circle - mockup exact */}
      <div
        className="absolute pointer-events-none w-[280px] h-[280px] -top-[35%] -right-[15%] md:w-[600px] md:h-[600px] md:-top-[50%] md:-right-[10%]"
        style={{
          background: "radial-gradient(circle, rgba(255,255,255,0.15), transparent 70%)",
          borderRadius: "50%",
        }}
      />

      <div className="max-w-container mx-auto px-6 relative">
        <div className="grid md:grid-cols-[1.2fr_1fr] gap-12 items-center">
          {/* Left: content */}
          <div>
            <h1 className="text-white mb-5">
              {text.titleTop} <span style={{ color: "#FFE66D" }}>{text.titleHighlight}</span>
              <br />
              {text.titleBottom}
            </h1>
            <p className="text-white/90 mb-8 max-w-[520px]" style={{ fontSize: "18px" }}>
              {text.subtitle}
            </p>

            {/* Stats */}
            <div className="flex flex-wrap gap-4 sm:gap-8 mb-8">
              <div>
                <div className="text-white font-extrabold leading-none" style={{ fontSize: "32px" }}>200+</div>
                <div className="text-white/85" style={{ fontSize: "13px" }}>{text.statCountry}</div>
              </div>
              <div>
                <div className="text-white font-extrabold leading-none" style={{ fontSize: "32px" }}>100K+</div>
                <div className="text-white/85" style={{ fontSize: "13px" }}>{text.statCustomers}</div>
              </div>
              <div>
                <div className="text-white font-extrabold leading-none" style={{ fontSize: "32px" }}>4.9★</div>
                <div className="text-white/85" style={{ fontSize: "13px" }}>{text.statRating}</div>
              </div>
            </div>

            {/* Hero search */}
            <div className="relative max-w-[600px]">
              <CountrySearchBox
                language={language}
                placeholder={text.searchPlaceholder}
                notFoundText={text.searchNotFound}
                fromLabel={text.from}
                submitLabel={text.searchButton}
                variant="hero"
              />
            </div>
          </div>

          {/* Right: bestseller card - mockup exact */}
          <div
            className="hidden md:block text-white"
            style={{
              background: "rgba(255,255,255,0.1)",
              backdropFilter: "blur(10px)",
              border: "1px solid rgba(255,255,255,0.2)",
              borderRadius: "20px",
              padding: "32px",
            }}
          >
            <h3 className="font-bold mb-4 flex items-center gap-2" style={{ fontSize: "18px" }}>
              <Icon icon="fire" /> {text.bestSellerTitle}
            </h3>
            <div className="grid grid-cols-2 gap-3">
              {bestsellers.map((item) => (
                <div
                  key={item}
                  className="flex items-center gap-2.5 font-medium"
                  style={{
                    background: "rgba(255,255,255,0.1)",
                    borderRadius: "12px",
                    padding: "16px",
                    fontSize: "14px",
                  }}
                >
                  <Icon icon="bolt" style={{ color: "#FFE66D", fontSize: "20px" }} />
                  {item}
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
