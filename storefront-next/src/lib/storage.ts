const TOKEN_KEY = "ezsim_token";
const REFRESH_TOKEN_KEY = "ezsim_refresh_token";

const isClient = () => typeof window !== "undefined";

function safeGetLocal(key: string): string | null {
  try {
    return isClient() ? localStorage.getItem(key) : null;
  } catch {
    return null;
  }
}

function safeSetLocal(key: string, value: string): void {
  try {
    if (isClient()) localStorage.setItem(key, value);
  } catch {
    // quota exceeded or access blocked — fail silently
  }
}

function safeRemoveLocal(key: string): void {
  try {
    if (isClient()) localStorage.removeItem(key);
  } catch {
    // ignore
  }
}

function safeGetSession(key: string): string | null {
  try {
    return isClient() ? sessionStorage.getItem(key) : null;
  } catch {
    return null;
  }
}

function safeSetSession(key: string, value: string): void {
  try {
    if (isClient()) sessionStorage.setItem(key, value);
  } catch {
    // quota exceeded or access blocked — fail silently
  }
}

function safeRemoveSession(key: string): void {
  try {
    if (isClient()) sessionStorage.removeItem(key);
  } catch {
    // ignore
  }
}

export const authStorage = {
  getToken(): string | null {
    return safeGetLocal(TOKEN_KEY);
  },
  setToken(token: string): void {
    safeSetLocal(TOKEN_KEY, token);
  },
  getRefreshToken(): string | null {
    const fromSession = safeGetSession(REFRESH_TOKEN_KEY);
    if (fromSession) return fromSession;

    const legacy = safeGetLocal(REFRESH_TOKEN_KEY);
    if (!legacy) return null;

    safeSetSession(REFRESH_TOKEN_KEY, legacy);
    safeRemoveLocal(REFRESH_TOKEN_KEY);
    return legacy;
  },
  setRefreshToken(token: string): void {
    safeSetSession(REFRESH_TOKEN_KEY, token);
    safeRemoveLocal(REFRESH_TOKEN_KEY);
  },
  clearAll(): void {
    safeRemoveLocal(TOKEN_KEY);
    safeRemoveSession(REFRESH_TOKEN_KEY);
    safeRemoveLocal(REFRESH_TOKEN_KEY);
  },
  clearToken(): void {
    safeRemoveLocal(TOKEN_KEY);
  },
};
