"use client";

import { useLanguage } from "@/hooks/useLanguage";

export default function Testimonials() {
  const { language } = useLanguage();
  const testimonials =
    language === "en"
      ? [
          {
            initials: "MA",
            name: "Minh Anh",
            role: "Korea Trip 2025",
            text: "My first eSIM for Korea. I ordered late at night and scanned the QR at the airport next morning. Super fast and much cheaper than roaming.",
          },
          {
            initials: "TH",
            name: "Tran Hung",
            role: "Mobile Gamer",
            text: "I regularly buy top-ups and data here. Easy interface, fast payments, and quick delivery. Support team is responsive and helpful.",
          },
          {
            initials: "LP",
            name: "Le Phuong",
            role: "Startup CEO",
            text: "On a 21-day Europe business trip, my eSIM worked smoothly across multiple countries without interruptions. Huge savings versus roaming.",
          },
        ]
      : [
          {
            initials: "MA",
            name: "Minh Anh",
            role: "Du lịch Hàn Quốc 2025",
            text: "Lần đầu mua eSIM đi Hàn, đặt 11h tối hôm trước, sáng ra sân bay quét QR là dùng được ngay. Quá nhanh và rẻ hơn roaming Viettel rất nhiều!",
          },
          {
            initials: "TH",
            name: "Trần Hùng",
            role: "Game thủ Liên Quân",
            text: "Tôi mua thẻ Garena và data 4G ở đây thường xuyên. Giao diện dễ dùng, thanh toán Momo nhanh, nhận mã 30 giây. Hỗ trợ chat siêu nhanh.",
          },
          {
            initials: "LP",
            name: "Lê Phương",
            role: "CEO Startup",
            text: "Đi công tác Châu Âu 21 ngày, mua eSIM Schengen 30 nước, dùng được ở Pháp - Ý - Đức không gián đoạn. Tiết kiệm gần 4 triệu so với roaming.",
          },
        ];

  const heading = language === "en" ? "Trusted by 100,000+ customers" : "100.000+ khách hàng tin dùng";
  const subtitle = language === "en" ? "Real feedback from EZSIM users" : "Đánh giá thật từ người Việt đã sử dụng EZSIM";

  return (
    <section className="bg-gray-50" style={{ padding: "64px 0" }}>
      <div className="max-w-container mx-auto px-6">
        <div className="flex flex-col items-center text-center mb-8">
          <h2 className="section-title">{heading}</h2>
          <p className="section-subtitle">{subtitle}</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {testimonials.map((t) => (
            <div
              key={t.name}
              className="bg-white"
              style={{
                border: "1px solid #E2E8F0",
                borderRadius: "20px",
                padding: "28px",
              }}
            >
              <div className="mb-3" style={{ color: "#F59E0B", fontSize: "16px" }}>
                ★★★★★
              </div>
              <p
                className="text-gray-700 italic mb-5"
                style={{ fontSize: "14px", lineHeight: 1.7 }}
              >
                &ldquo;{t.text}&rdquo;
              </p>
              <div className="flex items-center gap-3">
                <div
                  className="text-white font-bold flex items-center justify-center gradient-primary"
                  style={{
                    width: "44px",
                    height: "44px",
                    borderRadius: "50%",
                  }}
                >
                  {t.initials}
                </div>
                <div>
                  <div className="font-bold" style={{ fontSize: "14px" }}>
                    {t.name}
                  </div>
                  <div className="text-gray-500" style={{ fontSize: "12px" }}>
                    {t.role}
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
