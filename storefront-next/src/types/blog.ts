export interface BlogPost {
  id: string;
  title: string;
  slug: string;
  excerpt: string;
  content: string;
  thumbnail: string;
  metaTitle?: string;
  metaDescription?: string;
  publishedAt: string;
  author?: string;
}

export type BlogPostSummary = Omit<BlogPost, "content">;

export interface BlogPagination {
  currentPage: number;
  totalPages: number;
  totalPosts: number;
}

export interface BlogListPageData extends BlogPagination {
  posts: BlogPostSummary[];
}
