"use client";

import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import {
  faCheck,
  faCheckCircle,
  faClock,
  faCopy,
  faExclamationTriangle,
  faFileAlt,
  faHeadset,
  faLock,
  faShoppingCart,
  faUniversity,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useCallback, useEffect, useRef, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { toast } from "sonner";

import { useCartStore } from "@/lib/cartStore";
import {
  createPaymentQr,
  getPaymentStatus,
  isPaymentPaid,
  OrderApiError,
  PaymentQrData,
} from "@/lib/orderApi";
import { paymentFlowError, paymentFlowLog } from "@/lib/paymentFlowLog";
import { formatPrice } from "@/lib/product";
import { authStorage } from "@/lib/storage";

interface PaymentPageProps {
  params: { orderId: string };
}

type PaymentPaidEvent = {
  orderId?: string;
  OrderId?: string;
  paymentId?: string;
  orderCode?: string;
  status?: string;
  paidAt?: string;
};

type PaymentSuccessSource = "signalr" | "poll";

const PAYMENT_POLL_INTERVAL_MS = 5_000;
const SIGNALR_START_RETRIES = 4;

function getQrImageUrl(data: PaymentQrData | null): string {
  if (!data) return "";
  return data.qrImageUrl || data.qrCode || data.qrCodeUrl || data.qrDataUrl || data.qrUrl || data.paymentUrl || "";
}

function getOrderCode(data: PaymentQrData | null, fallbackOrderId: string): string {
  return data?.orderCode || fallbackOrderId;
}

function getBankCode(data: PaymentQrData | null): string {
  return data?.bankCode || data?.bankName || "";
}

function getBankAccountNo(data: PaymentQrData | null): string {
  return data?.bankAccountNo || data?.accountNumber || "";
}

function getBankAccountName(data: PaymentQrData | null): string {
  return data?.bankAccountName || data?.accountName || "";
}

function getTransferContent(data: PaymentQrData | null): string {
  return data?.transferContent || data?.content || data?.description || "";
}

function getExpiredAt(data: PaymentQrData | null): string | undefined {
  return data?.expiredAt || data?.expiresAt;
}

function getCountdownSeconds(expiredAt: string | undefined): number | null {
  if (!expiredAt) return null;
  const expiresMs = new Date(expiredAt).getTime();
  if (Number.isNaN(expiresMs)) return null;
  return Math.max(0, Math.floor((expiresMs - Date.now()) / 1000));
}

function formatDateTime(iso: string): string {
  return new Date(iso).toLocaleString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}

function formatCountdown(secondsLeft: number): string {
  const minutes = Math.floor(secondsLeft / 60)
    .toString()
    .padStart(2, "0");
  const seconds = (secondsLeft % 60).toString().padStart(2, "0");
  return `${minutes}:${seconds}`;
}

function getEventOrderId(data: PaymentPaidEvent): string | undefined {
  return data.orderId || data.OrderId;
}

function getConnectionSnapshot(connection: HubConnection | null) {
  if (!connection) {
    return { state: "none", connectionId: null };
  }

  return {
    state: HubConnectionState[connection.state] ?? connection.state,
    connectionId: connection.connectionId ?? null,
  };
}

async function startConnectionWithRetry(
  connection: HubConnection,
  sessionId: number,
  retries = SIGNALR_START_RETRIES
) {
  let lastError: unknown;

  for (let attempt = 0; attempt < retries; attempt += 1) {
    paymentFlowLog("signalr.start.attempt", {
      sessionId,
      attempt: attempt + 1,
      maxRetries: retries,
      ...getConnectionSnapshot(connection),
    });

    try {
      await connection.start();
      paymentFlowLog("signalr.start.success", {
        sessionId,
        attempt: attempt + 1,
        ...getConnectionSnapshot(connection),
      });
      return;
    } catch (error) {
      lastError = error;
      paymentFlowError("signalr.start.failed", error, {
        sessionId,
        attempt: attempt + 1,
        maxRetries: retries,
        ...getConnectionSnapshot(connection),
      });

      if (attempt < retries - 1) {
        const delayMs = 400 * (attempt + 1);
        paymentFlowLog("signalr.start.retry_scheduled", {
          sessionId,
          nextAttempt: attempt + 2,
          delayMs,
        });
        await new Promise((resolve) => setTimeout(resolve, delayMs));
      }
    }
  }

  throw lastError;
}

function CopyRow({
  label,
  value,
  highlight = false,
  copyable = true,
  valueClassName = "text-navy",
}: {
  label: string;
  value: string;
  highlight?: boolean;
  copyable?: boolean;
  valueClassName?: string;
}) {
  const copyValue = async () => {
    if (!value) return;
    try {
      await navigator.clipboard.writeText(value);
      toast.success("Đã sao chép");
    } catch {
      toast.error("Không thể sao chép");
    }
  };

  return (
    <div
      className={`flex items-center justify-between gap-4 rounded-lg px-3 py-2.5 text-sm ${
        highlight ? "bg-primary-light/60" : ""
      }`}
    >
      <span className="flex-shrink-0 text-gray-500">{label}</span>
      <div className="flex min-w-0 items-center justify-end gap-2 text-right">
        <span className={`break-all font-semibold ${valueClassName}`}>{value}</span>
        {copyable && value && (
          <button
            type="button"
            aria-label={`Sao chép ${label}`}
            onClick={() => void copyValue()}
            className="flex h-8 w-8 flex-shrink-0 items-center justify-center rounded-lg text-primary transition hover:bg-primary-light"
          >
            <FontAwesomeIcon icon={faCopy} className="text-sm" />
          </button>
        )}
      </div>
    </div>
  );
}

export default function PaymentPage({ params }: PaymentPageProps) {
  const router = useRouter();
  const searchParams = useSearchParams();
  const orderId = params.orderId;
  const paymentProviderCode = searchParams.get("paymentProviderCode") ?? undefined;
  const isBuyNow = searchParams.get("buyNow") === "1";
  const clearCart = useCartStore((state) => state.clearCart);
  const clearBuyNowItem = useCartStore((state) => state.clearBuyNowItem);
  const connectionRef = useRef<HubConnection | null>(null);
  const paymentHandledRef = useRef(false);
  const realtimeSessionRef = useRef(0);
  const pollCountRef = useRef(0);

  const [qrData, setQrData] = useState<PaymentQrData | null>(null);
  const [isLoadingQr, setIsLoadingQr] = useState(true);
  const [qrError, setQrError] = useState("");
  const [isPaid, setIsPaid] = useState(false);
  const [paidAt, setPaidAt] = useState<string | null>(null);
  const [isCheckingPayment, setIsCheckingPayment] = useState(false);
  const [secondsLeft, setSecondsLeft] = useState(0);

  const handlePaymentSuccess = useCallback(
    (source: PaymentSuccessSource, confirmedAt?: string) => {
      if (paymentHandledRef.current) {
        paymentFlowLog("payment.success.skipped_already_handled", { orderId, source });
        return;
      }

      paymentHandledRef.current = true;
      paymentFlowLog("payment.success", { orderId, source });

      setIsPaid(true);
      setPaidAt(confirmedAt || new Date().toISOString());
      if (isBuyNow) {
        clearBuyNowItem();
      } else {
        clearCart();
      }
    },
    [clearBuyNowItem, clearCart, isBuyNow, orderId]
  );

  const checkPaymentStatus = useCallback(
    async (source: PaymentSuccessSource, context: string) => {
      if (paymentHandledRef.current) return true;

      paymentFlowLog("payment.status_check.start", { orderId, source, context });

      const status = await getPaymentStatus(orderId);
      const paid = isPaymentPaid(status);

      paymentFlowLog("payment.status_check.result", {
        orderId,
        source,
        context,
        paid,
        status,
      });

      if (paid) {
        handlePaymentSuccess(source, status.paidAt);
        return true;
      }

      return false;
    },
    [handlePaymentSuccess, orderId]
  );

  const loadPaymentQr = useCallback(async () => {
    paymentFlowLog("qr.load.start", { orderId, paymentProviderCode: paymentProviderCode ?? null });
    setIsLoadingQr(true);
    setQrError("");

    try {
      try {
        const alreadyPaid = await checkPaymentStatus("poll", "before_qr_load");
        if (alreadyPaid) return;
      } catch (statusError) {
        paymentFlowError("payment.status_check_before_qr_failed", statusError, { orderId });
      }

      const data = await createPaymentQr(orderId, paymentProviderCode);
      paymentFlowLog("qr.load.success", {
        orderId,
        paymentProviderCode: paymentProviderCode ?? null,
        hasQrImage: Boolean(getQrImageUrl(data)),
        amount: data.amount ?? null,
        orderCode: data.orderCode ?? null,
      });
      setQrData(data);
    } catch (error) {
      paymentFlowError("qr.load.failed", error, { orderId });
      try {
        const paidAfterQrError = await checkPaymentStatus("poll", "after_qr_load_failed");
        if (paidAfterQrError) return;
      } catch (statusError) {
        paymentFlowError("payment.status_check_after_qr_failed", statusError, { orderId });
      }

      setQrError(
        error instanceof OrderApiError
          ? error.message
          : "Không thể tạo mã QR thanh toán. Vui lòng thử lại."
      );
    } finally {
      setIsLoadingQr(false);
    }
  }, [checkPaymentStatus, orderId, paymentProviderCode]);

  const handleManualCheck = useCallback(async () => {
    setIsCheckingPayment(true);
    try {
      const paid = await checkPaymentStatus("poll", "manual_check");
      if (!paid) {
        toast.info("Chưa nhận được thanh toán. Vui lòng thử lại sau vài giây.");
      }
    } catch (error) {
      paymentFlowError("payment.manual_check.failed", error, { orderId });
      toast.error("Không thể kiểm tra trạng thái thanh toán.");
    } finally {
      setIsCheckingPayment(false);
    }
  }, [checkPaymentStatus, orderId]);

  useEffect(() => {
    paymentFlowLog("page.mount", {
      orderId,
      apiUrl: process.env.NEXT_PUBLIC_API_URL ?? null,
      origin: typeof window !== "undefined" ? window.location.origin : null,
    });
    void loadPaymentQr();
  }, [loadPaymentQr, orderId]);

  useEffect(() => {
    if (isPaid) return;

    const checkOnResume = () => {
      if (document.visibilityState !== "visible") return;
      void checkPaymentStatus("poll", "page_resume").catch((error) => {
        paymentFlowError("payment.status_check_on_resume_failed", error, { orderId });
      });
    };

    window.addEventListener("focus", checkOnResume);
    window.addEventListener("pageshow", checkOnResume);
    document.addEventListener("visibilitychange", checkOnResume);

    return () => {
      window.removeEventListener("focus", checkOnResume);
      window.removeEventListener("pageshow", checkOnResume);
      document.removeEventListener("visibilitychange", checkOnResume);
    };
  }, [checkPaymentStatus, isPaid, orderId]);

  useEffect(() => {
    const expiredAt = getExpiredAt(qrData);
    if (!expiredAt || isPaid) {
      setSecondsLeft(0);
      return;
    }

    const updateCountdown = () => {
      const remaining = getCountdownSeconds(expiredAt);
      setSecondsLeft(remaining ?? 0);
    };

    updateCountdown();
    const timer = window.setInterval(updateCountdown, 1000);
    return () => window.clearInterval(timer);
  }, [isPaid, qrData]);

  useEffect(() => {
    paymentHandledRef.current = false;
    pollCountRef.current = 0;
    const sessionId = ++realtimeSessionRef.current;

    paymentFlowLog("realtime.session.start", {
      sessionId,
      orderId,
      pollIntervalMs: PAYMENT_POLL_INTERVAL_MS,
      signalrRetries: SIGNALR_START_RETRIES,
    });

    let cancelled = false;

    const pollPaymentStatus = async () => {
      if (cancelled || paymentHandledRef.current) {
        paymentFlowLog("poll.skipped", {
          sessionId,
          orderId,
          cancelled,
          paymentHandled: paymentHandledRef.current,
        });
        return;
      }

      const pollNo = ++pollCountRef.current;
      paymentFlowLog("poll.start", { sessionId, orderId, pollNo });

      try {
        const status = await getPaymentStatus(orderId);
        const paid = isPaymentPaid(status);

        paymentFlowLog("poll.result", {
          sessionId,
          orderId,
          pollNo,
          paid,
          status,
        });

        if (paid) {
          handlePaymentSuccess("poll", status.paidAt);
        }
      } catch (error) {
        paymentFlowError("poll.failed", error, { sessionId, orderId, pollNo });
      }
    };

    const pollInterval = window.setInterval(() => {
      void pollPaymentStatus();
    }, PAYMENT_POLL_INTERVAL_MS);

    void pollPaymentStatus();

    const connectSignalR = async () => {
      const apiUrl = process.env.NEXT_PUBLIC_SIGNALR_URL || process.env.NEXT_PUBLIC_API_URL;
      if (!apiUrl) {
        paymentFlowLog("signalr.config.missing_api_url", { sessionId, orderId }, "warn");
        return;
      }

      const hubUrl = `${apiUrl}/hubs/payment`;
      paymentFlowLog("signalr.init", {
        sessionId,
        orderId,
        hubUrl,
        hasAccessToken: Boolean(authStorage.getToken()),
        withCredentials: true,
      });

      const connection = new HubConnectionBuilder()
        .withUrl(hubUrl, {
          accessTokenFactory: () => authStorage.getToken() ?? "",
          withCredentials: true,
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      connectionRef.current = connection;

      connection.on("PaymentPaid", (data: PaymentPaidEvent) => {
        const eventOrderId = getEventOrderId(data);
        paymentFlowLog("signalr.event.PaymentPaid", {
          sessionId,
          orderId,
          eventOrderId: eventOrderId ?? null,
          payload: data,
          orderIdMatched: Boolean(
            eventOrderId && eventOrderId.toLowerCase() === orderId.toLowerCase()
          ),
        });

        if (!eventOrderId || eventOrderId.toLowerCase() !== orderId.toLowerCase()) {
          paymentFlowLog(
            "signalr.event.PaymentPaid.ignored_order_mismatch",
            {
              sessionId,
              expectedOrderId: orderId,
              eventOrderId,
            },
            "warn"
          );
          return;
        }

        handlePaymentSuccess("signalr", data.paidAt);
      });

      connection.on("PaymentFailed", (data) => {
        paymentFlowLog(
          "signalr.event.PaymentFailed",
          {
            sessionId,
            orderId,
            payload: data,
          },
          "warn"
        );
      });

      connection.onreconnecting((error) => {
        paymentFlowLog(
          "signalr.reconnecting",
          {
            sessionId,
            orderId,
            ...getConnectionSnapshot(connection),
            error: error ? String(error) : null,
          },
          "warn"
        );
      });

      connection.onreconnected(async () => {
        paymentFlowLog("signalr.reconnected", {
          sessionId,
          orderId,
          ...getConnectionSnapshot(connection),
        });

        if (cancelled || sessionId !== realtimeSessionRef.current) {
          paymentFlowLog(
            "signalr.reconnected.skipped_stale_session",
            {
              sessionId,
              activeSessionId: realtimeSessionRef.current,
            },
            "warn"
          );
          return;
        }

        try {
          paymentFlowLog("signalr.rejoin_group.start", { sessionId, orderId });
          await connection.invoke("JoinOrderPaymentGroup", orderId);
          paymentFlowLog("signalr.rejoin_group.success", { sessionId, orderId });
        } catch (error) {
          paymentFlowError("signalr.rejoin_group.failed", error, { sessionId, orderId });
        }
      });

      connection.onclose((error) => {
        paymentFlowLog(
          "signalr.closed",
          {
            sessionId,
            orderId,
            ...getConnectionSnapshot(connection),
            error: error ? String(error) : null,
          },
          error ? "warn" : "info"
        );
      });

      await startConnectionWithRetry(connection, sessionId);

      if (cancelled || sessionId !== realtimeSessionRef.current) {
        paymentFlowLog(
          "signalr.start.aborted_stale_session",
          {
            sessionId,
            activeSessionId: realtimeSessionRef.current,
            cancelled,
          },
          "warn"
        );
        await connection.stop();
        return;
      }

      try {
        paymentFlowLog("signalr.join_group.start", {
          sessionId,
          orderId,
          ...getConnectionSnapshot(connection),
        });
        await connection.invoke("JoinOrderPaymentGroup", orderId);
        paymentFlowLog("signalr.join_group.success", {
          sessionId,
          orderId,
          ...getConnectionSnapshot(connection),
        });
      } catch (error) {
        paymentFlowError("signalr.join_group.failed", error, {
          sessionId,
          orderId,
          ...getConnectionSnapshot(connection),
        });
        throw error;
      }
    };

    connectSignalR().catch((error) => {
      paymentFlowError("signalr.setup.failed", error, { sessionId, orderId });
    });

    return () => {
      cancelled = true;
      window.clearInterval(pollInterval);

      paymentFlowLog("realtime.session.cleanup", {
        sessionId,
        orderId,
        pollCount: pollCountRef.current,
        paymentHandled: paymentHandledRef.current,
        ...getConnectionSnapshot(connectionRef.current),
      });

      const connection = connectionRef.current;
      if (!connection) return;

      if (connection.state === HubConnectionState.Connected) {
        paymentFlowLog("signalr.leave_group.start", { sessionId, orderId });
        connection.invoke("LeaveOrderPaymentGroup", orderId).catch((error) => {
          paymentFlowError("signalr.leave_group.failed", error, { sessionId, orderId });
        });
      }

      connection.stop().catch((error) => {
        paymentFlowError("signalr.stop.failed", error, { sessionId, orderId });
      });
      connectionRef.current = null;
    };
  }, [handlePaymentSuccess, orderId]);

  const qrImageUrl = getQrImageUrl(qrData);
  const orderCode = getOrderCode(qrData, orderId);
  const bankCode = getBankCode(qrData);
  const bankAccountNo = getBankAccountNo(qrData);
  const bankAccountName = getBankAccountName(qrData);
  const transferContent = getTransferContent(qrData);
  const expiredAt = getExpiredAt(qrData);
  const showCountdown = getCountdownSeconds(expiredAt) !== null && secondsLeft > 0;
  const paidAtText = paidAt ? formatDateTime(paidAt) : "";

  return (
    <section className="mx-auto max-w-5xl px-4 py-8 md:px-6 md:py-12">
      <div className="rounded-2xl bg-white p-5 shadow-card md:p-8">
        <div className="text-center">
          <span className="inline-flex items-center gap-1.5 rounded-full bg-primary-light px-3 py-1 text-xs font-semibold text-primary">
            <FontAwesomeIcon icon={faLock} className="text-[10px]" />
            Thanh toán đơn hàng
          </span>

          {isPaid ? (
            <>
              <h1 className="mt-4 text-2xl font-bold text-navy md:text-3xl">
                Thanh toán thành công
              </h1>
              <p className="mt-2 text-sm text-gray-500">
                Mã đơn hàng: <span className="font-semibold text-primary">{orderCode}</span>
              </p>
              <div className="mt-3 flex flex-wrap items-center justify-center gap-2">
                <span className="inline-flex items-center gap-1.5 rounded-full border border-green-200 bg-green-50 px-3 py-1 text-xs font-semibold text-green-700">
                  <FontAwesomeIcon icon={faCheck} className="text-[10px]" />
                  Đã thanh toán
                </span>
                <span className="inline-flex items-center gap-1.5 rounded-full border border-green-200 bg-green-50 px-3 py-1 text-xs font-semibold text-green-700">
                  Đã gửi thông tin eSim qua email
                </span>
              </div>
              {paidAtText && (
                <p className="mt-2 text-xs text-gray-400">
                  Thanh toán được xác nhận lúc {paidAtText}
                </p>
              )}
            </>
          ) : (
            <>
              <h1 className="mt-4 text-2xl font-bold text-navy md:text-3xl">
                Quét mã QR để hoàn tất thanh toán
              </h1>
              <p className="mt-2 text-sm text-gray-500">
                Mã đơn hàng: <span className="font-semibold text-primary">{orderCode}</span>
              </p>
              <div className="mt-3 flex flex-wrap items-center justify-center gap-2">
                <span className="inline-flex items-center gap-1.5 rounded-full border border-amber-200 bg-amber-50 px-3 py-1 text-xs font-semibold text-amber-700">
                  <FontAwesomeIcon icon={faClock} className="text-[10px]" />
                  Đang chờ thanh toán
                </span>
                {showCountdown && (
                  <span className="inline-flex items-center gap-1.5 rounded-full border border-primary/20 bg-primary-light px-3 py-1 text-xs font-semibold text-primary">
                    <FontAwesomeIcon icon={faClock} className="text-[10px]" />
                    Còn lại {formatCountdown(secondsLeft)}
                  </span>
                )}
              </div>
            </>
          )}
        </div>

        {isLoadingQr ? (
          <div className="mt-8 flex h-80 items-center justify-center text-gray-500">
            Đang tạo mã QR...
          </div>
        ) : qrError ? (
          <div className="mt-8 rounded-2xl border-2 border-red-300 bg-red-50 px-5 py-8 text-center shadow-sm md:px-8">
            <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-red-100 text-red-600">
              <FontAwesomeIcon icon={faExclamationTriangle} className="text-3xl" />
            </div>
            <h2 className="text-xl font-bold text-red-700">Không thể tạo mã QR thanh toán</h2>
            <p className="mx-auto mt-3 max-w-xl text-base font-medium leading-relaxed text-red-800">
              {qrError}
            </p>
            <div className="mt-6 flex flex-col items-center justify-center gap-3 sm:flex-row">
              <button
                type="button"
                className="btn btn-secondary min-w-[160px] px-6 py-3"
                onClick={() => void loadPaymentQr()}
              >
                Thử lại
              </button>
              <button
                type="button"
                className="btn btn-outline min-w-[160px] px-6 py-3"
                onClick={() => router.push("/")}
              >
                Quay về trang chủ
              </button>
            </div>
          </div>
        ) : (
          <div className="mt-8 grid grid-cols-1 gap-5 lg:grid-cols-2 lg:items-stretch">
            <div className="flex h-full flex-col rounded-xl border border-gray-200 bg-white p-5">
              {isPaid ? (
                <div className="flex flex-1 flex-col items-center justify-center py-6 text-center">
                  <div className="relative flex h-40 w-64 items-center justify-center overflow-hidden rounded-2xl bg-white">
                    <div className="absolute h-36 w-36 rounded-full bg-green-100/70 blur-xl" />
                    <span className="absolute left-10 top-6 h-5 w-5 rotate-45 bg-green-400 [clip-path:polygon(50%_0,62%_38%,100%_50%,62%_62%,50%_100%,38%_62%,0_50%,38%_38%)]" />
                    <span className="absolute right-11 top-16 h-5 w-5 rotate-45 bg-green-400 [clip-path:polygon(50%_0,62%_38%,100%_50%,62%_62%,50%_100%,38%_62%,0_50%,38%_38%)]" />
                    <span className="absolute left-7 top-20 h-3 w-1 rotate-[-20deg] rounded-full bg-blue-500" />
                    <span className="absolute left-8 bottom-11 h-3 w-1 rotate-[24deg] rounded-full bg-green-500" />
                    <span className="absolute right-9 top-8 h-3 w-1 rotate-[-28deg] rounded-full bg-amber-400" />
                    <span className="absolute right-12 bottom-10 h-3 w-1 rotate-[35deg] rounded-full bg-pink-400" />
                    <div className="relative flex h-28 w-28 items-center justify-center rounded-full bg-gradient-to-br from-emerald-300 to-green-500 text-white shadow-lg shadow-green-200">
                      <FontAwesomeIcon icon={faCheck} className="text-5xl" />
                    </div>
                  </div>
                  <p className="mt-5 text-sm text-gray-500">Số tiền đã thanh toán</p>
                  {qrData?.amount != null && (
                    <p className="mt-1 text-3xl font-bold text-green-600">
                      {formatPrice(qrData.amount)}
                    </p>
                  )}
                  <div className="mt-5 w-full border-t border-gray-100 pt-4">
                    <p className="text-sm text-gray-600">
                      Hệ thống đã được thanh toán và đã gửi eSim qua email
                    </p>
                  </div>
                  <div className="mt-4 inline-flex items-center gap-2 rounded-full bg-green-50 px-3 py-1.5 text-xs font-medium text-green-700">
                    <span className="h-2 w-2 rounded-full bg-green-500" />
                    Đã thanh toán
                  </div>
                </div>
              ) : qrImageUrl ? (
                <div className="flex flex-1 flex-col items-center justify-center">
                  <img
                    src={qrImageUrl}
                    alt="Mã QR thanh toán"
                    className="h-80 w-80 max-w-full rounded-xl bg-white object-contain"
                  />
                  <p className="mt-5 text-sm text-gray-500">Số tiền cần thanh toán</p>
                  {qrData?.amount != null && (
                    <p className="mt-1 text-3xl font-bold text-primary">
                      {formatPrice(qrData.amount)}
                    </p>
                  )}
                  <div className="mt-5 w-full border-t border-gray-100 pt-4">
                    <p className="flex items-start gap-2 text-left text-xs leading-relaxed text-gray-500">
                      <FontAwesomeIcon icon={faUniversity} className="mt-0.5 text-primary" />
                      Mở ứng dụng ngân hàng, quét QR và giữ nguyên nội dung chuyển khoản. Hệ thống
                      sẽ tự động xác nhận sau khi nhận tiền.
                    </p>
                  </div>
                </div>
              ) : (
                <div className="flex h-64 items-center justify-center text-danger">
                  API không trả về ảnh QR hợp lệ.
                </div>
              )}
            </div>

            <div className="flex h-full flex-col rounded-xl border border-gray-200 bg-white p-5">
              <h2 className="flex items-center gap-2 text-base font-semibold text-navy">
                <FontAwesomeIcon icon={faUniversity} className="text-primary" />
                {isPaid ? "Thông tin thanh toán" : "Thông tin chuyển khoản"}
              </h2>

              <div className="mt-3 flex-1 space-y-1">
                {bankCode && <CopyRow label="Ngân hàng" value={bankCode} />}
                {bankAccountNo && <CopyRow label="Số tài khoản" value={bankAccountNo} />}
                {bankAccountName && <CopyRow label="Tên tài khoản" value={bankAccountName} />}
                {transferContent && (
                  <CopyRow label="Nội dung chuyển khoản" value={transferContent} highlight />
                )}
              </div>

              {!isPaid && (
                <div className="mt-4 rounded-lg border border-amber-200 bg-amber-50 p-4">
                  <p className="flex items-start gap-2 text-sm font-semibold text-amber-800">
                    <FontAwesomeIcon icon={faExclamationTriangle} className="mt-0.5 text-amber-600" />
                    Lưu ý quan trọng
                  </p>
                  <p className="mt-2 pl-6 text-xs leading-relaxed text-amber-700">
                    Vui lòng chuyển đúng số tiền và đúng nội dung chuyển khoản như hướng dẫn để hệ
                    thống tự động đối soát và xác nhận thanh toán.
                  </p>
                </div>
              )}

              <div className="mt-auto space-y-3 pt-5">
                {isPaid ? (
                  <>
                    <button
                      type="button"
                      className="btn btn-secondary w-full py-3"
                      onClick={() => router.push(`/account/orders/${orderId}`)}
                    >
                      <FontAwesomeIcon icon={faFileAlt} />
                      Xem đơn hàng
                    </button>
                    <button
                      type="button"
                      className="btn btn-outline w-full py-3"
                      onClick={() => router.push("/esim")}
                    >
                      <FontAwesomeIcon icon={faShoppingCart} />
                      Tiếp tục mua eSIM
                    </button>
                  </>
                ) : (
                  <>
                    <button
                      type="button"
                      className="btn btn-secondary w-full py-3 disabled:opacity-60"
                      disabled={isCheckingPayment}
                      onClick={() => void handleManualCheck()}
                    >
                      <FontAwesomeIcon icon={faCheck} />
                      {isCheckingPayment ? "Đang kiểm tra..." : "Tôi đã thanh toán"}
                    </button>
                    <button
                      type="button"
                      className="btn btn-outline w-full py-3"
                    >
                      <FontAwesomeIcon icon={faHeadset} />
                      Cần hỗ trợ
                    </button>
                  </>
                )}
              </div>
            </div>
          </div>
        )}

        <p className="mt-6 flex items-center justify-center gap-2 text-center text-xs text-gray-500">
          <FontAwesomeIcon icon={faCheckCircle} className="text-primary" />
          {isPaid
            ? "Đơn hàng của bạn sẽ được tiếp tục xử lý và gửi eSIM ngay sau bước xác nhận."
            : "Sau khi thanh toán thành công, hệ thống sẽ tự động cập nhật trạng thái và xử lý đơn hàng eSIM."}
        </p>
      </div>
    </section>
  );
}
