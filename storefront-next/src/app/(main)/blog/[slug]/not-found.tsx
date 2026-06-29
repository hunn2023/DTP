import Link from "next/link";

export default function BlogDetailNotFound() {
  return (
    <section className="max-w-container mx-auto px-4 md:px-6 py-16 text-center">
      <h1 className="text-2xl font-bold text-navy mb-3">Không tìm thấy bài viết</h1>
      <p className="text-gray-500 mb-8">Bài viết không tồn tại hoặc đã được gỡ bỏ.</p>
      <Link href="/blog" className="btn-primary">
        Quay lại blog
      </Link>
    </section>
  );
}
