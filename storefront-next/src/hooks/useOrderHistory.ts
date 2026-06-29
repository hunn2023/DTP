"use client";

import { useState, useEffect, useCallback } from "react";
import { getMyOrders } from "@/lib/orderApi";
import type { OrderHistoryItem, OrderFilters } from "@/lib/orderApi";
import type { Language } from "@/lib/i18n";
import { useAuthStore } from "@/lib/authStore";

export interface OrderListFilters {
  keyword: string;
  status: number | undefined;
  paymentStatus: number | undefined;
}

interface UseOrderHistoryReturn {
  orders: OrderHistoryItem[];
  isLoading: boolean;
  error: string | null;
  page: number;
  totalPages: number;
  filters: OrderListFilters;
  setPage: (page: number) => void;
  setFilters: (filters: OrderListFilters) => void;
  refetch: () => void;
}

export function useOrderHistory(language: Language = "vi"): UseOrderHistoryReturn {
  const user = useAuthStore((s) => s.user);
  const [orders, setOrders] = useState<OrderHistoryItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [filters, setFilters] = useState<OrderListFilters>({
    keyword: "",
    status: undefined,
    paymentStatus: undefined,
  });

  const fetchOrders = useCallback(async (p: number, customerId: string, f: OrderListFilters) => {
    setIsLoading(true);
    setError(null);
    try {
      const apiFilters: OrderFilters = {
        customerId,
        keyword: f.keyword || undefined,
        status: f.status,
        paymentStatus: f.paymentStatus,
      };
      const result = await getMyOrders(p, apiFilters);
      setOrders(result.orders);
      setTotalPages(Math.ceil(result.total / result.pageSize));
    } catch {
      setError(
        language === "vi"
          ? "Không thể tải danh sách đơn hàng. Vui lòng thử lại."
          : "Unable to load order history. Please try again."
      );
    } finally {
      setIsLoading(false);
    }
  }, [language]);

  useEffect(() => {
    if (!user?.id) return;
    fetchOrders(page, user.id, filters);
  }, [page, user?.id, filters, fetchOrders]);

  const handleSetFilters = useCallback((newFilters: OrderListFilters) => {
    setFilters(newFilters);
    setPage(1); // Reset to first page on filter change
  }, []);

  return {
    orders,
    isLoading,
    error,
    page,
    totalPages,
    filters,
    setPage,
    setFilters: handleSetFilters,
    refetch: () => { if (user?.id) fetchOrders(page, user.id, filters); },
  };
}
