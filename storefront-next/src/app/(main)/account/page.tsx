"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/hooks/useAuth";
import AccountSkeleton from "@/components/account/AccountSkeleton";
import AccountInfo from "@/components/account/AccountInfo";
import UpdateProfileForm from "@/components/account/UpdateProfileForm";
import ChangePasswordForm from "@/components/account/ChangePasswordForm";
import { useLanguage } from "@/hooks/useLanguage";

export default function AccountPage() {
  const { initialized, isAuthenticated } = useAuth();
  const router = useRouter();
  const { language } = useLanguage();

  const text = {
    memberPortal: language === "vi" ? "Cổng thành viên" : "Member portal",
    title: language === "vi" ? "Tài khoản của tôi" : "My account",
    description:
      language === "vi"
        ? "Quản lý thông tin cá nhân và cài đặt tài khoản"
        : "Manage your personal information and account settings",
  };

  useEffect(() => {
    if (initialized && !isAuthenticated) {
      router.replace("/login?returnUrl=/account");
    }
  }, [initialized, isAuthenticated, router]);

  if (!initialized || !isAuthenticated) {
    return <AccountSkeleton />;
  }

  return (
    <div className="relative bg-gradient-to-br from-slate-50 via-blue-50/30 to-cyan-50/20 px-4 py-10 md:py-14 overflow-hidden">
      {/* Decorative blobs */}
      <div
        aria-hidden
        className="pointer-events-none absolute -top-40 -left-40 w-[500px] h-[500px] rounded-full bg-primary/4 blur-3xl"
      />
      <div
        aria-hidden
        className="pointer-events-none absolute -bottom-40 -right-40 w-[500px] h-[500px] rounded-full bg-secondary/5 blur-3xl"
      />

      <div className="relative max-w-5xl mx-auto">
        {/* Page heading */}
        <div className="mb-8">
          <p className="text-xs font-semibold text-primary uppercase tracking-widest mb-1">
            {text.memberPortal}
          </p>
          <h1 className="text-2xl md:text-3xl font-bold text-navy leading-tight">
            {text.title}
          </h1>
          <p className="text-sm text-gray-500 mt-1">
            {text.description}
          </p>
        </div>

        {/* Layout */}
        <div className="flex flex-col lg:flex-row gap-6 items-start">
          {/* Left: profile summary */}
          <div className="w-full lg:w-72 flex-shrink-0">
            <AccountInfo />
          </div>

          {/* Right: edit form + change password */}
          <div className="flex-1 w-full space-y-6">
            <UpdateProfileForm />
            <ChangePasswordForm />
          </div>
        </div>
      </div>
    </div>
  );
}
