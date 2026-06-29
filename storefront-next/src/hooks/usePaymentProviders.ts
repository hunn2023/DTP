"use client";

import { useCallback, useEffect, useState } from "react";
import { getPaymentProviders, type PaymentProvider } from "@/lib/api/paymentProviderApi";

export function usePaymentProviders() {
  const [providers, setProviders] = useState<PaymentProvider[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const data = await getPaymentProviders();
      setProviders(data);
      if (data.length === 0) {
        setError("Không có phương thức thanh toán khả dụng.");
      }
    } catch {
      setProviders([]);
      setError("Không thể tải phương thức thanh toán. Vui lòng thử lại.");
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  return { providers, isLoading, error, reload: load };
}
