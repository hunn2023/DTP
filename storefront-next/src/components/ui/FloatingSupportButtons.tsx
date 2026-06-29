import Icon from "@/components/ui/Icon";
import ChatbotWidget from "@/components/ui/ChatbotWidget";

const SUPPORT_LINKS = {
  zalo: "#",
  phone: "#",
};

export default function FloatingSupportButtons() {
  return (
    <div className="fixed right-4 bottom-5 md:right-6 md:bottom-8 z-[100] flex flex-col items-end gap-3 select-none">
      <div className="hidden md:block rounded-full bg-white/95 backdrop-blur border border-slate-200 px-3 py-1.5 text-xs font-semibold text-slate-700 shadow-sm animate-pulse">
        Bạn cần hỗ trợ?
      </div>

      <ChatbotWidget />

      <div className="relative">
        <span
          aria-hidden
          className="absolute inset-0 rounded-full bg-[#16A34A]/35 animate-ping [animation-duration:2.2s] [animation-delay:220ms]"
        />
        <a
          href={SUPPORT_LINKS.phone}
          aria-label="Gọi điện hỗ trợ"
          className="group relative inline-flex h-11 w-11 items-center justify-center rounded-full bg-[#16A34A] text-white shadow-[0_10px_30px_rgba(22,163,74,0.28)] transition-all duration-300 hover:-translate-y-0.5 hover:scale-110 hover:shadow-[0_14px_36px_rgba(22,163,74,0.4)] active:scale-95"
        >
          <Icon icon="phone" className="text-base transition-transform duration-300 group-hover:rotate-12" />
        </a>
      </div>

      <div className="relative">
        <span
          aria-hidden
          className="absolute inset-0 rounded-full bg-[#0068FF]/35 animate-ping [animation-duration:2s]"
        />
        <a
          href={SUPPORT_LINKS.zalo}
          aria-label="Liên hệ Zalo"
          className="group relative inline-flex h-11 w-11 items-center justify-center rounded-full bg-[#0068FF] text-white shadow-[0_10px_30px_rgba(0,104,255,0.32)] transition-all duration-300 hover:-translate-y-0.5 hover:scale-110 hover:shadow-[0_14px_36px_rgba(0,104,255,0.42)] active:scale-95"
        >
          <span className="text-[11px] font-extrabold tracking-[0.2px]">Zalo</span>
        </a>
      </div>
    </div>
  );
}
