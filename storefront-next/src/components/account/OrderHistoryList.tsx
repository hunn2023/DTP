"use client";

import { useMemo, useState } from "react";
import Icon from "@/components/ui/Icon";
import OrderHistoryItem from "@/components/account/OrderHistoryItem";
import { useOrderHistory } from "@/hooks/useOrderHistory";
import type { Language } from "@/lib/i18n";

// ── Skeleton ─────────────────────────────────────────────────────────────────

function OrderSkeleton() {
  return (
    <div className="space-y-4 animate-pulse">
      {[1, 2, 3].map((i) => (
        <div key={i} className="bg-white rounded-2xl border border-gray-100 overflow-hidden">
          <div className="flex items-center justify-between px-5 py-4 border-b border-gray-50">
            <div className="flex gap-3">
              <div className="h-6 w-24 bg-gray-200 rounded-full" />
              <div className="h-6 w-28 bg-gray-100 rounded-full" />
            </div>
            <div className="h-6 w-20 bg-gray-100 rounded-full" />
          </div>
          <div className="px-5 py-3.5 border-b border-gray-50">
            <div className="h-4 w-2/3 bg-gray-100 rounded" />
          </div>
          <div className="flex items-center justify-between px-5 py-3.5">
            <div className="flex gap-4">
              <div className="h-4 w-24 bg-gray-100 rounded" />
              <div className="h-5 w-20 bg-gray-200 rounded" />
            </div>
            <div className="h-4 w-20 bg-gray-100 rounded" />
          </div>
        </div>
      ))}
    </div>
  );
}

// ── Empty state ───────────────────────────────────────────────────────────────

function EmptyState({ language }: { language: Language }) {
  const title = language === "vi" ? "Chưa có đơn hàng nào" : "No orders yet";
  const description =
    language === "vi"
      ? "Khi bạn đặt hàng, đơn hàng sẽ hiển thị tại đây."
      : "When you place an order, it will appear here.";

  return (
    <div className="bg-white rounded-2xl border border-gray-100 px-6 py-16 text-center">
      <div className="w-16 h-16 rounded-full bg-gray-50 border border-gray-100 flex items-center justify-center mx-auto mb-4">
        <Icon icon="shopping-cart" className="text-gray-300 text-2xl" />
      </div>
      <h3 className="font-semibold text-navy mb-1">{title}</h3>
      <p className="text-sm text-gray-400">
        {description}
      </p>
    </div>
  );
}

// ── Error state ───────────────────────────────────────────────────────────────

function ErrorState({ message, onRetry, language }: { message: string; onRetry: () => void; language: Language }) {
  const retryLabel = language === "vi" ? "Thử lại" : "Try again";

  return (
    <div className="bg-white rounded-2xl border border-red-100 px-6 py-12 text-center">
      <div className="w-12 h-12 rounded-full bg-red-50 flex items-center justify-center mx-auto mb-3">
        <Icon icon="times" className="text-danger text-lg" />
      </div>
      <p className="text-sm text-gray-600 mb-4">{message}</p>
      <button
        onClick={onRetry}
        className="btn btn-outline btn-sm"
      >
        {retryLabel}
      </button>
    </div>
  );
}

// ── Pagination ────────────────────────────────────────────────────────────────

interface PaginationProps {
  page: number;
  totalPages: number;
  onPageChange: (p: number) => void;
  language: Language;
}

