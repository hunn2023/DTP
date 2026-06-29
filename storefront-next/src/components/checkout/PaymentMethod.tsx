"use client";

import { useEffect, useMemo, useState } from "react";
import { UseFormRegister, UseFormSetValue } from "react-hook-form";
import { CheckoutFormData } from "@/lib/schemas/checkoutSchema";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCheck,
  faCopy,
  faCreditCard,
  faQrcode,
  faStar,
  faUniversity,
} from "@fortawesome/free-solid-svg-icons";
import type { Language } from "@/lib/i18n";
import { toast } from "sonner";
import type { PaymentQrData } from "@/lib/orderApi";
import {
  isProviderAmountEligible,
  resolvePaymentProviderLogoUrl,
  type PaymentProvider,
} from "@/lib/api/paymentProviderApi";

interface Props {
  register: UseFormRegister<CheckoutFormData>;
  setValue: UseFormSetValue<CheckoutFormData>;
  selectedMethod: string;
  providers: PaymentProvider[];
  isLoading?: boolean;
  loadError?: string | null;
  onRetry?: () => void;
  language?: Language;
  showDetails?: boolean;
  amount?: number;
  paymentQrData?: PaymentQrData | null;
  orderId?: string | null;
  onPaymentConfirmed?: () => void;
}

function formatAmount(amount: number) {
  return `${amount.toLocaleString("vi-VN")}đ`;
}

