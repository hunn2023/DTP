import Image from "next/image";
import Link from "next/link";
import type { BlogPostSummary } from "@/types/blog";
import { formatPublishedDate } from "@/lib/blog";
import type { Language } from "@/lib/i18n";

interface BlogCardProps {
  post: BlogPostSummary;
  language?: Language;
}

export default function BlogCard({ post, language = "vi" }: BlogCardProps) {
  const text = {
    viewDetail: language === "vi" ? "Xem chi tiết" : "Read more",
  };

  return (
    <article className="group bg-white rounded-2xl border border-gray-200 overflow-hidden transition-all duration-300 hover:-translate-y-1 hover:shadow-[0_18px_40px_rgba(15,23,42,0.12)]">
      <Link
        href={`/blog/${post.slug}`}
        className="block relative aspect-[16/9] bg-gray-100 overflow-hidden"
      >
        <Image
          src={post.thumbnail}
          alt={post.title}
          fill
          className="object-cover transition-transform duration-500 group-hover:scale-105"
          sizes="(max-width: 768px) 100vw, (max-width: 1280px) 50vw, 33vw"
        />
      </Link>

      <div className="p-5 md:p-6">
        <p className="text-xs text-gray-500 mb-2">
          {formatPublishedDate(post.publishedAt, language)}
        </p>
        <h2 className="text-lg md:text-xl font-bold text-navy line-clamp-2 group-hover:text-primary transition-colors">
          <Link
            href={`/blog/${post.slug}`}
            className="hover:text-primary transition-colors"
          >
            {post.title}
          </Link>
        </h2>
        <p className="text-sm text-gray-600 mt-3 line-clamp-3">
          {post.excerpt}
        </p>

        <Link
          href={`/blog/${post.slug}`}
          className="btn-outline btn-sm mt-5 inline-flex"
        >
          {text.viewDetail}
        </Link>
      </div>
    </article>
  );
}
