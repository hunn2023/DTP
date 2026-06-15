using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Domain;

namespace DTP.Modules.Ordering.Domain.Entities
{
    public class Order : EntityBase
    {
        private readonly List<OrderItem> _items = new();
        private readonly List<OrderHistory> _histories = new();

        private Order()
        {
        }

        public Order(
            string orderCode,
            Guid? customerId,
            string? customerEmail,
            string? customerPhone,
            string? customerName,
            string currency,
            string? note)
        {
            Id = Guid.NewGuid();
            OrderCode = orderCode;
            CustomerId = customerId;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
            CustomerName = customerName;
            Currency = currency;
            Note = note;

            Status = OrderStatus.Draft;
            PaymentStatus = OrderPaymentStatus.Unpaid;

            SubTotal = 0;
            DiscountAmount = 0;
            TotalAmount = 0;
            PaymentExpiredAt = DateTime.UtcNow.AddMinutes(15);
            CreatedAt = DateTime.UtcNow;
        }

        public string OrderCode { get; private set; } = default!;

        public Guid? CustomerId { get; private set; }

        public string? CustomerEmail { get; private set; }

        public string? CustomerPhone { get; private set; }

        public string? CustomerName { get; private set; }

        public string Currency { get; private set; } = "VND";

        public decimal SubTotal { get; private set; }

        public decimal DiscountAmount { get; private set; }

        public decimal TotalAmount { get; private set; }

        public OrderStatus Status { get; private set; }

        public OrderPaymentStatus PaymentStatus { get; private set; }

        public string? PaymentMethod { get; private set; }

        public string? PaymentTransactionId { get; private set; }

        public DateTime? PaidAt { get; private set; }

        public string? Note { get; private set; }

        public DateTime? PaymentExpiredAt { get; private set; }

        public DateTime? CancelledAt { get; private set; }

        public string? CancelReason { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items;

        public IReadOnlyCollection<OrderHistory> Histories => _histories;

        public void AddItem(OrderItem item)
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException("Chỉ được thêm sản phẩm khi đơn hàng đang ở trạng thái nháp.");

            _items.Add(item);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveItem(Guid orderItemId)
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException("Chỉ được xoá sản phẩm khi đơn hàng đang ở trạng thái nháp.");

            var item = _items.FirstOrDefault(x => x.Id == orderItemId);

            if (item is null)
                return;

            _items.Remove(item);
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Confirm(string? paymentMethod)
        {
            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Không thể xác nhận đơn hàng đã bị hủy.");

            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException("Không thể xác nhận đơn hàng đã hoàn tất.");

            if (PaymentStatus == OrderPaymentStatus.Paid)
                throw new InvalidOperationException("Đơn hàng đã được thanh toán.");

            if (Items == null || Items.Count == 0)
                throw new InvalidOperationException("Đơn hàng chưa có sản phẩm.");

            if (TotalAmount <= 0)
                throw new InvalidOperationException("Tổng tiền đơn hàng không hợp lệ.");

            PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod)
                ? null
                : paymentMethod.Trim();

            Status = OrderStatus.PendingPayment;
            PaymentStatus = OrderPaymentStatus.Unpaid;
            PaymentExpiredAt = DateTime.UtcNow.AddMinutes(15);
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkPaid(string paymentTransactionId)
        {
            if (PaymentStatus == OrderPaymentStatus.Paid)
                return;

            PaymentTransactionId = paymentTransactionId;
            PaymentStatus = OrderPaymentStatus.Paid;
            Status = OrderStatus.Paid;
            PaidAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkProcessing()
        {
            if (Status != OrderStatus.Paid)
                throw new InvalidOperationException("Chỉ xử lý đơn hàng đã thanh toán.");

            Status = OrderStatus.Processing;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != OrderStatus.Paid && Status != OrderStatus.Processing)
                throw new InvalidOperationException("Chỉ hoàn tất đơn hàng đã thanh toán hoặc đang xử lý.");

            Status = OrderStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel(string reason)
        {
            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException("Không thể huỷ đơn hàng đã hoàn tất.");

            Status = OrderStatus.Cancelled;
            CancelReason = reason;
            CancelledAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string reason)
        {
            Status = OrderStatus.Failed;
            CancelReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }


        public void MarkFulfillmentFailed(string reason)
        {
            if (Status != OrderStatus.Processing)
                throw new InvalidOperationException("Chỉ đánh dấu lỗi giao hàng cho đơn hàng đang xử lý.");
            Status = OrderStatus.FulfillmentFailed;
            CancelReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }
        public void ApplyDiscount(decimal discountAmount)
        {
            if (discountAmount < 0)
                throw new InvalidOperationException("Giảm giá không hợp lệ.");

            DiscountAmount = discountAmount;
            RecalculateTotal();
            UpdatedAt = DateTime.UtcNow;
        }

        private void RecalculateTotal()
        {
            SubTotal = _items.Sum(x => x.TotalPrice);
            TotalAmount = SubTotal - DiscountAmount;

            if (TotalAmount < 0)
                TotalAmount = 0;
        }


        public void Expire(string? reason = null)
        {
            if (Status != OrderStatus.PendingPayment)
                throw new InvalidOperationException("Chỉ có thể hết hạn đơn hàng đang chờ thanh toán.");

            if (PaymentStatus != OrderPaymentStatus.Unpaid)
                throw new InvalidOperationException("Chỉ có thể hết hạn đơn hàng chưa thanh toán.");

            Status = OrderStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            CancelReason = string.IsNullOrWhiteSpace(reason)
                ? "Đơn hàng hết hạn thanh toán."
                : reason.Trim();

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
