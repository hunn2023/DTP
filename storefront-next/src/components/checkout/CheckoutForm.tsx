"use client";

import { useState, useCallback, useEffect, useMemo, useRef } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { toast } from "sonner";

import { getCheckoutSchema, CheckoutFormData } from "@/lib/schemas/checkoutSchema";
import { createOrder, confirmOrder, mapFormDataToPayload, OrderApiError, OrderItem, PaymentQrData } from "@/lib/orderApi";
import { useCartStore } from "@/lib/cartStore";
import { formatPrice } from "@/lib/product";
import type { CartItem } from "@/types/cart";
import { useLanguage } from "@/hooks/useLanguage";
import { useAuth } from "@/hooks/useAuth";
import { usePaymentProviders } from "@/hooks/usePaymentProviders";
import { isProviderAmountEligible } from "@/lib/api/paymentProviderApi";
import Icon from "@/components/ui/Icon";

import ShippingInfoForm from "./ShippingInfoForm";
import PaymentMethod from "./PaymentMethod";
import OrderReview from "./OrderReview";

interface CheckoutFormProps {
  checkoutItems?: CartItem[];
  isBuyNow?: boolean;
}

export default function CheckoutForm({ checkoutItems, isBuyNow = false }: CheckoutFormProps) {
  const router = useRouter();
  const { language } = useLanguage();
  const { user, isAuthenticated } = useAuth();
  const { providers, isLoading: isProvidersLoading, error: providersError, reload: reloadProviders } =
    usePaymentProviders();
  const items = useCartStore((s) => s.items);
  const clearCart = useCartStore((s) => s.clearCart);
  const clearBuyNowItem = useCartStore((s) => s.clearBuyNowItem);
  const activeItems = checkoutItems ?? items;
  const orderTotal = useMemo(
    () => activeItems.reduce((sum, item) => sum + item.price * item.quantity, 0),
    [activeItems]
  );
  const prefilledRef = useRef(false);

  const text = useMemo(() => ({
    cartEmpty:
      language === "vi"
        ? "Giỏ hàng trống. Vui lòng thêm sản phẩm trước."
        : "Your cart is empty. Please add products first.",
    success: language === "vi" ? "Đặt hàng thành công!" : "Order placed successfully!",
    failed: language === "vi" ? "Đặt hàng thất bại. Vui lòng thử lại." : "Order failed. Please try again.",
    timeout: language === "vi" ? "Yêu cầu bị timeout. Vui lòng thử lại." : "Request timed out. Please try again.",
    serverError:
      language === "vi"
        ? "Server đang gặp sự cố. Vui lòng thử lại sau."
        : "Server is experiencing issues. Please try again later.",
    unknownError:
      language === "vi"
        ? "Đã xảy ra lỗi không xác định. Vui lòng thử lại."
        : "An unknown error occurred. Please try again.",
    shippingInfo: language === "vi" ? "Thông tin giao hàng" : "Shipping information",
    payment: language === "vi" ? "Thanh toán" : "Payment",
    totalPayment: language === "vi" ? "Tổng thanh toán" : "Total payment",
    processing: language === "vi" ? "Đang xử lý..." : "Processing...",
    placeOrder: language === "vi" ? "Đặt hàng" : "Place order",
    bankTransfer: language === "vi" ? "(QR Banking)" : "(QR Banking)",
    continueToPayment: language === "vi" ? "Tiếp tục sang Thanh toán" : "Continue to Payment",
    backToShipping: language === "vi" ? "← Quay lại Shipping information" : "← Back to Shipping information",
    backToCart: language === "vi" ? "← Quay lại giỏ hàng" : "← Back to cart",
    accountPrefillTitle:
      language === "vi"
        ? "Đã tự động điền từ tài khoản"
        : "Auto-filled from your account",
    accountPrefillDesc:
      language === "vi"
        ? "Thông tin email, số điện thoại và tên đã được đồng bộ. Bạn vẫn có thể chỉnh sửa trước khi đặt hàng."
        : "Email, phone, and name have been synced. You can still edit before placing the order.",
    invoiceRequest:
      language === "vi"
        ? "Tôi muốn xuất hóa đơn VAT"
        : "I want to request a VAT invoice",
    invoiceSupport:
      language === "vi"
        ? "Nếu cần xuất hóa đơn, vui lòng liên hệ CSKH để được hướng dẫn nhanh."
        : "If you need an invoice, please contact customer support for quick guidance.",
    termsAgreementPrefix:
      language === "vi"
        ? "Tôi đã đọc và đồng ý với"
        : "I have read and agree to the",
    termsAgreementLink:
      language === "vi" ? "Điều khoản dịch vụ" : "Terms of Service",
    termsPolicyTitle:
      language === "vi" ? "Chính sách điều khoản" : "Terms policy",
    termsPolicyBody:
      language === "vi"
        ? "Bạn cần đọc kỹ điều khoản về thanh toán, hoàn tiền, và trách nhiệm sử dụng dịch vụ trước khi hoàn tất đơn hàng."
        : "Please review terms on payment, refunds, and service responsibilities before completing the order.",
    termsPanelTitle:
      language === "vi" ? "Nội dung Điều khoản dịch vụ" : "Terms of Service content",
    needAgreeTermsFirst:
      language === "vi"
        ? "Vui lòng tích Điều khoản dịch vụ trước khi sang bước thanh toán."
        : "Please agree to the Terms of Service before continuing to payment.",
    selectPaymentMethod:
      language === "vi"
        ? "Vui lòng chọn phương thức thanh toán."
        : "Please select a payment method.",
    deviceModalTitle:
      language === "vi" ? "Danh sách thiết bị hỗ trợ eSIM" : "eSIM supported devices",
    deviceModalDesc:
      language === "vi"
        ? "Vui lòng đọc danh sách dưới đây và xác nhận bạn đã kiểm tra thiết bị trước khi tiếp tục."
        : "Please review the list below and confirm you have checked your device before continuing.",
    deviceConfirm:
      language === "vi"
        ? "Xác nhận thiết bị tương thích"
        : "I have read and confirmed device compatibility",
    close: language === "vi" ? "Đóng" : "Close",
    support: language === "vi" ? "Liên hệ CSKH" : "Contact support",
  }), [language]);

  const checkoutSchema = useMemo(() => getCheckoutSchema(language), [language]);

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showDeviceModal, setShowDeviceModal] = useState(false);
  const [currentStep, setCurrentStep] = useState<1 | 2>(1);
  const [createdOrderId] = useState<string | null>(null);
  const [paymentQrData] = useState<PaymentQrData | null>(null);
  const paymentPollRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    watch,
    reset,
    getValues,
    trigger,
  } = useForm<CheckoutFormData>({
    resolver: zodResolver(checkoutSchema),
    defaultValues: {
      fullName: "",
      phone: "",
      email: "",
      orderNote: "",
      paymentMethod: "",
      requestInvoice: false,
      agreeTerms: false,
      confirmDeviceSupport: false,
    },
    mode: "onBlur",
  });

  const selectedPaymentMethod = watch("paymentMethod") ?? "";
  const requestInvoice = watch("requestInvoice");
  const agreeTerms = watch("agreeTerms");

  useEffect(() => {
    if (providers.length === 0) return;

    const current = getValues("paymentMethod");
    const currentProvider = providers.find((provider) => provider.code === current);
    const isCurrentValid =
      currentProvider && isProviderAmountEligible(currentProvider, orderTotal);

    if (isCurrentValid) return;

    const eligibleProviders = providers.filter((provider) =>
      isProviderAmountEligible(provider, orderTotal)
    );
    const defaultProvider =
      eligibleProviders.find((provider) => provider.isDefault) ?? eligibleProviders[0];

    if (defaultProvider) {
      setValue("paymentMethod", defaultProvider.code, { shouldDirty: false, shouldValidate: true });
    }
  }, [getValues, orderTotal, providers, setValue]);

  useEffect(() => {
    if (!isAuthenticated || !user || prefilledRef.current) return;

    const current = getValues();
    setValue("fullName", current.fullName || user.name || "", { shouldDirty: false });
    setValue("phone", current.phone || user.phone || "", { shouldDirty: false });
    setValue("email", current.email || user.email || "", { shouldDirty: false });
    prefilledRef.current = true;
  }, [getValues, isAuthenticated, setValue, user]);

  // Cleanup: Reset form if items become empty
  useEffect(() => {
    if (activeItems.length === 0) {
      reset();
    }
  }, [activeItems.length, reset]);

  const onSubmit = useCallback(
    async (formData: CheckoutFormData) => {
      if (isSubmitting) return;

      // Create and confirm the order before opening its payment page.
      if (!formData.agreeTerms) {
        toast.error(text.needAgreeTermsFirst);
        return;
      }

      if (!formData.paymentMethod) {
        toast.error(text.selectPaymentMethod);
        return;
      }

      const selectedProvider = providers.find((provider) => provider.code === formData.paymentMethod);
      if (!selectedProvider || !isProviderAmountEligible(selectedProvider, orderTotal)) {
        toast.error(text.selectPaymentMethod);
        return;
      }

      if (!formData.confirmDeviceSupport) {
        setShowDeviceModal(true);
        return;
      }

      if (activeItems.length === 0) {
        toast.error(text.cartEmpty);
        router.push("/");
        return;
      }

      setIsSubmitting(true);

        const orderItems: OrderItem[] = activeItems.map((item) => ({
          id: item.id,
          name: item.name,
          price: item.price,
          quantity: item.quantity,
          image: item.image,
          productId: item.productId,
          productVariantId: item.productVariantId,
          esimPackageId: item.esimPackageId,
          phoneCardId: item.phoneCardId,
          itemType: item.itemType,
        }));

        const payload = mapFormDataToPayload(formData, orderItems, orderTotal);
        console.log("[Checkout] createOrder payload:", JSON.stringify(payload, null, 2));

        try {
          // 1. Create order
          const result = await createOrder(payload);
          console.log("[Checkout] createOrder result:", result);
          if (!result.success || !result.orderId) {
            toast.error(result.message || text.failed);
            return;
          }

          const orderId = result.orderId;
          await confirmOrder(orderId, selectedProvider.paymentMethod);
          toast.success(language === "vi" ? "Xác nhận đơn hàng thành công." : "Order confirmed successfully.");
          router.push(
            `/checkout/payment/${orderId}?paymentProviderCode=${encodeURIComponent(selectedProvider.code)}${isBuyNow ? "&buyNow=1" : ""}`
          );
        } catch (error) {
          console.error("[Checkout] createOrder error:", error);
          if (error instanceof OrderApiError) {
            if (error.statusCode === 408) {
              toast.error(text.timeout);
            } else if (error.statusCode >= 500) {
              toast.error(text.serverError);
            } else {
              toast.error(error.message);
            }
          } else if (error instanceof Error) {
            toast.error(error.message);
          } else {
            toast.error(text.unknownError);
          }
        } finally {
          setIsSubmitting(false);
        }
      return;

    },
    [isSubmitting, activeItems, orderTotal, router, text, language, providers, isBuyNow]
  );

  const handleContinueToPayment = useCallback(async () => {
    const isValid = await trigger([
      "fullName",
      "phone",
      "email",
      "orderNote",
      "paymentMethod",
      "agreeTerms",
    ]);
    if (!isValid) return;

    if (!getValues("agreeTerms")) {
      toast.error(text.needAgreeTermsFirst);
      return;
    }

    if (!getValues("paymentMethod")) {
      toast.error(text.selectPaymentMethod);
      return;
    }

    setShowDeviceModal(true);
  }, [getValues, text.needAgreeTermsFirst, text.selectPaymentMethod, trigger]);

  const handleDeviceConfirm = useCallback(async () => {
    setShowDeviceModal(false);
    const formData = { ...getValues(), confirmDeviceSupport: true };
    setValue("confirmDeviceSupport", true, { shouldDirty: true });
    await onSubmit(formData);
  }, [getValues, onSubmit, setValue]);

  if (activeItems.length === 0) {
    return null;
  }

  return (
    <form onSubmit={handleSubmit(onSubmit, (validationErrors) => {
      console.error("[Checkout] Form validation errors:", validationErrors);
      // Show first validation error as toast so user gets immediate feedback
      const firstError = Object.values(validationErrors)[0];
      if (firstError?.message) {
        toast.error(firstError.message as string);
      }
    })} className="space-y-6">
      {/* Main Grid Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left Column: Form Fields */}
        <div className="lg:col-span-2 space-y-6">
          {currentStep === 1 ? (
            <>
              {/* Shipping Info Section */}
              <section className="bg-white rounded-xl p-6 shadow-card">
                <ShippingInfoForm
                  register={register}
                  errors={errors}
                  language={language}
                />

                <div className="rounded-xl border border-gray-200 p-4 space-y-3">
                  <label className="flex items-start gap-2 cursor-pointer">
                    <input type="checkbox" {...register("requestInvoice")} className="mt-1" />
                    <span className="text-sm text-gray-700">{text.invoiceRequest}</span>
                  </label>
                  {requestInvoice && (
                    <p className="text-sm text-gray-600 bg-gray-50 rounded-lg px-3 py-2">
                      {text.invoiceSupport}{" "}
                      <a href="/support" className="text-primary font-semibold hover:underline">
                        {text.support}
                      </a>
                    </p>
                  )}
                </div>

                <div className="space-y-3 mt-4">
                  <label className="flex items-start gap-2 text-sm cursor-pointer">
                    <input type="checkbox" {...register("agreeTerms")} className="mt-1" />
                    <span>
                      {text.termsAgreementPrefix} <strong className="text-primary"> {text.termsAgreementLink}</strong>
                    </span>
                  </label>
                  {errors.agreeTerms && <p className="text-danger text-xs">{errors.agreeTerms.message}</p>}

                  {agreeTerms && (
                    <>
                      <div className="rounded-lg border border-gray-200 bg-gray-50 p-3">
                        <p className="font-semibold text-navy text-sm mb-2">{text.termsPanelTitle}</p>
                        <div className="h-44 overflow-y-auto rounded-md bg-white border border-gray-200 p-3 text-sm text-gray-600 leading-6">
                          {language === "vi" ? (
                            <>
                              <p>1. Đây là nội dung điều khoản dịch vụ dạng mock để tích hợp giao diện trong giai đoạn chưa có API.</p>
                              <p>2. Khách hàng cần kiểm tra kỹ thông tin sản phẩm, thời hạn gói và khu vực áp dụng trước khi thanh toán.</p>
                              <p>3. Đơn hàng được xử lý ngay sau khi hệ thống ghi nhận thanh toán hợp lệ.</p>
                              <p>4. Trường hợp thông tin thiết bị không tương thích eSIM, khách hàng cần liên hệ CSKH để được hỗ trợ.</p>
                              <p>5. Chính sách hoàn tiền áp dụng theo điều kiện của từng loại sản phẩm và thời điểm sử dụng.</p>
                              <p>6. Khách hàng chịu trách nhiệm đảm bảo thiết bị đã mở khóa mạng và hỗ trợ eSIM.</p>
                              <p>7. EZSIM có quyền từ chối giao dịch có dấu hiệu gian lận hoặc thông tin không chính xác.</p>
                              <p>8. Dữ liệu cá nhân được sử dụng để xử lý đơn hàng và chăm sóc khách hàng theo chính sách bảo mật.</p>
                              <p>9. Trong trường hợp phát sinh tranh chấp, hai bên ưu tiên giải quyết bằng thương lượng.</p>
                              <p>10. Nội dung điều khoản này sẽ được thay thế bằng dữ liệu chính thức từ API trong phiên bản tiếp theo.</p>
                            </>
                          ) : (
                            <>
                              <p>1. This is a mock Terms of Service block for UI integration before the API is available.</p>
                              <p>2. Customers should verify product details, validity, and coverage before payment.</p>
                              <p>3. Orders are processed immediately after successful payment verification.</p>
                              <p>4. If a device is not eSIM-compatible, please contact customer support for assistance.</p>
                              <p>5. Refund policy depends on each product type and usage status.</p>
                              <p>6. Customers are responsible for ensuring their devices are network-unlocked and eSIM-ready.</p>
                              <p>7. EZSIM reserves the right to reject suspicious or inaccurate transactions.</p>
                              <p>8. Personal data is used for order processing and customer support under privacy rules.</p>
                              <p>9. In case of disputes, both parties should prioritize negotiation.</p>
                              <p>10. This mock content will be replaced by official API data in a future release.</p>
                            </>
                          )}
                        </div>
                      </div>

                      <div className="rounded-lg bg-gray-50 border border-gray-200 p-3 text-sm text-gray-600">
                        <p className="font-semibold text-navy mb-1">{text.termsPolicyTitle}</p>
                        <p>{text.termsPolicyBody}</p>
                      </div>
                    </>
                  )}
                </div>
              </section>

              {/* Payment Method Section */}
              <section className="bg-white rounded-xl p-6 shadow-card">
                <PaymentMethod
                  register={register}
                  setValue={setValue}
                  selectedMethod={selectedPaymentMethod}
                  providers={providers}
                  isLoading={isProvidersLoading}
                  loadError={providersError}
                  onRetry={() => void reloadProviders()}
                  language={language}
                  showDetails={false}
                  amount={orderTotal}
                />
                {errors.paymentMethod && (
                  <p className="text-danger text-xs mt-2">{errors.paymentMethod.message}</p>
                )}
              </section>
            </>
          ) : (
            <section className="bg-white rounded-xl p-6 shadow-card">
              <PaymentMethod
                register={register}
                setValue={setValue}
                selectedMethod={selectedPaymentMethod}
                providers={providers}
                isLoading={isProvidersLoading}
                loadError={providersError}
                onRetry={() => void reloadProviders()}
                language={language}
                showDetails
                amount={orderTotal}
                paymentQrData={paymentQrData}
                orderId={createdOrderId}
                onPaymentConfirmed={() => {
                  toast.success(text.success);
                  if (isBuyNow) {
                    clearBuyNowItem();
                  } else {
                    clearCart();
                  }
                  router.push("/account/orders");
                }}
              />

              {/* Confirm payment button */}
              <div className="mt-6 pt-4 border-t">
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className={`w-full py-4 rounded-xl font-semibold text-white transition ${
                    isSubmitting
                      ? "bg-gray-400 cursor-not-allowed"
                      : "bg-primary hover:bg-primary-dark active:scale-[0.98] shadow-btn"
                  }`}
                >
                  {isSubmitting
                    ? (language === "vi" ? "Đang kiểm tra..." : "Checking...")
                    : (language === "vi" ? "Tôi đã thanh toán" : "I have paid")}
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setCurrentStep(1);
                    if (paymentPollRef.current) {
                      clearInterval(paymentPollRef.current);
                      paymentPollRef.current = null;
                    }
                  }}
                  className="w-full mt-2 py-2 text-sm text-gray-500 hover:text-primary transition text-center"
                >
                  {text.backToShipping}
                </button>
              </div>
            </section>
          )}
        </div>

        {/* Right Column: Order Review (only shipping step) */}
        {currentStep === 1 && (
          <div className="lg:col-span-1">
            <section className="bg-white rounded-xl p-6 shadow-card sticky top-24">
              <OrderReview items={activeItems} shippingFee={0} language={language} />

              <div className="mt-6 pt-4 border-t">
                <div className="flex justify-between items-center mb-4">
                  <span className="font-semibold text-navy">{text.totalPayment}</span>
                  <span className="text-2xl font-bold text-primary">
                    {formatPrice(orderTotal)}
                  </span>
                </div>

                {/* Submit Button */}
                <button
                  type="button"
                  onClick={() => void handleContinueToPayment()}
                  disabled={
                    isSubmitting ||
                    isProvidersLoading ||
                    providers.length === 0 ||
                    !selectedPaymentMethod
                  }
                  className={`w-full py-4 rounded-xl font-semibold text-white transition ${
                    isSubmitting
                      ? "bg-gray-400 cursor-not-allowed"
                      : "bg-primary hover:bg-primary-dark active:scale-[0.98] shadow-btn"
                  }`}
                  aria-busy={isSubmitting}
                >
                  {isSubmitting ? (
                    <span className="flex items-center justify-center gap-2">
                      <svg
                        className="animate-spin h-5 w-5 text-white"
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                      >
                        <circle
                          className="opacity-25"
                          cx="12"
                          cy="12"
                          r="10"
                          stroke="currentColor"
                          strokeWidth="4"
                        />
                        <path
                          className="opacity-75"
                          fill="currentColor"
                          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                        />
                      </svg>
                      <span>{text.processing}</span>
                    </span>
                  ) : (
                    text.continueToPayment
                  )}
                </button>

                {/* Back to cart link */}
                <button
                  type="button"
                  onClick={() => router.push("/cart")}
                  className="w-full mt-2 py-2 text-sm text-gray-500 hover:text-primary transition text-center"
                >
                  {text.backToCart}
                </button>
              </div>
            </section>
          </div>
        )}
      </div>

      {showDeviceModal && (
        <div className="fixed inset-0 z-[240] bg-black/50 flex items-center justify-center p-4" role="dialog" aria-modal="true" aria-label={text.deviceModalTitle}>
          <div className="w-full max-w-2xl bg-white rounded-2xl shadow-xl border border-gray-200 overflow-hidden">
            <div className="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
              <h3 className="font-bold text-navy text-base">{text.deviceModalTitle}</h3>
              <button type="button" onClick={() => setShowDeviceModal(false)} className="w-8 h-8 flex items-center justify-center rounded-lg hover:bg-gray-100">
                <Icon icon="times" />
              </button>
            </div>

            <div className="popup-esim-compatible-devices device-compatibility p-5 space-y-5 max-h-[72vh] overflow-y-auto">
              {language === "vi" ? (
                <>
                  <div className="rounded-lg border border-primary/15 bg-primary-light/40 p-3">
                    <p className="font-semibold text-navy">Các thiết bị sau có hỗ trợ eSIM:</p>
                    <p className="text-sm text-gray-600 mt-1">
                      Để sử dụng eSIM, thiết bị di động của bạn phải được mở khoá mạng và tương thích với eSIM.
                      Danh sách dưới đây là phiên bản mock theo bố cục Gigago để hiển thị trong checkout.
                    </p>
                  </div>

                  <section className="space-y-2">
                    <h4 className="font-bold text-navy">APPLE</h4>
                    <ul className="list-disc pl-5 text-sm text-gray-700 grid grid-cols-1 md:grid-cols-2 gap-y-1">
                      <li>iPhone 17, 17 Air, 17 Pro, 17 Pro Max</li>
                      <li>iPhone 16, 16 Plus, 16 Pro, 16 Pro Max</li>
                      <li>iPhone 15, 15 Plus, 15 Pro, 15 Pro Max</li>
                      <li>iPhone 14, 14 Plus, 14 Pro, 14 Pro Max</li>
                      <li>iPhone 13, 13 Mini, 13 Pro, 13 Pro Max</li>
                      <li>iPhone 12, 12 Mini, 12 Pro, 12 Pro Max</li>
                      <li>iPhone 11, 11 Pro, 11 Pro Max</li>
                      <li>iPhone XS, XS Max, XR, iPhone SE 2020/2022</li>
                      <li>iPad Air gen 3, 4</li>
                      <li>iPad Pro 11-inch gen 1, 2, 3</li>
                      <li>iPad Pro 12.9-inch gen 3, 4, 5</li>
                      <li>iPad gen 7, 8, 9; iPad Mini gen 5, 6</li>
                    </ul>
                    <div className="rounded-md bg-amber-50 border border-amber-200 p-3 text-xs text-amber-800 leading-5">
                      <p>- iPhone bản Trung Quốc đại lục không hỗ trợ eSIM.</p>
                      <p>- Một số mẫu từ Hong Kong/Macao có giới hạn hỗ trợ eSIM.</p>
                      <p>- iPad chỉ hỗ trợ khi là bản Wi-Fi + Cellular.</p>
                    </div>
                  </section>

                  <section className="space-y-2">
                    <h4 className="font-bold text-navy">SAMSUNG</h4>
                    <ul className="list-disc pl-5 text-sm text-gray-700 grid grid-cols-1 md:grid-cols-2 gap-y-1">
                      <li>Galaxy S26, S26+, S26 Ultra</li>
                      <li>Galaxy S25, S25+, S25 Ultra, S25 FE</li>
                      <li>Galaxy S24, S24+, S24 Ultra, S24 FE</li>
                      <li>Galaxy S23, S23+, S23 Ultra, S23 FE</li>
                      <li>Galaxy S22 5G, S22+ 5G, S22 Ultra 5G</li>
                      <li>Galaxy S21, S21+ 5G, S21 Ultra 5G</li>
                      <li>Galaxy S20, S20+, S20 Ultra</li>
                      <li>Galaxy Z Flip/Flip3/Flip4/Flip5/Flip6/Flip7</li>
                      <li>Galaxy Z Fold/Fold2/Fold3/Fold4/Fold5/Fold6/Fold7</li>
                      <li>Galaxy Note 20, Note 20 Ultra 5G</li>
                      <li>Galaxy A55 5G, A35, A56, A36</li>
                    </ul>
                    <div className="rounded-md bg-amber-50 border border-amber-200 p-3 text-xs text-amber-800 leading-5">
                      <p>- Dòng S20 FE và S21 FE không hỗ trợ eSIM ở nhiều thị trường.</p>
                      <p>- Một số thiết bị Samsung bản Mỹ/Hàn có thể bị giới hạn eSIM theo nhà mạng.</p>
                    </div>
                  </section>

                  <section className="space-y-2">
                    <h4 className="font-bold text-navy">GOOGLE PIXEL</h4>
                    <ul className="list-disc pl-5 text-sm text-gray-700 grid grid-cols-1 md:grid-cols-2 gap-y-1">
                      <li>Pixel 10, 10 Pro, 10 Pro XL, 10a</li>
                      <li>Pixel 9, 9 Pro, 9 Pro XL</li>
                      <li>Pixel Fold</li>
                      <li>Pixel 8, 8 Pro</li>
                      <li>Pixel 7, 7 Pro</li>
                      <li>Pixel 6, 6a, 6 Pro</li>
                      <li>Pixel 5</li>
                      <li>Pixel 4, 4a, 4 XL</li>
                      <li>Pixel 3, 3 XL, 3a, 3a XL</li>
                    </ul>
                  </section>

                  <section className="space-y-2">
                    <h4 className="font-bold text-navy">XIAOMI / HUAWEI / ONEPLUS / OPPO / SONY / MOTOROLA</h4>
                    <ul className="list-disc pl-5 text-sm text-gray-700 grid grid-cols-1 md:grid-cols-2 gap-y-1">
                      <li>Xiaomi 15, 15 Ultra, 14T, 14T Pro</li>
                      <li>Huawei P40, P40 Pro, Mate 40 Pro, Pura 70 Pro</li>
                      <li>OnePlus 11 5G, OnePlus 12, OnePlus Open</li>
                      <li>OPPO Find X3/X5/X8/X9 series, Find N2/N3 Flip</li>
                      <li>Sony Xperia 10 IV/V, Xperia 1 IV/V/VI, Xperia 5 IV/V</li>
                      <li>Motorola Razr 2019/2022/5G/2024/2024+</li>
                      <li>Motorola Edge 40/50/60 series</li>
                      <li>Motorola Moto G Power 5G (2024), G53/G54/G84...</li>
                    </ul>
                  </section>

                  <section className="space-y-2">
                    <h4 className="font-bold text-navy">WINDOWS 10 / WINDOWS 11</h4>
                    <ul className="list-disc pl-5 text-sm text-gray-700 grid grid-cols-1 md:grid-cols-2 gap-y-1">
                      <li>Acer Swift 3, Swift 7, TravelMate P2/P6</li>
                      <li>ASUS NovaGo TP370QL, VivoBook Flip 14 TP401NA</li>
                      <li>Dell Latitude 7210/7310/7410/9410/9510</li>
                      <li>HP Elitebook G5, Probook G5, Zbook G5</li>
                      <li>Lenovo ThinkPad X1 Carbon Gen 9, X1 Fold, X1 Nano</li>
                      <li>Microsoft Surface Pro (gen 5, 7+) và Surface Pro X</li>
                    </ul>
                  </section>

                  <p className="text-xs text-gray-500 leading-5">
                    Lưu ý: Tính tương thích eSIM có thể thay đổi theo quốc gia, phiên bản phần cứng và nhà mạng.
                    Đây là dữ liệu mock để mô phỏng giao diện popup danh sách thiết bị tương thích.
                  </p>
                </>
              ) : (
                <>
                  <div className="rounded-lg border border-primary/15 bg-primary-light/40 p-3">
                    <p className="font-semibold text-navy">The following devices support eSIM:</p>
                    <p className="text-sm text-gray-600 mt-1">
                      Your device must be network-unlocked and eSIM-compatible.
                      This is a mock compatibility list structured similarly to Gigago.
                    </p>
                  </div>

                  <section className="space-y-2">
                    <h4 className="font-bold text-navy">APPLE</h4>
                    <ul className="list-disc pl-5 text-sm text-gray-700 grid grid-cols-1 md:grid-cols-2 gap-y-1">
                      <li>iPhone 17 / 16 / 15 / 14 / 13 / 12 / 11 series</li>
                      <li>iPhone XS, XS Max, XR, iPhone SE 2020/2022</li>
                      <li>iPad Air gen 3/4, iPad Pro 11-inch gen 1/2/3</li>
                      <li>iPad Pro 12.9-inch gen 3/4/5, iPad gen 7/8/9</li>
                    </ul>
                  </section>

                  <section className="space-y-2">
                    <h4 className="font-bold text-navy">SAMSUNG</h4>
                    <ul className="list-disc pl-5 text-sm text-gray-700 grid grid-cols-1 md:grid-cols-2 gap-y-1">
                      <li>Galaxy S26/S25/S24/S23/S22/S21/S20 series</li>
                      <li>Galaxy Z Flip and Z Fold family</li>
                      <li>Galaxy Note 20 series, selected Galaxy A series</li>
                    </ul>
                  </section>

                  <section className="space-y-2">
                    <h4 className="font-bold text-navy">GOOGLE PIXEL</h4>
                    <ul className="list-disc pl-5 text-sm text-gray-700 grid grid-cols-1 md:grid-cols-2 gap-y-1">
                      <li>Pixel 10/9/8/7/6 series, Pixel Fold</li>
                      <li>Pixel 5, Pixel 4 family, Pixel 3 family</li>
                    </ul>
                  </section>

                  <section className="space-y-2">
                    <h4 className="font-bold text-navy">OTHER BRANDS</h4>
                    <ul className="list-disc pl-5 text-sm text-gray-700 grid grid-cols-1 md:grid-cols-2 gap-y-1">
                      <li>Xiaomi, Huawei, OnePlus, OPPO, Sony, Motorola selected models</li>
                      <li>Windows laptops from Acer/ASUS/Dell/HP/Lenovo/Surface with eSIM support</li>
                    </ul>
                  </section>

                  <p className="text-xs text-gray-500 leading-5">
                    Note: eSIM compatibility varies by market, region, carrier, and hardware variant.
                    This is mock data for checkout popup presentation.
                  </p>
                </>
              )}
            </div>

            <div className="px-5 py-4 border-t border-gray-100 flex items-center justify-end gap-2">
              <button
                type="button"
                onClick={() => setShowDeviceModal(false)}
                className="px-4 py-2 rounded-lg border border-gray-200 text-gray-600 hover:bg-gray-50"
              >
                {text.close}
              </button>
              <button
                type="button"
                onClick={() => void handleDeviceConfirm()}
                className="px-4 py-2 rounded-lg bg-primary text-white hover:bg-primary-dark"
              >
                {text.deviceConfirm}
              </button>
            </div>
          </div>
        </div>
      )}
    </form>
  );
}
