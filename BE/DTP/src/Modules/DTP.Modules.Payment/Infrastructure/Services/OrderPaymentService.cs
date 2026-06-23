using DTP.Modules.Ordering.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Shared.Application;
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
                Status = order.Status.ToString(),
                PaymentExpiredAt = order.PaymentExpiredAt,
                CustomerEmail = order.CustomerEmail ?? "",
                CustomerName = order.CustomerName ?? "",
                CustomerPhone = order.CustomerPhone ?? ""

            };
        }

        public async Task<Result> MarkOrderPaidAsync(
            Guid orderId,
            Guid paymentTransactionId,
            string? providerTransactionId,
            DateTime paidAt,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

                if (order == null)
                    return Result.Failure("Order not found.");

                order.MarkPaid(paymentTransactionId.ToString());

                _orderRepository.Update(order);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> MarkFulfillmentFailedAsync(Guid orderId,
            string reason,
            CancellationToken cancellationToken = default)
        {

            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

            if (order == null)
                return Result.Failure("Order not found.");


            order.MarkFulfillmentFailed(reason);

            _orderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
