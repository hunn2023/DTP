import Icon from "@/components/ui/Icon";
import Link from "next/link";

interface BreadcrumbItem {
  label: string;
  href?: string;
}

export default function Breadcrumb({ items }: { items: BreadcrumbItem[] }) {
  return (
    <div className="max-w-container mx-auto px-4 md:px-6 pt-4 pb-1">
      <ol
        className="flex items-center gap-1.5 text-xs text-gray-400 flex-wrap"
        aria-label="Breadcrumb"
      >
        <li>
          <Link href="/" className="hover:text-primary transition" aria-label="Trang chủ">
            <Icon icon="home" className="text-[11px]" />
          </Link>
        </li>
        {items.map((item, i) => (
          <li key={i} className="flex items-center gap-1.5">
            <Icon icon="chevron-right" className="text-[9px] text-gray-300" />
            {item.href ? (
              <Link href={item.href} className="hover:text-primary transition">
                {item.label}
              </Link>
            ) : (
              <span className="text-navy font-semibold" aria-current="page">
                {item.label}
              </span>
            )}
          </li>
        ))}
      </ol>
    </div>
  );
}
