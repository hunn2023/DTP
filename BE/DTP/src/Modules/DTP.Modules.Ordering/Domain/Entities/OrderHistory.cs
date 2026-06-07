using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Domain;
namespace DTP.Modules.Ordering.Domain.Entities
{
    public class OrderHistory : EntityBase
    {
        private OrderHistory()
        {
        }

        public OrderHistory(
            Guid orderId,
            OrderStatus fromStatus,
            OrderStatus toStatus,
            string? note)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            FromStatus = fromStatus;
            ToStatus = toStatus;
            Note = note;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid OrderId { get; private set; }

        public OrderStatus FromStatus { get; private set; }
        public OrderStatus ToStatus { get; private set; }

        public string? Note { get; private set; }

        public Guid? CreatedBy { get; private set; }

        public Order Order { get; private set; } = default!;
    }
}
