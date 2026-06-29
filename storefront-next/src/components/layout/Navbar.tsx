"use client";

import Icon from "@/components/ui/Icon";
import Link from "next/link";
import { usePathname } from "next/navigation";
import type { IconName } from "@fortawesome/fontawesome-svg-core";

const menuItems: { label: string; icon: IconName; href: string }[] = [
  { label: "Trang chủ", icon: "home", href: "/" },
  { label: "Sản phẩm", icon: "list", href: "/esim-du-lich" },
  { label: "Khuyến mãi", icon: "tag", href: "/khuyen-mai" },
];

export default function Navbar() {
  const pathname = usePathname();

  return (
    <nav className="hidden lg:block bg-white border-b border-gray-200">
      <div className="max-w-container mx-auto px-6 flex gap-8">
        {menuItems.map((item) => {
          const basePath = item.href.split("?")[0];
          const isActive = basePath === "/" ? pathname === "/" : pathname.startsWith(basePath);
          return (
            <Link
              key={item.label}
              href={item.href}
              className={`py-3.5 font-medium text-sm flex items-center gap-1.5 border-b-2 transition ${
                isActive
                  ? "text-primary border-primary"
                  : "text-gray-700 border-transparent hover:text-primary"
              }`}
            >
              <Icon icon={item.icon} /> {item.label}
            </Link>
          );
        })}
      </div>
    </nav>
  );
}
