"use client";

import { useLanguage } from "@/hooks/useLanguage";

export default function HowItWorks() {
  const { language } = useLanguage();
  const steps =
    language === "en"
      ? [
          {
            num: 1,
            title: "Choose your plan",
            desc: "Pick destination and data package. Filter by budget and trip duration.",
          },
          {
            num: 2,
            title: "Fast checkout",
            desc: "Use your preferred payment method and complete checkout in seconds.",
          },
          {
            num: 3,
            title: "Receive and use instantly",
            desc: "QR code arrives by email/SMS. Scan and connect right away.",
          },
        ]
      : [
          {
            num: 1,
            title: "Chọn sản phẩm",
            desc: "Chọn quốc gia + gói data hoặc loại thẻ. Lọc theo ngân sách & số ngày.",
          },
          {
            num: 2,
            title: "Thanh toán nhanh",
            desc: "Momo, ZaloPay, VNPay, thẻ ngân hàng, QR Banking — chọn cách bạn thích.",
          },
          {
            num: 3,
            title: "Nhận & dùng ngay",
            desc: "Mã QR / mã thẻ gửi qua email + SMS. Quét là chạy, không cần đợi.",
          },
        ];

  const heading = language === "en" ? "Buy and activate in just 3 steps" : "Mua và sử dụng cực dễ — chỉ 3 bước";
  const subtitle = language === "en" ? "Total time: under 60 seconds after payment" : "Tổng thời gian: dưới 60 giây kể từ lúc thanh toán";

  return (
    <section className="bg-gray-50" style={{ padding: "64px 0" }}>
      <div className="max-w-container mx-auto px-6">
        <div className="flex flex-col items-center text-center mb-12">
          <h2 className="section-title">{heading}</h2>
          <p className="section-subtitle">{subtitle}</p>
        </div>

        <div className="relative grid grid-cols-1 md:grid-cols-3 gap-8">
          {/* Connecting line - mockup style */}
          <div
            className="absolute hidden md:block pointer-events-none"
            style={{
              top: "40px",
              left: "16%",
              right: "16%",
              height: "2px",
              background:
                "linear-gradient(to right, transparent, #0066FF, transparent)",
              opacity: 0.3,
            }}
          />

          {steps.map((step) => (
            <div
              key={step.num}
              className="bg-white text-center relative"
              style={{
                borderRadius: "20px",
                padding: "32px",
                border: "1px solid #E2E8F0",
              }}
            >
              <div
                className="mx-auto text-white font-extrabold flex items-center justify-center gradient-primary"
                style={{
                  width: "80px",
                  height: "80px",
                  borderRadius: "50%",
                  fontSize: "32px",
                  margin: "-64px auto 20px",
                  border: "6px solid #F8FAFC",
                }}
              >
                {step.num}
              </div>
              <h3 className="font-bold mb-2" style={{ fontSize: "18px" }}>
                {step.title}
              </h3>
              <p className="text-gray-500" style={{ fontSize: "14px", lineHeight: 1.6 }}>
                {step.desc}
              </p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
