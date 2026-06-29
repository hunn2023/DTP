import Link from "next/link";
import BlogCard from "./BlogCard";
import type { BlogPostSummary } from "@/types/blog";
import type { Language } from "@/lib/i18n";

interface RelatedPostsProps {
  posts: BlogPostSummary[];
  language?: Language;
}

export default function RelatedPosts({ posts, language = "vi" }: RelatedPostsProps) {
  if (posts.length === 0) {
    return null;
  }

  const text = {
    eyebrow: language === "vi" ? "Bài viết liên quan" : "Related articles",
    title: language === "vi" ? "Đọc thêm" : "Read more",
    viewAll: language === "vi" ? "Xem tất cả" : "View all",
  };

  return (
    <section className="mt-14">
      <div className="flex items-end justify-between gap-4 mb-5">
        <div>
          <p className="text-sm font-semibold text-primary uppercase tracking-[0.18em] mb-2">
            {text.eyebrow}
          </p>
          <h2 className="text-2xl font-bold text-navy">{text.title}</h2>
        </div>
        <Link
          href="/blog"
          className="text-primary font-semibold hover:underline hidden md:inline-flex"
        >
          {text.viewAll}
        </Link>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-5 md:gap-6">
        {posts.map((post) => (
          <BlogCard key={post.id} post={post} language={language} />
        ))}
      </div>
    </section>
  );
}
