import type { Metadata } from "next";
import { notFound } from "next/navigation";
import BlogDetailPageClient from "@/app/(main)/blog/[slug]/BlogDetailPageClient";
import { getBlogPostBySlug, getBlogSlugs, getRelatedBlogPosts } from "@/lib/api/blogApi";
import { buildMetadata } from "@/lib/seo";

export const revalidate = 60;

export async function generateStaticParams() {
  const slugs = await getBlogSlugs();
  return slugs.map((slug) => ({ slug }));
}

interface BlogDetailPageProps {
  params: { slug: string };
}

export async function generateMetadata({
  params,
}: BlogDetailPageProps): Promise<Metadata> {
  const post = await getBlogPostBySlug(params.slug, "vi");

  if (!post) {
    return buildMetadata({
      title: "Bài viết không tồn tại",
      description: "Bài viết bạn tìm không tồn tại hoặc đã được gỡ bỏ.",
      canonicalPath: `/blog/${params.slug}`,
      noIndex: true,
    });
  }

  return buildMetadata({
    title: post.metaTitle ?? post.title,
    description: post.metaDescription ?? post.excerpt,
    image: post.thumbnail,
    canonicalPath: `/blog/${post.slug}`,
    type: "article",
  });
}

export default async function BlogDetailPage({ params }: BlogDetailPageProps) {
  const postVi = await getBlogPostBySlug(params.slug, "vi");

  if (!postVi) {
    notFound();
  }

  const [postEn, relatedPostsVi, relatedPostsEn] = await Promise.all([
    getBlogPostBySlug(params.slug, "en"),
    getRelatedBlogPosts(postVi.slug, 3, "vi"),
    getRelatedBlogPosts(postVi.slug, 3, "en"),
  ]);

  return (
    <BlogDetailPageClient
      postVi={postVi}
      postEn={postEn ?? postVi}
      relatedPostsVi={relatedPostsVi}
      relatedPostsEn={relatedPostsEn}
    />
  );
}
