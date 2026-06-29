import Link from "next/link";
import BlogCard from "./BlogCard";
import type { BlogListPageData } from "@/types/blog";
import type { Language } from "@/lib/i18n";

interface BlogListProps extends BlogListPageData {
  basePath?: string;
  language?: Language;
}

export default function BlogList({ posts, currentPage, totalPages, totalPosts, basePath = "/blog", language = "vi" }: BlogListProps) {
  const createPageHref = (page: number) => (page <= 1 ? basePath : `${basePath}?page=${page}`);
  const text = {
    postCount: language === "vi" ? "bài viết" : "posts",
    page: language === "vi" ? "Trang" : "Page",
    empty: language === "vi" ? "Chưa có bài viết nào trong chuyên mục này." : "No posts in this category yet.",
    paginationLabel: language === "vi" ? "Phân trang blog" : "Blog pagination",
  };

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between gap-4">
        <p className="text-sm text-gray-500">{totalPosts} {text.postCount}</p>
        <p className="text-sm text-gray-500">{text.page} {currentPage}/{totalPages}</p>
      </div>

      {posts.length === 0 ? (
        <div className="rounded-2xl border border-dashed border-gray-300 bg-white px-6 py-10 text-center text-gray-500">
          {text.empty}
        </div>
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-5 md:gap-6">
            {posts.map((post) => (
              <BlogCard key={post.id} post={post} language={language} />
            ))}
          </div>

          {totalPages > 1 && (
            <nav className="flex items-center justify-center gap-2 flex-wrap" aria-label={text.paginationLabel}>
              <Link
                href={createPageHref(Math.max(1, currentPage - 1))}
                aria-disabled={currentPage <= 1}
                className={`flex h-10 min-w-10 items-center justify-center rounded-lg border px-3 text-sm font-medium transition ${
                  currentPage <= 1
                    ? "pointer-events-none opacity-40 border-gray-200 bg-white text-gray-400"
                    : "border-gray-200 bg-white text-gray-700 hover:border-primary hover:text-primary"
                }`}
              >
                &lt;
              </Link>

              {Array.from({ length: totalPages }, (_, index) => index + 1).map((page) => (
                <Link
                  key={page}
                  href={createPageHref(page)}
                  aria-current={page === currentPage ? "page" : undefined}
                  className={`flex h-10 min-w-10 items-center justify-center rounded-lg border px-3 text-sm font-medium transition ${
                    page === currentPage
                      ? "border-primary bg-primary text-white"
                      : "border-gray-200 bg-white text-gray-600 hover:border-primary hover:text-primary"
                  }`}
                >
                  {page}
                </Link>
              ))}

              <Link
                href={createPageHref(Math.min(totalPages, currentPage + 1))}
                aria-disabled={currentPage >= totalPages}
                className={`flex h-10 min-w-10 items-center justify-center rounded-lg border px-3 text-sm font-medium transition ${
                  currentPage >= totalPages
                    ? "pointer-events-none opacity-40 border-gray-200 bg-white text-gray-400"
                    : "border-gray-200 bg-white text-gray-700 hover:border-primary hover:text-primary"
                }`}
              >
                &gt;
              </Link>
            </nav>
          )}
        </>
      )}
    </div>
  );
}
