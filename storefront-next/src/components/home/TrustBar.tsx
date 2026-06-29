"use client";

import Icon from "@/components/ui/Icon";
import { useLanguage } from "@/hooks/useLanguage";

export default function TrustBar() {
  const { language } = useLanguage();
  const items =
    language === "en"
      ? [
          { icon: "bolt" as const, title: "Activate in 30 seconds", desc: "Scan QR and go online instantly" },
          { icon: "shield-alt" as const, title: "100% refund", desc: "Guaranteed if service does not work" },
          { icon: "headset" as const, title: "24/7 support", desc: "Fast live support" },
          { icon: "tags" as const, title: "Best pricing", desc: "Save up to 96% vs roaming" },
        ]
      : [
          { icon: "bolt" as const, title: "Kích hoạt 30 giây", desc: "Quét QR là dùng ngay" },
          { icon: "shield-alt" as const, title: "Hoàn tiền 100%", desc: "Cam kết nếu không dùng được" },
          { icon: "headset" as const, title: "Hỗ trợ 24/7", desc: "Tổng đài tiếng Việt" },
          { icon: "tags" as const, title: "Giá rẻ nhất", desc: "Tiết kiệm tới 96% chuyển vùng" },
        ];

  return (
    <section className="bg-white border-b border-gray-200" style={{ padding: "32px 0" }}>
      <div className="max-w-container mx-auto px-6">
        <div className="grid grid-cols-2 md:grid-cols-4 gap-8">
          {items.map((item) => (
            <div key={item.title} className="flex items-center gap-4">
              <div
                className="flex items-center justify-center flex-shrink-0 text-primary"
                style={{
                  width: "48px",
                  height: "48px",
                  borderRadius: "12px",
                  background: "rgba(0,102,255,0.1)",
                  fontSize: "22px",
                }}
              >
                <Icon icon={item.icon} />
              </div>
              <div>
                <div className="font-bold text-navy" style={{ fontSize: "15px", marginBottom: "4px" }}>
                  {item.title}
                </div>
                <div className="text-gray-500" style={{ fontSize: "13px" }}>
                  {item.desc}
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
