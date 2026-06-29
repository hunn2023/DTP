import Link from "next/link";
import Icon from "@/components/ui/Icon";

export default function SuccessActions() {
  return (
    <div className="bg-white rounded-2xl shadow-card px-6 py-5 md:px-8">
      <Link
        href="/esim-du-lich"
        className="btn btn-primary w-full flex items-center justify-center gap-2 py-3 text-sm md:text-base"
      >
        <Icon icon="shopping-cart" className="text-sm" />
        Tiếp tục mua hàng
      </Link>
    </div>
  );
}
