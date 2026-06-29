"use client";

import { Breadcrumb } from "@/components/ui";
import { BlogDetail } from "@/components/blog";
import { useLanguage } from "@/hooks/useLanguage";
import type { BlogPost, BlogPostSummary } from "@/types/blog";

interface BlogDetailPageClientProps {
  postVi: BlogPost;
  postEn: BlogPost;
  relatedPostsVi: BlogPostSummary[];
  relatedPostsEn: BlogPostSummary[];
}

export default function BlogDetailPageClient({
  postVi,
  postEn,
  relatedPostsVi,
  relatedPostsEn,
}: BlogDetailPageClientProps) {
  const { language } = useLanguage();
  const post = language === "en" ? postEn : postVi;
  const relatedPosts = language === "en" ? relatedPostsEn : relatedPostsVi;
  const breadcrumbLabel = language === "vi" ? "Blog tin tức" : "Blog";

  return (
    <>
      <Breadcrumb
        items={[
          { label: breadcrumbLabel, href: "/blog" },
          { label: post.title },
        ]}
      />

      <BlogDetail
        title={post.title}
        excerpt={post.excerpt}
        thumbnail={post.thumbnail}
        publishedAt={post.publishedAt}
        author={post.author}
        content={post.content}
        relatedPosts={relatedPosts}
        language={language}
      />
    </>
  );
}
