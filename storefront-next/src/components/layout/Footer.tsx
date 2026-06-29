"use client";

import Icon from "@/components/ui/Icon";
import Link from "next/link";
import { useLanguage } from "@/hooks/useLanguage";
import Logo from "./Logo";

const footerLinks = {
  products: [
    { label: "eSIM Du lịch", href: "/esim-du-lich" },
    { label: "Thẻ Viễn thông", href: "/the-nap?tab=telecom" },
    { label: "Thẻ Game", href: "/the-nap?tab=game" },
    { label: "Data 4G/5G", href: "/the-nap?tab=data" },
    { label: "Khuyến mãi", href: "/the-nap?tab=promo" },
  ],
  support: [
    { label: "Cài đặt eSIM", href: "/huong-dan/cai-dat-esim" },
    { label: "Kiểm tra thiết bị", href: "/kiem-tra-thiet-bi" },
    { label: "Câu hỏi thường gặp", href: "/faq" },
    { label: "Liên hệ", href: "/lien-he" },
    { label: "Blog", href: "/blog" },
  ],
  policies: [
    { label: "Chính sách bảo mật", href: "/chinh-sach-bao-mat" },
    { label: "Điều khoản sử dụng", href: "/dieu-khoan" },
    { label: "Chính sách đổi trả", href: "/chinh-sach-doi-tra" },
    { label: "Chính sách thanh toán", href: "/chinh-sach-thanh-toan" },
    { label: "Giới thiệu", href: "/gioi-thieu" },
  ],
};

export default function Footer() {
  const { language } = useLanguage();
  const text = {
    brandDescription:
      language === "vi"
        ? "Kết nối dễ như chớp mắt — Connect in a Flash. Nền tảng cung cấp eSIM du lịch, thẻ viễn thông, thẻ game và gói Data hàng đầu Việt Nam."
        : "Connect in a Flash. A leading platform for travel eSIM, telecom cards, game cards, and data packages in Vietnam.",
    products: language === "vi" ? "Sản phẩm" : "Products",
    support: language === "vi" ? "Hỗ trợ" : "Support",
    policies: language === "vi" ? "Chính sách" : "Policies",
    contact: language === "vi" ? "Liên hệ" : "Contact",
    location: language === "vi" ? "Hà Nội, Việt Nam" : "Hanoi, Vietnam",
    availability: language === "vi" ? "24/7 - 365 ngày" : "24/7 - 365 days",
    privacy: language === "vi" ? "Chính sách bảo mật" : "Privacy policy",
    terms: language === "vi" ? "Điều khoản" : "Terms",
    ministry: language === "vi" ? "Đã thông báo Bộ Công Thương" : "Registered with Ministry of Industry and Trade",
    copyright: language === "vi" ? "© 2026 EZSIM Vietnam. All rights reserved." : "© 2026 EZSIM Vietnam. All rights reserved.",
  };

  const productsLinks =
    language === "vi"
      ? footerLinks.products
      : [
          { label: "Travel eSIM", href: "/esim-du-lich" },
          { label: "Telecom cards", href: "/the-nap?tab=telecom" },
          { label: "Game cards", href: "/the-nap?tab=game" },
          { label: "4G/5G Data", href: "/the-nap?tab=data" },
          { label: "Promotions", href: "/the-nap?tab=promo" },
        ];

  const supportLinks =
    language === "vi"
      ? footerLinks.support
      : [
          { label: "Install eSIM", href: "/huong-dan/cai-dat-esim" },
          { label: "Check device", href: "/kiem-tra-thiet-bi" },
          { label: "FAQ", href: "/faq" },
          { label: "Contact", href: "/lien-he" },
          { label: "Blog", href: "/blog" },
        ];

  const policyLinks =
    language === "vi"
      ? footerLinks.policies
      : [
          { label: "Privacy policy", href: "/chinh-sach-bao-mat" },
          { label: "Terms of use", href: "/dieu-khoan" },
          { label: "Refund policy", href: "/chinh-sach-doi-tra" },
          { label: "Payment policy", href: "/chinh-sach-thanh-toan" },
          { label: "About us", href: "/gioi-thieu" },
        ];

  return (
    <footer className="bg-navy text-gray-400 pt-16 pb-6">
      <div className="max-w-container mx-auto px-6">
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-8 lg:gap-12 mb-12">
          {/* Brand */}
          <div className="sm:col-span-2 lg:col-span-1">
            <div className="mb-4"><Logo className="text-white" /></div>
            <p className="text-sm leading-relaxed max-w-[320px]">
              {text.brandDescription}
            </p>
            <div className="mt-5 flex gap-3">
              {(["facebook-f", "tiktok", "youtube", "instagram"] as const).map((icon) => (
                <Link
                  key={icon}
                  href="#"
                  className="w-9 h-9 rounded-full bg-white/10 text-white flex items-center justify-center hover:bg-cyan transition"
                >
                  <Icon icon={["fab", icon]} />
                </Link>
              ))}
            </div>
          </div>

          {/* Sản phẩm */}
          <div>
            <h4 className="text-white font-bold text-[15px] mb-4">{text.products}</h4>
            <ul className="space-y-2.5">
              {productsLinks.map((l) => (
                <li key={l.label}>
                  <Link href={l.href} className="text-[13px] hover:text-cyan transition">{l.label}</Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Hỗ trợ */}
          <div>
            <h4 className="text-white font-bold text-[15px] mb-4">{text.support}</h4>
            <ul className="space-y-2.5">
              {supportLinks.map((l) => (
                <li key={l.label}>
                  <Link href={l.href} className="text-[13px] hover:text-cyan transition">{l.label}</Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Chính sách */}
          <div>
            <h4 className="text-white font-bold text-[15px] mb-4">{text.policies}</h4>
            <ul className="space-y-2.5">
              {policyLinks.map((l) => (
                <li key={l.label}>
                  <Link href={l.href} className="text-[13px] hover:text-cyan transition">{l.label}</Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Liên hệ */}
          <div>
            <h4 className="text-white font-bold text-[15px] mb-4">{text.contact}</h4>
            <ul className="space-y-2.5 text-[13px]">
              <li className="flex items-center gap-2"><Icon icon="phone" />1900 1234</li>
              <li className="flex items-center gap-2"><Icon icon="envelope" />support@ezsim.vn</li>
              <li className="flex items-center gap-2"><Icon icon="map-marker-alt" />{text.location}</li>
              <li className="flex items-center gap-2"><Icon icon="clock" />{text.availability}</li>
            </ul>
          </div>
        </div>

        <div className="border-t border-white/10 pt-6 flex flex-col md:flex-row justify-between gap-3 text-[13px]">
          <div>{text.copyright}</div>
          <div className="flex flex-wrap gap-2">
            <Link href="/chinh-sach-bao-mat" className="hover:text-cyan transition">{text.privacy}</Link>
            <span>•</span>
            <Link href="/dieu-khoan" className="hover:text-cyan transition">{text.terms}</Link>
            <span>•</span>
            <Link href="#" className="hover:text-cyan transition">{text.ministry}</Link>
          </div>
        </div>
      </div>
    </footer>
  );
}
