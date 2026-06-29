import type {
  LoginPayload,
  LoginResponse,
  RegisterPayload,
  RegisterResponse,
} from "@/types/auth";
import type { User } from "@/types/user";
import type { ApiLoginResponse, ApiUserProfile } from "@/types/api";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

const TIMEOUT_MS = 30_000;

export class AuthApiError extends Error {
  constructor(
    message: string,
    public readonly statusCode: number
  ) {
    super(message);
    this.name = "AuthApiError";
  }
}

async function request<T>(
  path: string,
  options: RequestInit,
  getErrorMessage: (status: number) => string
): Promise<T> {
  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), TIMEOUT_MS);

  try {
    const response = await fetchWithAuth(path, {
      ...options,
      signal: controller.signal,
      _skipRefresh: true,
    });

    clearTimeout(timeoutId);

    const json = await response.json().catch(() => ({}));

    // Unwrap { isSuccess, data, error } wrapper
    if (typeof json === "object" && json !== null && "isSuccess" in json) {
      if (!json.isSuccess || !response.ok) {
        throw new AuthApiError(
          json.error ?? json.message ?? getErrorMessage(response.status),
          response.status
        );
      }
      return json.data as T;
    }

    if (!response.ok) {
      throw new AuthApiError(
        (json as { message?: string }).message ?? getErrorMessage(response.status),
        response.status
      );
    }

    return json as T;
  } catch (error) {
    clearTimeout(timeoutId);
    if (error instanceof AuthApiError) throw error;
    if (error instanceof Error) {
      if (error.name === "AbortError") {
        throw new AuthApiError("Yêu cầu bị timeout. Vui lòng thử lại.", 408);
      }
      if (error.message === "Failed to fetch") {
        throw new AuthApiError(
          "Không thể kết nối API. Hãy chạy `npm run dev` (cần cả proxy cổng 4000).",
          0
        );
      }
      throw new AuthApiError(error.message, 500);
    }
    throw new AuthApiError("Lỗi không xác định.", 500);
  }
}

function mapProfileToUser(profile: ApiUserProfile): User {
  return {
    id: profile.id,
    name: profile.fullName || profile.email,
    email: profile.email,
    phone: profile.phone || undefined,
    avatar: profile.avatarUrl || undefined,
  };
}

export async function getMe(token: string): Promise<User> {
  const profile = await request<ApiUserProfile>(
    "/api/auth/profile",
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    },
    (status) => {
      if (status === 401) return "Phiên đăng nhập đã hết hạn.";
      if (status >= 500) return "Server đang gặp sự cố. Vui lòng thử lại sau.";
      return "Không thể lấy thông tin người dùng.";
    }
  );
  return mapProfileToUser(profile);
}

export async function login(payload: LoginPayload): Promise<LoginResponse> {
  const data = await request<ApiLoginResponse>(
    "/api/auth/login",
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: payload.email, password: payload.password }),
    },
    (status) => {
      if (status === 401) return "Email hoặc mật khẩu không chính xác.";
      if (status === 403) return "Tài khoản đã bị khóa.";
      if (status === 429) return "Quá nhiều lần đăng nhập. Vui lòng thử lại sau.";
      if (status >= 500) return "Server đang gặp sự cố. Vui lòng thử lại sau.";
      return "Đăng nhập thất bại. Vui lòng thử lại.";
    }
  );

  // Fetch profile if not included in login response
  let user: User;
  if (data.user) {
    user = mapProfileToUser(data.user);
  } else {
    user = await getMe(data.accessToken);
  }

  return {
    accessToken: data.accessToken,
    refreshToken: data.refreshToken,
    user,
  };
}

export async function register(payload: RegisterPayload): Promise<RegisterResponse> {
  return request<RegisterResponse>(
    "/api/auth/register",
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        email: payload.email,
        phone: payload.phone,
        fullName: payload.name,
        password: payload.password,
      }),
    },
    (status) => {
      if (status === 409) return "Email này đã được sử dụng. Vui lòng dùng email khác.";
      if (status === 422) return "Thông tin đăng ký không hợp lệ.";
      if (status === 429) return "Quá nhiều yêu cầu. Vui lòng thử lại sau.";
      if (status >= 500) return "Server đang gặp sự cố. Vui lòng thử lại sau.";
      return "Đăng ký thất bại. Vui lòng thử lại.";
    }
  );
}

export async function verifyRegisterOtp(email: string, otpCode: string): Promise<void> {
  const result = await request<{ success?: { isSuccess: boolean; error?: string }; message?: string }>(
    "/api/auth/verify-register-otp",
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        email,
        otpCode,
        ipAddress:"1",
        userAgent: typeof navigator !== "undefined" ? navigator.userAgent : undefined,
      }),
    },
    (status) => {
      if (status === 400) return "Mã OTP không chính xác hoặc đã hết hạn.";
      if (status === 404) return "Không tìm thấy yêu cầu đăng ký.";
      if (status === 429) return "Quá nhiều lần thử. Vui lòng đợi và thử lại.";
      if (status >= 500) return "Server đang gặp sự cố. Vui lòng thử lại sau.";
      return "Xác thực OTP thất bại. Vui lòng thử lại.";
    }
  );

  if (result?.success && !result.success.isSuccess) {
    throw new AuthApiError(
      result.success.error ?? "Xác thực OTP thất bại.",
      400
    );
  }
}

export async function resendRegisterOtp(email: string): Promise<void> {
  await request<unknown>(
    "/api/auth/register/resend-otp",
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email }),
    },
    (status) => {
      if (status === 404) return "Không tìm thấy yêu cầu đăng ký cho email này.";
      if (status === 429) return "Vui lòng đợi trước khi gửi lại mã OTP.";
      if (status >= 500) return "Server đang gặp sự cố. Vui lòng thử lại sau.";
      return "Gửi lại OTP thất bại. Vui lòng thử lại.";
    }
  );
}

export async function logoutApi(refreshToken: string): Promise<void> {
  // Fire and forget — don't block UI on logout failure
  try {
    await fetchWithAuth("/api/auth/logout", {
      method: "POST",
      body: JSON.stringify({ refreshToken }),
      _skipRefresh: true,
    });
  } catch {
    // ignore
  }
}
