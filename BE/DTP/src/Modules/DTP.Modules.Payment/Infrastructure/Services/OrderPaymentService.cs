using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class OrderPaymentService : IOrderPaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderUnitOfWork _unitOfWork;

        public OrderPaymentService(
            IOrderRepository orderRepository,
            IOrderUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderPaymentInfo?> GetOrderPaymentInfoAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

            if (order == null)
                return null;

            return new OrderPaymentInfo
            {
                OrderId = order.Id,
                OrderCode = order.OrderCode,
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Currency = order.Currency,
                Status = order.Status.ToString()
            };
        }

        public async Task MarkOrderPaidAsync(
            Guid orderId,
            Guid paymentTransactionId,
            string? providerTransactionId,
            DateTime paidAt,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            order.MarkPaid(paymentTransactionId.ToString());

            _orderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
