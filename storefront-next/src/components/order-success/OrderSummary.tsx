import { formatPrice } from "@/lib/product";

interface OrderSummaryProps {
  orderCode: string | undefined;
  total: number | undefined;
  paymentMethod: string | undefined;
}

const PAYMENT_LABELS: Record<string, string> = {
  cod: "COD (Thanh toán khi nhận hàng)",
  COD: "COD (Thanh toán khi nhận hàng)",
  banking: "Chuyển khoản ngân hàng",
  BANKING: "Chuyển khoản ngân hàng",
  momo: "MoMo",
  MOMO: "MoMo",
  vnpay: "VNPay",
  VNPAY: "VNPay",
};

function getPaymentLabel(method: string): string {
  return PAYMENT_LABELS[method] ?? method.toUpperCase();
}

interface SummaryRowProps {
  label: string;
  value: string;
  highlight?: boolean;
}

function SummaryRow({ label, value, highlight = false }: SummaryRowProps) {
  return (
    <div className="flex items-center justify-between py-3 border-b border-gray-100 last:border-0">
      <span className="text-sm text-gray-500">{label}</span>
      <span
        className={`font-semibold ${
          highlight ? "text-base text-primary" : "text-sm text-navy"
        }`}
      >
        {value}
      </span>
    </div>
  );
}

export default function OrderSummary({
  orderCode,
  total,
  paymentMethod,
}: OrderSummaryProps) {
  return (
    <div className="bg-white rounded-2xl shadow-card px-6 py-5 md:px-8">
      <h2 className="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-1">
        Thông tin đơn hàng
      </h2>
      <div>
        {orderCode && <SummaryRow label="Mã đơn hàng" value={orderCode} />}
        {total !== undefined && (
          <SummaryRow label="Tổng tiền" value={formatPrice(total)} highlight />
        )}
        {paymentMethod && (
          <SummaryRow
            label="Phương thức thanh toán"
            value={getPaymentLabel(paymentMethod)}
          />
        )}
      </div>
    </div>
  );
}
