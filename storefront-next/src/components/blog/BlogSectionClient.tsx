"use client";

import Link from "next/link";
import { BlogCard } from "@/components/blog";
import { useLanguage } from "@/hooks/useLanguage";
import type { BlogPostSummary } from "@/types/blog";

interface BlogSectionClientProps {
  postsVi: BlogPostSummary[];
  postsEn: BlogPostSummary[];
}

export default function BlogSectionClient({ postsVi, postsEn }: BlogSectionClientProps) {
  const { language } = useLanguage();
  const posts = language === "en" ? postsEn : postsVi;
  const text = {
    eyebrow: language === "vi" ? "Tin tức tổng hợp" : "Insights",
    title: language === "vi" ? "Bài viết mới nhất" : "Latest articles",
    description:
      language === "vi"
        ? "Cập nhật kinh nghiệm, mẹo chọn gói và hướng dẫn sử dụng eSIM dành cho mọi chuyến đi."
        : "Get travel tips, package recommendations, and practical eSIM guides for every journey.",
    viewAll: language === "vi" ? "Xem tất cả bài viết" : "View all articles",
    empty: language === "vi" ? "Chưa có bài viết nào để hiển thị." : "No articles available yet.",
  };

  return (
    <section className="py-12 md:py-16 bg-gradient-to-b from-white to-[#F8FBFF]">
      <div className="max-w-container mx-auto px-4 md:px-6">
        <div className="flex flex-col md:flex-row md:items-end md:justify-between gap-3 mb-8">
          <div>
            <p className="text-sm font-semibold text-primary uppercase tracking-[0.18em] mb-2">
              {text.eyebrow}
            </p>
            <h2 className="text-2xl md:text-3xl font-bold text-navy">
              {text.title}
            </h2>
            <p className="text-gray-500 mt-2 max-w-2xl">
              {text.description}
            </p>
          </div>

          <Link
            href="/blog"
            className="text-primary font-semibold hover:underline inline-flex items-center gap-2"
          >
            {text.viewAll}
            <span aria-hidden="true">→</span>
          </Link>
        </div>

        {posts.length === 0 ? (
          <div className="rounded-2xl border border-dashed border-gray-300 bg-white px-6 py-10 text-center text-gray-500">
            {text.empty}
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-5 md:gap-6">
            {posts.map((post) => (
              <BlogCard key={post.id} post={post} language={language} />
            ))}
          </div>
        )}
      </div>
    </section>
  );
}
