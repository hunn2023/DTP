"use client";

import { useState } from "react";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";
import { useLanguage } from "@/hooks/useLanguage";
import { updateProfile } from "@/lib/accountApi";
import { AuthApiError } from "@/lib/authApi";
import type { ProfileFormData } from "@/lib/schemas/profileSchema";

export function useProfile() {
  const [isSaving, setIsSaving] = useState(false);
  const { setUser } = useAuth();
  const { language } = useLanguage();

  const saveProfile = async (data: ProfileFormData) => {
    if (isSaving) return;
    setIsSaving(true);
    try {
      const updatedUser = await updateProfile({
        name: data.name,
        phone: data.phone,
      });
      setUser(updatedUser);
      toast.success(language === "vi" ? "Cập nhật thông tin thành công!" : "Profile updated successfully!");
    } catch (error) {
      if (error instanceof AuthApiError) {
        toast.error(error.message);
      } else {
        toast.error(language === "vi" ? "Đã xảy ra lỗi. Vui lòng thử lại." : "An error occurred. Please try again.");
      }
    } finally {
      setIsSaving(false);
    }
  };

  return { saveProfile, isSaving };
}
