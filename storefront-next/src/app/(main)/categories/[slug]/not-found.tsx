import Link from "next/link";
import Icon from "@/components/ui/Icon";

export default function NotFound() {
  return (
    <section className="max-w-container mx-auto px-4 md:px-6 py-16 text-center">
      <Icon icon="folder-open" className="text-6xl text-gray-300 mb-6" />
      <h1 className="text-2xl font-bold text-navy mb-3">Không tìm thấy danh mục</h1>
      <p className="text-gray-500 mb-8">
        Danh mục bạn đang tìm không tồn tại hoặc đã bị xóa.
      </p>
      <Link href="/esim-du-lich" className="btn-primary">
        Xem tất cả sản phẩm
      </Link>
    </section>
  );
}
