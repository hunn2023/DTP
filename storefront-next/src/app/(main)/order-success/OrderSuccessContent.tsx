"use client";

import { Suspense } from "react";
import { useSearchParams } from "next/navigation";
import SuccessCard from "@/components/order-success/SuccessCard";
import OrderSummary from "@/components/order-success/OrderSummary";
import SuccessActions from "@/components/order-success/SuccessActions";

type SearchParam = string | null;

function safeString(val: SearchParam): string | undefined {
  if (!val || val.trim() === "") return undefined;
  return val.trim();
}

function safeTotal(val: SearchParam): number | undefined {
  const s = safeString(val);
  if (!s) return undefined;
  const n = parseFloat(s);
  return isNaN(n) || n < 0 ? undefined : n;
}

export default function OrderSuccessContent() {
  return (
    <Suspense fallback={<div className="min-h-[200px]" />}>
      <OrderSuccessContentInner />
    </Suspense>
  );
}

function OrderSuccessContentInner() {
  const searchParams = useSearchParams();

  const orderId = safeString(searchParams.get("orderId"));
  const orderCode = safeString(searchParams.get("orderCode")) ?? orderId;
  const total = safeTotal(searchParams.get("total"));
  const paymentMethod = safeString(searchParams.get("paymentMethod"));

  const hasOrderInfo =
    orderCode !== undefined ||
    total !== undefined ||
    paymentMethod !== undefined;

  return (
    <div className="w-full max-w-lg space-y-3">
      <SuccessCard />
      {hasOrderInfo && (
        <OrderSummary
          orderCode={orderCode}
          total={total}
          paymentMethod={paymentMethod}
        />
      )}
      <SuccessActions />
    </div>
  );
}
