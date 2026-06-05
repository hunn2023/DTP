/** URL backend — đọc từ .env, dùng cho Vite proxy (dev) và fetch trực tiếp (prod). */
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5104'

/**
 * Dev: gọi `/api/...` cùng origin → Vite proxy sang VITE_API_BASE_URL (không cần CORS).
 * Prod: gọi thẳng API_BASE_URL.
 */
export const API_REQUEST_BASE = import.meta.env.DEV ? '' : API_BASE_URL

export const API_PATHS = {
  adminCategories: '/api/admin/catalog/categories',
  publicCategories: '/api/catalog/categories',
} as const
