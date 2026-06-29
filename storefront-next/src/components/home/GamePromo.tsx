import Link from "next/link";
import Icon from "@/components/ui/Icon";

const games = [
  { emoji: "🎮", name: "Garena", bg: "rgba(239,68,68,0.3)" },
  { emoji: "⚔️", name: "Zing",   bg: "rgba(59,130,246,0.3)" },
  { emoji: "🎯", name: "Steam",  bg: "rgba(34,197,94,0.3)" },
  { emoji: "👑", name: "Vcoin",  bg: "rgba(168,85,247,0.3)" },
  { emoji: "🏆", name: "Riot",   bg: "rgba(245,158,11,0.3)" },
  { emoji: "💎", name: "Gate",   bg: "rgba(236,72,153,0.3)" },
];

export default function GamePromo() {
  return (
    <section style={{ padding: "64px 0" }}>
      <div className="max-w-container mx-auto px-6">
        <div
          className="text-white relative overflow-hidden grid md:grid-cols-[1.2fr_1fr] gap-8 items-center"
          style={{
            background:
              "linear-gradient(135deg, #1E1B4B 0%, #312E81 50%, #4338CA 100%)",
            borderRadius: "24px",
            padding: "48px",
          }}
        >
          {/* Decorative purple glow */}
          <div
            className="absolute pointer-events-none"
            style={{
              top: 0,
              right: 0,
              width: "300px",
              height: "300px",
              background:
                "radial-gradient(circle, rgba(168,85,247,0.4), transparent 70%)",
            }}
          />

          <div className="relative">
            <h2 className="font-extrabold mb-3" style={{ fontSize: "36px", letterSpacing: "-0.5px" }}>
              Game thủ tiết kiệm 5-15%
              <br />
              khi mua thẻ tại EZSIM
            </h2>
            <p className="text-white/85 mb-6" style={{ fontSize: "16px" }}>
              Garena, Zing, Vcoin, Gate, Steam, Riot, MyCard, BIT, Wing — nhận mã trong 1 phút sau khi thanh toán.
            </p>
            <Link
              href="/the-nap?tab=game"
              className="inline-flex items-center gap-2 bg-white text-navy font-bold no-underline hover:opacity-90 transition"
              style={{
                padding: "14px 28px",
                borderRadius: "12px",
              }}
            >
              Mua thẻ game ngay <Icon icon="arrow-right" />
            </Link>
          </div>

          <div className="grid grid-cols-3 gap-3 relative">
            {games.map((game) => (
              <div
                key={game.name}
                className="text-center font-semibold"
                style={{
                  background: "rgba(255,255,255,0.1)",
                  backdropFilter: "blur(10px)",
                  border: "1px solid rgba(255,255,255,0.15)",
                  borderRadius: "12px",
                  padding: "16px",
                  fontSize: "12px",
                }}
              >
                <div
                  className="mx-auto flex items-center justify-center"
                  style={{
                    width: "48px",
                    height: "48px",
                    borderRadius: "12px",
                    margin: "0 auto 8px",
                    fontSize: "24px",
                    background: game.bg,
                  }}
                >
                  {game.emoji}
                </div>
                {game.name}
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}
