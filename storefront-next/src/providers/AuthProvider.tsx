"use client";

import { useEffect } from "react";
import { useAuthStore } from "@/lib/authStore";

export default function AuthProvider({ children }: { children: React.ReactNode }) {
  const initSession = useAuthStore((s) => s.initSession);

  useEffect(() => {
    initSession();
  }, [initSession]);

  return <>{children}</>;
}
