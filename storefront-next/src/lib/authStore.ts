import { create } from "zustand";
import type { User } from "@/types/user";
import type { LoginPayload } from "@/types/auth";
import { authStorage } from "@/lib/storage";
import { getMe, login as apiLogin, logoutApi, AuthApiError } from "@/lib/authApi";
import { isValidToken, isValidUser } from "@/utils/auth";

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  initialized: boolean;

  setAuth: (token: string, user: User) => void;
  setUser: (user: User) => void;
  login: (payload: LoginPayload) => Promise<void>;
  logout: () => void;
  initSession: () => Promise<void>;
}

export const useAuthStore = create<AuthState>()((set, get) => ({
  user: null,
  token: null,
  isAuthenticated: false,
  isLoading: false,
  initialized: false,

  setAuth: (token, user) => {
    authStorage.setToken(token);
    set({ token, user, isAuthenticated: true });
  },

  setUser: (user) => {
    set({ user });
  },

  login: async (payload) => {
    if (get().isLoading) return;
    set({ isLoading: true });
    try {
      const { accessToken, refreshToken, user } = await apiLogin(payload);
      authStorage.setToken(accessToken);
      authStorage.setRefreshToken(refreshToken);
      set({ token: accessToken, user, isAuthenticated: true, isLoading: false });
    } catch (error) {
      set({ isLoading: false });
      throw error;
    }
  },

  logout: () => {
    const refreshToken = authStorage.getRefreshToken();
    if (refreshToken) {
      logoutApi(refreshToken);
    }
    authStorage.clearAll();
    set({ token: null, user: null, isAuthenticated: false });
  },

  initSession: async () => {
    if (get().initialized) return;

    const token = authStorage.getToken();
    if (!isValidToken(token)) {
      set({ initialized: true });
      return;
    }

    set({ isLoading: true });
    try {
      const user = await getMe(token);
      if (!isValidUser(user)) {
        throw new AuthApiError("Dữ liệu người dùng không hợp lệ.", 500);
      }
      set({ token, user, isAuthenticated: true, initialized: true, isLoading: false });
    } catch {
      authStorage.clearAll();
      set({ token: null, user: null, isAuthenticated: false, initialized: true, isLoading: false });
    }
  },
}));

// Standalone selectors — safe to call outside React components (e.g. fetch interceptors)
export const getToken = (): string | null => useAuthStore.getState().token;
export const getUser = (): User | null => useAuthStore.getState().user;
export const isAuthenticated = (): boolean => useAuthStore.getState().isAuthenticated;
