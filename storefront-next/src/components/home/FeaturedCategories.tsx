import Link from "next/link";
import Icon from "@/components/ui/Icon";

const categories = [
  {
    name: "eSIM Du lịch",
    icon: "globe-asia" as const,
    href: "/esim-du-lich",
    desc: "Kết nối ngay khi đặt chân tới 200+ quốc gia. Không cần tháo SIM gốc.",
    tags: ["200+ quốc gia", "Quét QR", "4G/5G"],
    cta: "Xem điểm đến",
    gradient: "linear-gradient(135deg, #0066FF 0%, #00D4FF 100%)",
  },
  {
    name: "Thẻ Viễn thông",
    icon: "sim-card" as const,
    href: "/the-nap?tab=telecom",
    desc: "Nạp tiền điện thoại Viettel, Vinaphone, Mobifone, Vietnamobile chiết khấu cao.",
    tags: ["Viettel", "Vina", "Mobi"],
    cta: "Mua thẻ ngay",
    gradient: "linear-gradient(135deg, #F59E0B, #EF4444)",
  },
  {
    name: "Thẻ Game",
    icon: "gamepad" as const,
    href: "/the-nap?tab=game",
    desc: "Garena, Zing, Vcoin, Gate, Steam, Riot, MyCard. Nhận mã trong 1 phút.",
    tags: ["Garena", "Zing", "Steam"],
    cta: "Mua thẻ ngay",
    gradient: "linear-gradient(135deg, #8B5CF6, #EC4899)",
  },
  {
    name: "Data 4G/5G",
    icon: "wifi" as const,
    href: "/the-nap?tab=data",
    desc: "Đăng ký gói data tốc độ cao của các nhà mạng VN. Có hiệu lực ngay.",
    tags: ["Tháng", "Tuần", "Ngày"],
    cta: "Đăng ký gói",
    gradient: "linear-gradient(135deg, #10B981, #06B6D4)",
  },
];

export default function FeaturedCategories() {
  return (
    <section style={{ padding: "64px 0" }}>
      <div className="max-w-container mx-auto px-6">
        <div className="flex justify-between items-end mb-8">
          <div>
            <h2 className="section-title">Bạn đang cần gì?</h2>
            <p className="section-subtitle">4 dòng sản phẩm - 1 nơi mua sắm tiện lợi</p>
          </div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
          {categories.map((cat) => (
            <Link
              key={cat.name}
              href={cat.href}
              className="bg-white relative overflow-hidden text-navy no-underline transition-all duration-300 group hover:-translate-y-1"
              style={{
                border: "1.5px solid #E2E8F0",
                borderRadius: "20px",
                padding: "28px",
              }}
            >
              {/* Top accent bar on hover */}
              <div
                className="absolute top-0 left-0 w-full h-1 origin-left scale-x-0 group-hover:scale-x-100 transition-transform duration-300 gradient-primary"
              />

              {/* Icon */}
              <div
                className="flex items-center justify-center text-white"
                style={{
                  width: "56px",
                  height: "56px",
                  borderRadius: "16px",
                  background: cat.gradient,
                  fontSize: "26px",
                  marginBottom: "20px",
                }}
              >
                <Icon icon={cat.icon} />
              </div>

              <h3 className="font-extrabold mb-2" style={{ fontSize: "20px" }}>
                {cat.name}
              </h3>
              <p className="text-gray-500 mb-4" style={{ fontSize: "14px", lineHeight: 1.6 }}>
                {cat.desc}
              </p>

              <div className="flex flex-wrap gap-1.5 mb-4">
                {cat.tags.map((tag) => (
                  <span
                    key={tag}
                    className="bg-gray-100 text-gray-700 font-medium"
                    style={{
                      padding: "4px 10px",
                      borderRadius: "6px",
                      fontSize: "12px",
                    }}
                  >
                    {tag}
                  </span>
                ))}
              </div>

              <div className="text-primary font-semibold flex items-center gap-1" style={{ fontSize: "14px" }}>
                {cat.cta} <Icon icon="arrow-right" />
              </div>
            </Link>
          ))}
        </div>
      </div>
    </section>
  );
}
