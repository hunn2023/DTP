"use client";

import { Toaster } from "sonner";

export default function ToastProvider() {
  return (
    <Toaster
      position="top-right"
      richColors
      closeButton
      toastOptions={{
        classNames: {
          error: "bg-danger text-white",
          success: "bg-success text-white",
          warning: "bg-warning text-navy",
          info: "bg-primary text-white",
        },
      }}
    />
  );
}