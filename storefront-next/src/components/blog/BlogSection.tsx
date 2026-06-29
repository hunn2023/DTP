import BlogSectionClient from "@/components/blog/BlogSectionClient";
import { getLatestBlogPosts } from "@/lib/api/blogApi";

export default async function BlogSection() {
  const [postsVi, postsEn] = await Promise.all([
    getLatestBlogPosts(3, "vi"),
    getLatestBlogPosts(3, "en"),
  ]);

  return <BlogSectionClient postsVi={postsVi} postsEn={postsEn} />;
}
