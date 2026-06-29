"use client";

import { useAuthStore } from "@/lib/authStore";
import { useShallow } from "zustand/react/shallow";

export function useAuth() {
  return useAuthStore(
    useShallow((s) => ({
      user: s.user,
      token: s.token,
      isAuthenticated: s.isAuthenticated,
      isLoading: s.isLoading,
      initialized: s.initialized,
      setAuth: s.setAuth,
      setUser: s.setUser,
      login: s.login,
      logout: s.logout,
    }))
  );
}