function Pagination({ page, totalPages, onPageChange, language }: PaginationProps) {
  if (totalPages <= 1) return null;

  const prevLabel = language === "vi" ? "Trang trước" : "Previous page";
  const nextLabel = language === "vi" ? "Trang sau" : "Next page";

  return (
    <div className="flex items-center justify-center gap-2 mt-6">
      <button
        onClick={() => onPageChange(page - 1)}
        disabled={page === 1}
        aria-label={prevLabel}
        className="w-9 h-9 rounded-xl border border-gray-200 flex items-center justify-center text-gray-500 hover:border-primary hover:text-primary transition disabled:opacity-40 disabled:cursor-not-allowed"
      >
        <Icon icon="chevron-left" className="text-xs" />
      </button>

      {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
        <button
          key={p}
          onClick={() => onPageChange(p)}
          aria-current={p === page ? "page" : undefined}
          className={`w-9 h-9 rounded-xl border text-sm font-semibold transition ${
            p === page
              ? "gradient-primary text-white border-transparent shadow-sm"
              : "border-gray-200 text-gray-600 hover:border-primary hover:text-primary"
          }`}
        >
          {p}
        </button>
      ))}

      <button
        onClick={() => onPageChange(page + 1)}
        disabled={page === totalPages}
        aria-label={nextLabel}
        className="w-9 h-9 rounded-xl border border-gray-200 flex items-center justify-center text-gray-500 hover:border-primary hover:text-primary transition disabled:opacity-40 disabled:cursor-not-allowed"
      >
        <Icon icon="chevron-right" className="text-xs" />
      </button>
    </div>
  );
}

// ── Main component ────────────────────────────────────────────────────────────

interface OrderHistoryListProps {
  language?: Language;
}

