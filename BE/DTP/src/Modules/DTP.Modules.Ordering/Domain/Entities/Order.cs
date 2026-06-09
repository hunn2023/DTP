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
            Guid userId,
            string customerEmail,
            string? customerName,
            string? customerPhone,
            string currencyCode,
            string? ipAddress,
            string? userAgent,
            string? referrerUrl,
            string? checkoutSource)
        {
            Id = Guid.NewGuid();
            OrderCode = orderCode;
            UserId = userId;
            CustomerEmail = customerEmail;
            CustomerName = customerName;
            CustomerPhone = customerPhone;
            CurrencyCode = currencyCode;

            IpAddress = ipAddress;
            UserAgent = userAgent;
            ReferrerUrl = referrerUrl;
            CheckoutSource = checkoutSource;

            Status = OrderStatus.WaitingPayment;
            PaymentStatus = OrderPaymentStatus.Unpaid;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public string OrderCode { get; private set; } = default!;

        public Guid UserId { get; private set; }

        public string CustomerEmail { get; private set; } = default!;
        public string? CustomerName { get; private set; }
        public string? CustomerPhone { get; private set; }

        public string CurrencyCode { get; private set; } = "VND";

        public decimal SubtotalAmount { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TotalAmount { get; private set; }

        public OrderStatus Status { get; private set; }
        public OrderPaymentStatus PaymentStatus { get; private set; }

        public DateTime? PaidAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public string? PaymentTransactionCode { get; private set; }
        public string? Note { get; private set; }

        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public string? ReferrerUrl { get; private set; }
        public string? CheckoutSource { get; private set; }


        public IReadOnlyCollection<OrderItem> Items => _items;
        public IReadOnlyCollection<OrderHistory> Histories => _histories;

        public void AddItem(OrderItem item)
        {
            _items.Add(item);
            RecalculateAmount();
        }

        public void RecalculateAmount()
        {
            SubtotalAmount = _items.Sum(x => x.TotalPrice);
            TotalAmount = SubtotalAmount - DiscountAmount;

            if (TotalAmount < 0)
                TotalAmount = 0;

            UpdatedAt = DateTime.UtcNow;
        }

        public void AttachPaymentTransaction(string paymentTransactionCode)
        {
            PaymentTransactionCode = paymentTransactionCode;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkPaid(string? note = null)
        {
            if (PaymentStatus == OrderPaymentStatus.Paid)
                return;

            var oldStatus = Status;

            PaymentStatus = OrderPaymentStatus.Paid;
            Status = OrderStatus.Paid;
            PaidAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            AddHistory(oldStatus, Status, note ?? "Payment successful");
        }

        public void MarkProcessing(string? note = null)
        {
            ChangeStatus(OrderStatus.Processing, note);
        }

        public void MarkDelivered(string? note = null)
        {
            ChangeStatus(OrderStatus.Delivered, note);
        }

        public void MarkCompleted(string? note = null)
        {
            ChangeStatus(OrderStatus.Completed, note);
            CompletedAt = DateTime.UtcNow;
        }

        public void Cancel(string? note = null)
        {
            if (Status == OrderStatus.Completed || Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel completed or delivered order.");

            var oldStatus = Status;

            Status = OrderStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            AddHistory(oldStatus, Status, note ?? "Order cancelled");
        }

        public void MarkPaymentFailed(string? note = null)
        {
            var oldStatus = Status;

            PaymentStatus = OrderPaymentStatus.Failed;
            Status = OrderStatus.PaymentFailed;
            UpdatedAt = DateTime.UtcNow;

            AddHistory(oldStatus, Status, note ?? "Payment failed");
        }

        private void ChangeStatus(OrderStatus newStatus, string? note = null)
        {
            if (Status == newStatus)
                return;

            var oldStatus = Status;

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;

            AddHistory(oldStatus, newStatus, note);
        }

        private void AddHistory(OrderStatus fromStatus, OrderStatus toStatus, string? note)
        {
            _histories.Add(new OrderHistory(
                Id,
                fromStatus,
                toStatus,
                note));
        }
    }
}