export default function PaymentMethod({
  register,
  setValue,
  selectedMethod,
  providers,
  isLoading = false,
  loadError = null,
  onRetry,
  language = "vi",
  showDetails = true,
  amount = 0,
  paymentQrData,
}: Props) {
  const [secondsLeft, setSecondsLeft] = useState(600);

  const text = {
    title: language === "vi" ? "Phương thức thanh toán" : "Payment method",
    paymentPageTitle: language === "vi" ? "Thanh toán" : "Payment",
    subtitle:
      language === "vi"
        ? "Chọn cổng thanh toán phù hợp để hoàn tất đơn hàng"
        : "Choose a payment gateway to complete your order",
    recommended: language === "vi" ? "Đề xuất" : "Recommended",
    loading: language === "vi" ? "Đang tải phương thức thanh toán..." : "Loading payment methods...",
    retry: language === "vi" ? "Thử lại" : "Retry",
    noProviders:
      language === "vi"
        ? "Hiện chưa có phương thức thanh toán khả dụng."
        : "No payment methods are currently available.",
    amountOutOfRange:
      language === "vi"
        ? "Không áp dụng cho đơn hàng này"
        : "Not available for this order amount",
    minAmount: language === "vi" ? "Tối thiểu" : "Min",
    maxAmount: language === "vi" ? "Tối đa" : "Max",
    bankDesc:
      language === "vi"
        ? "Vui lòng quét mã QR và hoàn tất chuyển khoản để xử lý đơn hàng"
        : "Please scan the QR code and complete transfer to process your order",
    transferInfo: language === "vi" ? "Thông tin chuyển khoản:" : "Bank transfer details:",
    bank: language === "vi" ? "Ngân hàng" : "Bank",
    accountNumber: language === "vi" ? "Số TK" : "Account No.",
    accountName: language === "vi" ? "Chủ TK" : "Account name",
    amountLabel: language === "vi" ? "Số tiền" : "Amount",
    qrPlaceholder: language === "vi" ? "Mã QR thanh toán" : "Payment QR code",
    maxTime: language === "vi" ? "Thời gian thanh toán tối đa" : "Maximum payment time",
    expired:
      language === "vi"
        ? "Đã hết thời gian thanh toán. Vui lòng tạo lại giao dịch."
        : "Payment time expired. Please create a new transaction.",
    copied: language === "vi" ? "Đã sao chép" : "Copied",
    copyFailed: language === "vi" ? "Không thể sao chép" : "Failed to copy",
    copyAction: language === "vi" ? "Sao chép" : "Copy",
    transferNote:
      language === "vi"
        ? "Sau khi chuyển khoản, hệ thống sẽ xác nhận tự động. Nếu cần hỗ trợ, vui lòng liên hệ CSKH."
        : "After transfer, the system will auto-verify. If needed, please contact customer support.",
  };

  const selectedProvider = useMemo(
    () => providers.find((provider) => provider.code === selectedMethod) ?? null,
    [providers, selectedMethod]
  );

  useEffect(() => {
    if (!showDetails) return;

    setSecondsLeft(600);
    const timer = setInterval(() => {
      setSecondsLeft((prev) => (prev <= 1 ? 0 : prev - 1));
    }, 1000);

    return () => clearInterval(timer);
  }, [showDetails]);

  const countdownText = useMemo(() => {
    const minutes = Math.floor(secondsLeft / 60)
      .toString()
      .padStart(2, "0");
    const seconds = (secondsLeft % 60).toString().padStart(2, "0");
    return `${minutes}:${seconds}`;
  }, [secondsLeft]);

  const copyValue = async (value: string) => {
    try {
      await navigator.clipboard.writeText(value);
      toast.success(text.copied);
    } catch {
      toast.error(text.copyFailed);
    }
  };

  const handleSelectProvider = (code: string) => {
    setValue("paymentMethod", code, { shouldDirty: true, shouldValidate: true });
  };

  const accountName = paymentQrData?.accountName || "CONG TY EZSIM VIET NAM";
  const accountNumber = paymentQrData?.accountNumber || "";
  const bankName = paymentQrData?.bankName || "Vietcombank";
  const transferContent = paymentQrData?.content || "";
  const qrImageUrl = paymentQrData?.qrCodeUrl || paymentQrData?.qrDataUrl || "";
  const amountText = formatAmount(paymentQrData?.amount || amount);

  return (
    <div className="space-y-4">
      <div>
        <h3 className="text-lg font-semibold text-navy">
          {showDetails ? text.paymentPageTitle : text.title}
        </h3>
        {!showDetails && (
          <p className="text-sm text-gray-500 mt-1">{text.subtitle}</p>
        )}
      </div>

      <input {...register("paymentMethod")} type="hidden" />

      {isLoading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          {[1, 2].map((item) => (
            <div
              key={item}
              className="h-28 rounded-xl border border-gray-200 bg-gray-50 animate-pulse"
            />
          ))}
          <p className="text-sm text-gray-500 sm:col-span-2">{text.loading}</p>
        </div>
      ) : loadError ? (
        <div className="rounded-xl border border-danger/20 bg-danger-light/40 p-4 text-sm text-gray-700">
          <p>{loadError}</p>
          {onRetry && (
            <button
              type="button"
              onClick={onRetry}
              className="mt-3 text-primary font-semibold hover:underline"
            >
              {text.retry}
            </button>
          )}
        </div>
      ) : providers.length === 0 ? (
        <div className="rounded-xl border border-gray-200 bg-gray-50 p-4 text-sm text-gray-600">
          {text.noProviders}
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          {providers.map((provider) => {
            const isSelected = selectedMethod === provider.code;
            const isEligible = isProviderAmountEligible(provider, amount);
            const logoUrl = resolvePaymentProviderLogoUrl(provider.logoUrl);

            return (
              <button
                key={provider.code}
                type="button"
                disabled={!isEligible}
                onClick={() => handleSelectProvider(provider.code)}
                className={`relative text-left rounded-xl border p-4 transition-all ${
                  isSelected
                    ? "border-primary bg-primary-light/50 shadow-card ring-1 ring-primary/20"
                    : "border-gray-200 bg-white hover:border-primary/30 hover:bg-primary-light/20"
                } ${!isEligible ? "opacity-50 cursor-not-allowed" : "cursor-pointer"}`}
                aria-pressed={isSelected}
              >
                <div className="flex items-start gap-3">
                  <div
                    className={`w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 overflow-hidden ${
                      isSelected ? "bg-white border border-primary/20" : "bg-gray-50 border border-gray-200"
                    }`}
                  >
                    {logoUrl ? (
                      // eslint-disable-next-line @next/next/no-img-element
                      <img
                        src={logoUrl}
                        alt={provider.name}
                        className="w-10 h-10 object-contain"
                      />
                    ) : (
                      <FontAwesomeIcon icon={faCreditCard} className="text-primary text-lg" />
                    )}
                  </div>

                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 flex-wrap">
                      <p className="font-semibold text-navy">{provider.name}</p>
                      {provider.isDefault && (
                        <span className="inline-flex items-center gap-1 text-[11px] font-semibold px-2 py-0.5 rounded-full bg-primary text-white">
                          <FontAwesomeIcon icon={faStar} className="text-[10px]" />
                          {text.recommended}
                        </span>
                      )}
                    </div>
                    {provider.description && (
                      <p className="text-sm text-gray-600 mt-1 line-clamp-2">{provider.description}</p>
                    )}
                    {!isEligible && amount > 0 && (
                      <p className="text-xs text-danger mt-2">
                        {text.amountOutOfRange} ({text.minAmount}: {formatAmount(provider.minAmount)}
                        {provider.maxAmount < Number.MAX_SAFE_INTEGER && (
                          <>, {text.maxAmount}: {formatAmount(provider.maxAmount)}</>
                        )}
                        )
                      </p>
                    )}
                  </div>

                  <div
                    className={`w-6 h-6 rounded-full border flex items-center justify-center flex-shrink-0 ${
                      isSelected ? "border-primary bg-primary text-white" : "border-gray-300 bg-white"
                    }`}
                  >
                    {isSelected && <FontAwesomeIcon icon={faCheck} className="text-xs" />}
                  </div>
                </div>
              </button>
            );
          })}
        </div>
      )}

      {showDetails && selectedProvider && (
        <div className="rounded-xl border border-primary/20 bg-primary-light/40 p-4">
          <div className="flex items-start gap-3">
            <div className="w-10 h-10 rounded-full bg-primary text-white flex items-center justify-center flex-shrink-0">
              <FontAwesomeIcon icon={faCreditCard} />
            </div>
            <div className="flex-1">
              <p className="font-semibold text-navy">{selectedProvider.name}</p>
              <p className="text-sm text-gray-600 mt-1">
                {selectedProvider.description || text.bankDesc}
              </p>
            </div>
            <span className="text-xs font-semibold px-2 py-1 rounded-full bg-primary text-white">
              {selectedProvider.code}
            </span>
          </div>
        </div>
      )}

      {showDetails && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mt-4 p-4 bg-gray-50 rounded-lg text-sm text-gray-500">
          <div className="md:col-span-2 rounded-lg border border-dashed border-primary/40 bg-white p-5 text-center flex flex-col items-center justify-center min-h-[400px]">
            {qrImageUrl ? (
              // eslint-disable-next-line @next/next/no-img-element
              <img
                src={qrImageUrl}
                alt={text.qrPlaceholder}
                className="w-[300px] h-[300px] max-w-full rounded-2xl border border-primary/20 object-contain"
              />
            ) : (
              <div className="w-[300px] h-[300px] max-w-full rounded-2xl border border-primary/20 bg-primary-light/30 flex flex-col items-center justify-center">
                <FontAwesomeIcon icon={faQrcode} className="text-8xl text-primary mb-3" />
                <p className="text-sm text-gray-500">{text.qrPlaceholder}</p>
              </div>
            )}
            <div className="mt-3 inline-flex items-center gap-1 text-[11px] text-gray-400">
              <FontAwesomeIcon icon={faUniversity} />
              <span>{bankName}</span>
            </div>
          </div>

          <div className="md:col-span-1">
            <p className="font-medium text-navy mb-2">{text.transferInfo}</p>

            <div className="space-y-2">
              <div className="flex items-center justify-between gap-2">
                <span>
                  {text.bank}: <span className="font-semibold">{bankName}</span>
                </span>
              </div>

              <div className="flex items-center justify-between gap-2">
                <span>
                  {text.accountNumber}: <span className="font-semibold">{accountNumber}</span>
                </span>
                <button
                  type="button"
                  title={text.copyAction}
                  aria-label={`${text.copyAction} ${text.accountNumber}`}
                  onClick={() => copyValue(accountNumber)}
                  className="text-primary text-sm font-semibold inline-flex items-center hover:opacity-80"
                >
                  <FontAwesomeIcon icon={faCopy} />
                </button>
              </div>

              <div className="flex items-center justify-between gap-2">
                <span>
                  {text.accountName}: <span className="font-semibold">{accountName}</span>
                </span>
                <button
                  type="button"
                  title={text.copyAction}
                  aria-label={`${text.copyAction} ${text.accountName}`}
                  onClick={() => copyValue(accountName)}
                  className="text-primary text-sm font-semibold inline-flex items-center hover:opacity-80"
                >
                  <FontAwesomeIcon icon={faCopy} />
                </button>
              </div>

              <div className="flex items-center justify-between gap-2">
                <span>
                  {text.amountLabel}: <span className="font-semibold text-primary">{amountText}</span>
                </span>
                <button
                  type="button"
                  title={text.copyAction}
                  aria-label={`${text.copyAction} ${text.amountLabel}`}
                  onClick={() => copyValue((paymentQrData?.amount || amount).toString())}
                  className="text-primary text-sm font-semibold inline-flex items-center hover:opacity-80"
                >
                  <FontAwesomeIcon icon={faCopy} />
                </button>
              </div>

              {transferContent && (
                <div className="flex items-center justify-between gap-2">
                  <span>
                    {language === "vi" ? "Nội dung CK" : "Transfer note"}:{" "}
                    <span className="font-semibold text-xs break-all">{transferContent}</span>
                  </span>
                  <button
                    type="button"
                    title={text.copyAction}
                    onClick={() => copyValue(transferContent)}
                    className="text-primary text-sm font-semibold inline-flex items-center hover:opacity-80"
                  >
                    <FontAwesomeIcon icon={faCopy} />
                  </button>
                </div>
              )}
            </div>

            <div className="mt-3 rounded-lg border border-primary/20 bg-white px-3 py-2">
              <p className="text-xs text-gray-500">{text.maxTime}</p>
              <p className={`text-sm font-bold ${secondsLeft === 0 ? "text-danger" : "text-primary"}`}>
                {secondsLeft === 0 ? text.expired : countdownText}
              </p>
            </div>

            <p className="mt-2 text-xs">{text.transferNote}</p>
          </div>
        </div>
      )}
    </div>
  );
}