export default function OrderHistoryList({ language = "vi" }: OrderHistoryListProps) {
  const { orders, isLoading, error, page, totalPages, filters, setPage, setFilters, refetch } = useOrderHistory(language);
  const [keyword, setKeyword] = useState("");

  const filteredOrders = useMemo(() => {
    const normalizedKeyword = keyword.trim().toLowerCase();
    if (!normalizedKeyword) return orders;

    return orders.filter((order) => {
      const codeMatches = order.orderCode.toLowerCase().includes(normalizedKeyword);
      const nameMatches = order.items.some((item) => item.name.toLowerCase().includes(normalizedKeyword));
      return codeMatches || nameMatches;
    });
  }, [orders, keyword]);

  const statusOptions: { value: number | undefined; label: string }[] = [
    { value: undefined, label: language === "vi" ? "Tất cả trạng thái" : "All statuses" },
    { value: 1, label: language === "vi" ? "Chờ xác nhận" : "Pending" },
    { value: 2, label: language === "vi" ? "Đã xác nhận" : "Confirmed" },
    { value: 3, label: language === "vi" ? "Đang xử lý" : "Processing" },
    { value: 4, label: language === "vi" ? "Đang giao" : "Shipping" },
    { value: 5, label: language === "vi" ? "Đã giao" : "Delivered" },
    { value: 6, label: language === "vi" ? "Đã hủy" : "Cancelled" },
    { value: 7, label: language === "vi" ? "Hoàn tiền" : "Refunded" },
  ];

  const paymentStatusOptions: { value: number | undefined; label: string }[] = [
    { value: undefined, label: language === "vi" ? "Tất cả TT thanh toán" : "All payment statuses" },
    { value: 1, label: language === "vi" ? "Chờ thanh toán" : "Pending" },
    { value: 2, label: language === "vi" ? "Đã thanh toán" : "Paid" },
    { value: 3, label: language === "vi" ? "Thanh toán thất bại" : "Failed" },
    { value: 4, label: language === "vi" ? "Đã hoàn tiền" : "Refunded" },
    { value: 5, label: language === "vi" ? "Đã hủy" : "Cancelled" },
  ];

  const text = {
    searchPlaceholder: language === "vi" ? "Nhập mã đơn hàng..." : "Type order code...",
    noSearchResultTitle: language === "vi" ? "Không tìm thấy đơn hàng phù hợp" : "No matching orders found",
    noSearchResultDescription:
      language === "vi"
        ? "Thử từ khóa khác hoặc xóa bộ lọc để xem toàn bộ danh sách."
        : "Try another keyword or clear the filter to view all orders.",
    filterLabel: language === "vi" ? "Lọc đơn hàng" : "Filter orders",
    clearFilters: language === "vi" ? "Xóa bộ lọc" : "Clear filters",
  };

  const hasActiveFilters = filters.status !== undefined || filters.paymentStatus !== undefined || filters.keyword !== "";

  return (
    <div>
      {/* Filter section */}
      <div className="mb-5 space-y-3">
        <div className="flex items-center justify-between">
          <label className="block text-xs font-semibold uppercase tracking-wide text-slate-500">
            {text.filterLabel}
          </label>
          {hasActiveFilters && (
            <button
              onClick={() => {
                setFilters({ keyword: "", status: undefined, paymentStatus: undefined });
                setKeyword("");
              }}
              className="text-xs text-primary hover:text-primary-dark font-medium transition"
            >
              {text.clearFilters}
            </button>
          )}
        </div>

        {/* Keyword search */}
        <div className="relative">
          <span className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">
            <Icon icon="search" className="text-xs" />
          </span>
          <input
            type="text"
            value={keyword}
            onChange={(event) => setKeyword(event.target.value)}
            onKeyDown={(event) => {
              if (event.key === "Enter") {
                setFilters({ ...filters, keyword: keyword.trim() });
              }
            }}
            onBlur={() => {
              if (keyword.trim() !== filters.keyword) {
                setFilters({ ...filters, keyword: keyword.trim() });
              }
            }}
            placeholder={text.searchPlaceholder}
            className="w-full rounded-xl border border-gray-200 bg-white py-2.5 pl-9 pr-3 text-sm text-navy outline-none transition focus:border-primary"
          />
        </div>

        {/* Dropdowns */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <select
            value={filters.status ?? ""}
            onChange={(e) => {
              const val = e.target.value ? Number(e.target.value) : undefined;
              setFilters({ ...filters, status: val });
            }}
            className="w-full rounded-xl border border-gray-200 bg-white py-2.5 px-3 text-sm text-navy outline-none transition focus:border-primary appearance-none cursor-pointer"
          >
            {statusOptions.map((opt) => (
              <option key={opt.value ?? "all"} value={opt.value ?? ""}>
                {opt.label}
              </option>
            ))}
          </select>

          <select
            value={filters.paymentStatus ?? ""}
            onChange={(e) => {
              const val = e.target.value ? Number(e.target.value) : undefined;
              setFilters({ ...filters, paymentStatus: val });
            }}
            className="w-full rounded-xl border border-gray-200 bg-white py-2.5 px-3 text-sm text-navy outline-none transition focus:border-primary appearance-none cursor-pointer"
          >
            {paymentStatusOptions.map((opt) => (
              <option key={opt.value ?? "all"} value={opt.value ?? ""}>
                {opt.label}
              </option>
            ))}
          </select>
        </div>
      </div>

      {isLoading ? (
        <OrderSkeleton />
      ) : error ? (
        <ErrorState message={error} onRetry={refetch} language={language} />
      ) : orders.length === 0 ? (
        hasActiveFilters ? (
          <div className="bg-white rounded-2xl border border-gray-100 px-6 py-12 text-center">
            <h3 className="font-semibold text-navy mb-1">{text.noSearchResultTitle}</h3>
            <p className="text-sm text-gray-400">{text.noSearchResultDescription}</p>
          </div>
        ) : (
          <EmptyState language={language} />
        )
      ) : filteredOrders.length === 0 ? (
        <div className="bg-white rounded-2xl border border-gray-100 px-6 py-12 text-center">
          <h3 className="font-semibold text-navy mb-1">{text.noSearchResultTitle}</h3>
          <p className="text-sm text-gray-400">{text.noSearchResultDescription}</p>
        </div>
      ) : (
        <>
          <div className="space-y-4">
            {filteredOrders.map((order) => (
              <OrderHistoryItem key={order.id} order={order} language={language} />
            ))}
          </div>
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} language={language} />
        </>
      )}
    </div>
  );
}
