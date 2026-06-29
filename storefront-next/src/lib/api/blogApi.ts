import type { Language } from "@/lib/i18n";
import type { BlogPost, BlogPostSummary } from "@/types/blog";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

export const BLOG_PAGE_SIZE = 6;

export interface BlogListResult {
  posts: BlogPostSummary[];
  currentPage: number;
  totalPages: number;
  totalPosts: number;
}

// ─── Backend response shapes ────────────────────────────────────────────────────

/** Public content article as returned by /api/content/articles* (PublicContentArticles). */
interface ApiContentArticle {
  id: string;
  title: string | null;
  slug: string | null;
  summary: string | null;
  content: string | null;
  thumbnailUrl: string | null;
  authorName: string | null;
  categoryCode: string | null;
  tags: string | null;
  status?: number;
  isFeatured?: boolean;
  sortOrder?: number;
  publishedAt?: string | null;
  createdAt?: string | null;
  updatedAt?: string | null;
}

interface ApiResult<T> {
  isSuccess: boolean;
  data: T | null;
  error: unknown;
}

interface ApiPagedData<T> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// ─── Mapping ────────────────────────────────────────────────────────────────────

function fallbackThumbnail(slug: string): string {
  return `https://picsum.photos/seed/${encodeURIComponent(slug || "blog")}/1200/630`;
}

function mapArticleToPost(article: ApiContentArticle): BlogPost {
  const slug = article.slug ?? "";
  return {
    id: article.id,
    title: article.title ?? "",
    slug,
    excerpt: article.summary ?? "",
    content: article.content ?? "",
    thumbnail: article.thumbnailUrl || fallbackThumbnail(slug),
    publishedAt: article.publishedAt || article.createdAt || article.updatedAt || "",
    author: article.authorName ?? undefined,
  };
}

function toSummary({ content: _content, ...summary }: BlogPost): BlogPostSummary {
  return summary;
}

// ─── HTTP layer ─────────────────────────────────────────────────────────────────

async function fetchJson<T>(path: string): Promise<T | null> {
  try {
    const response = await fetchWithAuth(path);
    if (!response.ok) return null;
    return (await response.json()) as T;
  } catch {
    return null;
  }
}

/** Fetch a page of published articles, already mapped to BlogPost. */
async function fetchArticlesPage(
  pageIndex: number,
  pageSize: number
): Promise<{ posts: BlogPost[]; totalCount: number; totalPages: number; pageIndex: number }> {
  const params = new URLSearchParams({
    pageIndex: String(pageIndex),
    pageSize: String(pageSize),
  });
  const json = await fetchJson<ApiResult<ApiPagedData<ApiContentArticle>>>(
    `/api/content/articles?${params.toString()}`
  );

  const data = json?.data;
  if (!data || !Array.isArray(data.items)) {
    return { posts: [], totalCount: 0, totalPages: 0, pageIndex };
  }

  return {
    posts: data.items.map(mapArticleToPost),
    totalCount: data.totalCount ?? data.items.length,
    totalPages: data.totalPages ?? 0,
    pageIndex: data.pageIndex ?? pageIndex,
  };
}

// ─── Public API (signatures unchanged) ────────────────────────────────────────────

export async function getBlogPosts(_language: Language = "vi"): Promise<BlogPostSummary[]> {
  // Pull a generous page so callers that need the full list (slugs, related) are covered.
  const { posts } = await fetchArticlesPage(1, 100);
  return posts.map(toSummary);
}

export async function getLatestBlogPosts(limit = 3, _language: Language = "vi"): Promise<BlogPostSummary[]> {
  const { posts } = await fetchArticlesPage(1, limit);
  return posts.slice(0, limit).map(toSummary);
}

export async function getBlogPostsPage(
  page = 1,
  pageSize = BLOG_PAGE_SIZE,
  _language: Language = "vi"
): Promise<BlogListResult> {
  const requestedPage = Math.max(1, page);
  const { posts, totalCount, totalPages: apiTotalPages, pageIndex } = await fetchArticlesPage(
    requestedPage,
    pageSize
  );
  const totalPages = Math.max(1, apiTotalPages || Math.ceil(totalCount / pageSize) || 1);

  return {
    posts: posts.map(toSummary),
    currentPage: Math.min(Math.max(1, pageIndex || requestedPage), totalPages),
    totalPages,
    totalPosts: totalCount,
  };
}

export async function getBlogPostBySlug(slug: string, _language: Language = "vi"): Promise<BlogPost | null> {
  const json = await fetchJson<ApiResult<ApiContentArticle>>(
    `/api/content/articles/${encodeURIComponent(slug)}`
  );
  const article = json?.data;
  if (!article || json?.isSuccess === false) return null;
  return mapArticleToPost(article);
}

export async function getRelatedBlogPosts(slug: string, limit = 3, _language: Language = "vi"): Promise<BlogPostSummary[]> {
  // Backend has no "related" endpoint; mirror previous behaviour: latest posts excluding the current one.
  const { posts } = await fetchArticlesPage(1, limit + 1);
  return posts
    .filter((post) => post.slug !== slug)
    .slice(0, limit)
    .map(toSummary);
}

export async function getBlogSlugs(): Promise<string[]> {
  const { posts } = await fetchArticlesPage(1, 100);
  return posts.map((post) => post.slug).filter(Boolean);
}
