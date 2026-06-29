import Icon from "@/components/ui/Icon";
import type { IconName } from "@fortawesome/fontawesome-svg-core";

interface Stat {
  icon: IconName;
  label: string;
  value: string;
}

export interface PageHeaderProps {
  title?: string;
  subtitle?: string;
  titleIcon?: IconName;
  stats?: Stat[];
}

const DEFAULT_STATS: Stat[] = [
  { icon: "percentage", label: "Chiết khấu", value: "Lên đến 15%" },
  { icon: "bolt",       label: "Nhận mã",   value: "30 giây" },
  { icon: "shield-alt", label: "Bảo hành",  value: "100% mã thẻ" },
  { icon: "headset",    label: "Hỗ trợ",    value: "24/7" },
];

export default function PageHeader({
  title = "Mua thẻ điện thoại & thẻ game online",
  subtitle = "Nạp tiền điện thoại tất cả nhà mạng, mua thẻ game Garena, Zing, Steam, Vcoin... Chiết khấu cao - Nhận mã trong 30 giây.",
  titleIcon = "credit-card",
  stats = DEFAULT_STATS,
}: PageHeaderProps) {
  return (
    <section
      className="text-white relative overflow-hidden"
      style={{
        background: "linear-gradient(135deg, #1E1B4B 0%, #4338CA 50%, #7C3AED 100%)",
        padding: "48px 0 32px",
      }}
    >
      {/* Purple glow */}
      <div
        className="absolute pointer-events-none"
        style={{
          top: "-50%",
          right: "-10%",
          width: "500px",
          height: "500px",
          background: "radial-gradient(circle, rgba(168,85,247,0.4), transparent 70%)",
          borderRadius: "50%",
        }}
      />

      <div className="max-w-container mx-auto px-4 md:px-6 relative">
        <h1
          className="font-extrabold flex flex-col items-start gap-4 md:flex-row md:items-center"
          style={{
            fontSize: "36px",
            marginBottom: "8px",
            letterSpacing: "-1px",
          }}
        >
          <div
            className="flex items-center justify-center"
            style={{
              width: "56px",
              height: "56px",
              borderRadius: "16px",
              background: "rgba(255,255,255,0.2)",
              backdropFilter: "blur(10px)",
              fontSize: "28px",
            }}
          >
            <Icon icon={titleIcon} />
          </div>
          {title}
        </h1>
        <p className="text-white/85 max-w-[700px]" style={{ fontSize: "16px", lineHeight: 1.6 }}>
          {subtitle}
        </p>

        <div className="flex gap-4 md:gap-6 flex-wrap" style={{ marginTop: "24px" }}>
          {stats.map((s) => (
            <div
              key={s.label}
              className="flex items-center gap-3"
              style={{
                background: "rgba(255,255,255,0.15)",
                backdropFilter: "blur(10px)",
                borderRadius: "12px",
                padding: "12px 20px",
              }}
            >
              <Icon icon={s.icon} style={{ fontSize: "20px", color: "#FFE66D" }} />
              <div style={{ fontSize: "13px" }}>
                {s.label}
                <b className="block" style={{ fontSize: "16px" }}>{s.value}</b>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
