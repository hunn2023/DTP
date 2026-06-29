"use client";

import { useState } from "react";
import { toast } from "sonner";
import Icon from "@/components/ui/Icon";
import { fetchWithAuth } from "@/lib/fetchWithAuth";

function getApiErrorMessage(json: unknown, fallback: string): string {
  if (typeof json !== "object" || json === null) return fallback;

  const payload = json as { error?: string | null; message?: string | null };
  if (typeof payload.error === "string" && payload.error.trim()) return payload.error;
  if (typeof payload.message === "string" && payload.message.trim()) return payload.message;

  return fallback;
}

interface PasswordFieldProps {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  show: boolean;
  onToggleShow: () => void;
  placeholder: string;
  leftIcon: "lock";
  disabled?: boolean;
}

function PasswordField({
  id,
  label,
  value,
  onChange,
  show,
  onToggleShow,
  placeholder,
  leftIcon,
  disabled = false,
}: PasswordFieldProps) {
  return (
    <div>
      <label htmlFor={id} className="block text-sm font-semibold text-navy mb-1.5">
        {label}
      </label>
      <div className="relative">
        <span className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 pointer-events-none">
          <Icon icon={leftIcon} className="text-sm" />
        </span>
        <input
          id={id}
          type={show ? "text" : "password"}
          value={value}
          onChange={(e) => onChange(e.target.value)}
          placeholder={placeholder}
          disabled={disabled}
          className="input pl-10 pr-12 disabled:bg-gray-50 disabled:cursor-not-allowed"
        />
        <button
          type="button"
          onClick={onToggleShow}
          disabled={disabled}
          aria-label={show ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
          className="absolute right-3 top-1/2 -translate-y-1/2 flex h-9 w-9 items-center justify-center rounded-lg text-gray-400 transition hover:bg-gray-100 hover:text-navy disabled:cursor-not-allowed disabled:opacity-50"
        >
          <Icon icon={show ? "eye-slash" : "eye"} className="text-sm" />
        </button>
      </div>
    </div>
  );
}

export default function ChangePasswordForm() {
  const [expanded, setExpanded] = useState(false);
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [showCurrent, setShowCurrent] = useState(false);
  const [showNew, setShowNew] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (newPassword.length < 6) {
      toast.error("Mật khẩu mới phải có ít nhất 6 ký tự.");
      return;
    }

    if (newPassword !== confirmPassword) {
      toast.error("Mật khẩu xác nhận không khớp.");
      return;
    }

    setLoading(true);
    try {
      const res = await fetchWithAuth("/api/auth/change-password", {
        method: "POST",
        body: JSON.stringify({
          currentPassword,
          newPassword,
          confirmNewPassword: confirmPassword,
        }),
        _skipRefresh: true,
      });

      const json = await res.json().catch(() => ({}));
      const isFailure =
        !res.ok || (typeof json === "object" && json !== null && "isSuccess" in json && json.isSuccess === false);

      if (isFailure) {
        toast.error(getApiErrorMessage(json, "Đổi mật khẩu thất bại. Vui lòng thử lại."));
        return;
      }

      toast.success("Đổi mật khẩu thành công!");
      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");
      setExpanded(false);
    } catch {
      toast.error("Có lỗi xảy ra. Vui lòng thử lại.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-white rounded-2xl shadow-card border border-gray-100 overflow-hidden">
      <button
        type="button"
        onClick={() => setExpanded(!expanded)}
        className="w-full px-6 md:px-8 py-5 flex items-center justify-between hover:bg-gray-50/50 transition"
      >
        <div className="text-left">
          <h3 className="font-bold text-navy text-base">Đổi mật khẩu</h3>
          <p className="text-xs text-gray-400 mt-0.5">Cập nhật mật khẩu đăng nhập</p>
        </div>
        <div className="w-9 h-9 rounded-xl gradient-primary flex items-center justify-center shadow-btn flex-shrink-0">
          <Icon icon={expanded ? "chevron-up" : "lock"} className="text-white text-xs" />
        </div>
      </button>

      {expanded && (
        <form onSubmit={handleSubmit} noValidate className="p-6 md:p-8 pt-0 space-y-5 border-t border-gray-100">
          <PasswordField
            id="cp-current"
            label="Mật khẩu hiện tại"
            value={currentPassword}
            onChange={setCurrentPassword}
            show={showCurrent}
            onToggleShow={() => setShowCurrent((value) => !value)}
            placeholder="••••••••"
            leftIcon="lock"
            disabled={loading}
          />

          <PasswordField
            id="cp-new"
            label="Mật khẩu mới"
            value={newPassword}
            onChange={setNewPassword}
            show={showNew}
            onToggleShow={() => setShowNew((value) => !value)}
            placeholder="Nhập mật khẩu mới"
            leftIcon="lock"
            disabled={loading}
          />

          <PasswordField
            id="cp-confirm"
            label="Xác nhận mật khẩu mới"
            value={confirmPassword}
            onChange={setConfirmPassword}
            show={showConfirm}
            onToggleShow={() => setShowConfirm((value) => !value)}
            placeholder="Nhập lại mật khẩu mới"
            leftIcon="lock"
            disabled={loading}
          />

          <div className="pt-1">
            <button
              type="submit"
              disabled={loading || !currentPassword || !newPassword || !confirmPassword}
              className="btn btn-primary px-8 py-3 disabled:opacity-50 disabled:cursor-not-allowed active:scale-[0.98]"
            >
              {loading ? "Đang xử lý..." : "Đổi mật khẩu"}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}
