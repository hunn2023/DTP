import Image from "next/image";
import type { BlogPostSummary } from "@/types/blog";
import { formatPublishedDate } from "@/lib/blog";
import type { Language } from "@/lib/i18n";
import SafeHtmlContent from "./SafeHtmlContent";
import RelatedPosts from "./RelatedPosts";

interface BlogDetailProps {
  title: string;
  excerpt: string;
  thumbnail: string;
  publishedAt: string;
  author?: string;
  content: string;
  relatedPosts: BlogPostSummary[];
  language?: Language;
}

export default function BlogDetail({
  title,
  excerpt,
  thumbnail,
  publishedAt,
  author,
  content,
  relatedPosts,
  language = "vi",
}: BlogDetailProps) {
  return (
    <article className="max-w-container mx-auto px-4 md:px-6 py-6 md:py-10">
      <header className="max-w-3xl mx-auto mb-6 md:mb-8 text-center md:text-left">
        <p className="text-xs md:text-sm text-gray-500">
          {formatPublishedDate(publishedAt, language)}
          {author ? ` • ${author}` : ""}
        </p>
        <h1 className="text-2xl md:text-4xl font-bold text-navy mt-2 leading-tight">{title}</h1>
        <p className="text-gray-600 mt-4">{excerpt}</p>
      </header>

      <div className="max-w-4xl mx-auto">
        <div className="relative aspect-[16/9] rounded-2xl overflow-hidden bg-gray-100 mb-8 shadow-[0_20px_45px_rgba(15,23,42,0.08)]">
          <Image
            src={thumbnail}
            alt={title}
            fill
            className="object-cover"
            sizes="(max-width: 1024px) 100vw, 1200px"
          />
        </div>

        <SafeHtmlContent html={content} />

        <RelatedPosts posts={relatedPosts} language={language} />
      </div>
    </article>
  );
}
