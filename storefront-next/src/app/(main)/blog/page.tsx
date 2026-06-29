import type { Metadata } from "next";
import BlogPageClient from "@/app/(main)/blog/BlogPageClient";
import { getBlogPosts } from "@/lib/api/blogApi";
import { buildMetadata } from "@/lib/seo";

export const revalidate = 60;

export const metadata: Metadata = buildMetadata({
  title: "Blog tin tức eSIM",
  description: "Tin tức, hướng dẫn và mẹo sử dụng eSIM khi đi du lịch quốc tế.",
  canonicalPath: "/blog",
});

export default async function BlogPage() {
  try {
    const [postsVi, postsEn] = await Promise.all([
      getBlogPosts("vi"),
      getBlogPosts("en"),
    ]);

    return <BlogPageClient postsVi={postsVi} postsEn={postsEn} />;
  } catch {
    return <BlogPageClient postsVi={null} postsEn={null} />;
  }
}
