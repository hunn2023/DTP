"use client";

import Icon from "@/components/ui/Icon";
import { Breadcrumb } from "@/components/ui";
import { useLanguage } from "@/hooks/useLanguage";

export default function SupportContent() {
  const { language } = useLanguage();
  const text = {
    breadcrumb: language === "vi" ? "Hỗ trợ" : "Support",
    title: language === "vi" ? "Hỗ trợ khách hàng" : "Customer support",
    description:
      language === "vi"
        ? "Đội ngũ EZSIM luôn sẵn sàng hỗ trợ bạn trong suốt quá trình mua và kích hoạt eSIM."
        : "The EZSIM team is always ready to support you during eSIM purchase and activation.",
    emailTitle: language === "vi" ? "Email hỗ trợ" : "Support email",
    emailDesc:
      language === "vi"
        ? "Phản hồi trong giờ làm việc."
        : "Replies during business hours.",
    hotlineTitle: "Hotline",
    hotlineDesc:
      language === "vi"
        ? "Hỗ trợ nhanh các vấn đề khẩn cấp."
        : "Fast support for urgent issues.",
  };

  return (
    <>
      <Breadcrumb items={[{ label: text.breadcrumb }]} />

      <section className="max-w-container mx-auto px-6" style={{ paddingTop: "40px", paddingBottom: "56px" }}>
        <div
          className="gradient-primary text-white relative overflow-hidden"
          style={{ borderRadius: "20px", padding: "32px" }}
        >
          <h1 className="text-white mb-2">{text.title}</h1>
          <p className="text-white/90" style={{ fontSize: "15px" }}>
            {text.description}
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-5" style={{ marginTop: "24px" }}>
          <a
            href="mailto:support@ezsim.vn"
            className="bg-white no-underline text-navy transition hover:-translate-y-0.5"
            style={{ border: "1.5px solid #E2E8F0", borderRadius: "16px", padding: "24px" }}
          >
            <div className="flex items-center gap-3 mb-2">
              <div
                className="flex items-center justify-center text-primary"
                style={{ width: "44px", height: "44px", borderRadius: "12px", background: "#EFF6FF" }}
              >
                <Icon icon="envelope" className="text-lg" />
              </div>
              <h2 className="text-xl font-bold text-navy">{text.emailTitle}</h2>
            </div>
            <p className="text-gray-500 text-sm">{text.emailDesc}</p>
            <p className="text-primary font-semibold" style={{ marginTop: "10px" }}>support@ezsim.vn</p>
          </a>

          <a
            href="tel:19001234"
            className="bg-white no-underline text-navy transition hover:-translate-y-0.5"
            style={{ border: "1.5px solid #E2E8F0", borderRadius: "16px", padding: "24px" }}
          >
            <div className="flex items-center gap-3 mb-2">
              <div
                className="flex items-center justify-center text-primary"
                style={{ width: "44px", height: "44px", borderRadius: "12px", background: "#EFF6FF" }}
              >
                <Icon icon="headset" className="text-lg" />
              </div>
              <h2 className="text-xl font-bold text-navy">{text.hotlineTitle}</h2>
            </div>
            <p className="text-gray-500 text-sm">{text.hotlineDesc}</p>
            <p className="text-primary font-semibold" style={{ marginTop: "10px" }}>1900 1234</p>
          </a>
        </div>
      </section>
    </>
  );
}
