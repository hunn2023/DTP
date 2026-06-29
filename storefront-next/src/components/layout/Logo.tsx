import Icon from "@/components/ui/Icon";
import Link from "next/link";

interface LogoProps {
  className?: string;
}

export default function Logo({ className = "" }: LogoProps) {
  return (
    <Link
      href="/"
      className={`flex items-center gap-3 text-[28px] font-extrabold tracking-tight no-underline text-navy ${className}`}
    >
      <div className="w-10 h-10 rounded-full gradient-primary flex items-center justify-center text-white text-[22px]">
        <Icon icon="bolt" />
      </div>
      ez<span className="gradient-text">sim</span>
    </Link>
  );
}
