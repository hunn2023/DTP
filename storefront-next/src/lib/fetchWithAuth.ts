import { authStorage } from "@/lib/storage";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "";

async function updateAuthToken(token: string): Promise<void> {
  try {
    const { useAuthStore } = await import("@/lib/authStore");
    useAuthStore.setState({ token });
  } catch {
    // Auth store may not be available during server/static rendering.
  }
}

async function logoutAuthStore(): Promise<void> {
  try {
    const { useAuthStore } = await import("@/lib/authStore");
    useAuthStore.getState().logout();
  } catch {
    authStorage.clearAll();
  }
}

export type FetchWithAuthOptions = RequestInit & {
  /** Skip auto-prefixing with API_BASE_URL — use for absolute URLs */
  absoluteUrl?: boolean;
  /** Skip refresh token retry on 401 (used internally to avoid loops) */
  _skipRefresh?: boolean;
};

async function tryRefreshToken(): Promise<string | null> {
  const refreshToken = authStorage.getRefreshToken();
  if (!refreshToken) return null;

  try {
    const response = await fetch(`${API_BASE_URL}/api/auth/refresh-token`, {
      method: "POST",
      headers: buildAuthHeaders({ "Content-Type": "application/json" }),
      body: JSON.stringify({ refreshToken }),
    });

    if (!response.ok) return null;

    const json = await response.json();
    // Unwrap { isSuccess, data } wrapper
    const payload = json.data ?? json;
    const newAccessToken = payload.accessToken as string | undefined;
    const newRefreshToken = payload.refreshToken as string | undefined;

    if (newAccessToken) {
      authStorage.setToken(newAccessToken);
      if (newRefreshToken) authStorage.setRefreshToken(newRefreshToken);
      await updateAuthToken(newAccessToken);
      return newAccessToken;
    }
    return null;
  } catch {
    return null;
  }
}

function buildAuthHeaders(init?: HeadersInit): Headers {
  const token = authStorage.getToken();
  const headers = new Headers(init);
  if (!headers.has("Content-Type")) headers.set("Content-Type", "application/json");
  if (token) headers.set("Authorization", `Bearer ${token}`);
  return headers;
}

export async function fetchWithAuth(
  path: string,
  options: FetchWithAuthOptions = {}
): Promise<Response> {
  const { absoluteUrl, _skipRefresh, ...fetchOptions } = options;

  const headers = buildAuthHeaders(fetchOptions.headers);

  const url = absoluteUrl ? path : `${API_BASE_URL}${path}`;
  const response = await fetch(url, { ...fetchOptions, headers });

  // On 401, try to refresh token before logging out
  if (response.status === 401 && !_skipRefresh) {
    const newToken = await tryRefreshToken();
    if (newToken) {
      // Retry the original request with the new token
      const retryHeaders = new Headers(fetchOptions.headers);
      if (!retryHeaders.has("Content-Type")) retryHeaders.set("Content-Type", "application/json");
      retryHeaders.set("Authorization", `Bearer ${newToken}`);
      return fetch(url, { ...fetchOptions, headers: retryHeaders });
    }
    // Refresh failed — logout
    await logoutAuthStore();
  }

  return response;
}
