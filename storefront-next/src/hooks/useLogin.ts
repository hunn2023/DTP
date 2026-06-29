"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { toast } from "sonner";
import { useAuthStore } from "@/lib/authStore";
import { AuthApiError } from "@/lib/authApi";
import { useShallow } from "zustand/react/shallow";
import type { LoginPayload } from "@/types/auth";

export function useLogin() {
  const { login, isLoading } = useAuthStore(
    useShallow((s) => ({ login: s.login, isLoading: s.isLoading }))
  );
  const router = useRouter();
  const searchParams = useSearchParams();

  const handleLogin = async (payload: LoginPayload) => {
    try {
      await login(payload);
      const user = useAuthStore.getState().user;
      toast.success(`Chào mừng trở lại, ${user?.name ?? "bạn"}!`);
      const returnUrl = searchParams.get("returnUrl") ?? "/";
      router.push(returnUrl);
      router.refresh();
    } catch (error) {
      if (error instanceof AuthApiError) {
        toast.error(error.message);
      } else {
        toast.error("Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.");
      }
    }
  };

  return { handleLogin, isLoading };
}
