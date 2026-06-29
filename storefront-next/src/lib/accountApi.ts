import { fetchWithAuth } from "@/lib/fetchWithAuth";
import { AuthApiError } from "@/lib/authApi";
import type { User } from "@/types/user";

export interface UpdateProfilePayload {
  name: string;
  phone: string;
  address?: string;
}

export async function updateProfile(payload: UpdateProfilePayload): Promise<User> {
  const response = await fetchWithAuth("/account/profile", {
    method: "PUT",
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const data = await response.json().catch(() => ({}));
    const message = (data as { message?: string }).message;
    if (response.status === 422) throw new AuthApiError(message ?? "Thông tin không hợp lệ.", 422);
    if (response.status >= 500) throw new AuthApiError(message ?? "Server gặp sự cố. Vui lòng thử lại.", 500);
    throw new AuthApiError(message ?? "Cập nhật thất bại. Vui lòng thử lại.", response.status);
  }

  return response.json() as Promise<User>;
}
