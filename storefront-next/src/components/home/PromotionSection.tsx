import Icon from "@/components/ui/Icon";
import type { IconName } from "@fortawesome/fontawesome-svg-core";

const commitments: { icon: IconName; title: string; desc: string }[] = [
  { icon: "bolt", title: "Kích hoạt tức thì", desc: "Nhận mã ngay sau thanh toán, không cần chờ đợi" },
  { icon: "shield-alt", title: "Bảo hành 100%", desc: "Đổi trả miễn phí nếu sản phẩm lỗi" },
  { icon: "percentage", title: "Giá tốt nhất", desc: "Cam kết giá rẻ hơn mua trực tiếp tại nhà mạng" },
  { icon: "headset", title: "Hỗ trợ 24/7", desc: "Đội ngũ tư vấn sẵn sàng hỗ trợ mọi lúc" },
];

export default function PromotionSection() {
  return (
    <section className="py-12 md:py-16 bg-navy text-white">
      <div className="max-w-container mx-auto px-4 md:px-6">
        <div className="text-center mb-10">
          <span className="inline-block text-xs font-bold uppercase tracking-widest text-secondary bg-white/10 px-3 py-1 rounded-full mb-2">
            Cam kết
          </span>
          <h2 className="text-white">Của chúng tôi</h2>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
          {commitments.map((item) => (
            <div key={item.title} className="text-center">
              <div className="w-14 h-14 mx-auto mb-4 rounded-xl bg-white/10 flex items-center justify-center text-2xl text-secondary">
                <Icon icon={item.icon} />
              </div>
              <h4 className="text-white font-semibold text-sm mb-2">{item.title}</h4>
              <p className="text-slate-400 text-xs leading-relaxed">{item.desc}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
