import type { User } from "@/types/user";

export function isValidToken(token: unknown): token is string {
  return typeof token === "string" && token.trim().length > 0;
}

export function isValidUser(value: unknown): value is User {
  if (!value || typeof value !== "object") return false;
  const u = value as Record<string, unknown>;
  return (
    typeof u.id === "string" &&
    u.id.length > 0 &&
    typeof u.name === "string" &&
    typeof u.email === "string" &&
    u.email.includes("@")
  );
}
