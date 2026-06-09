using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Queries
{
    public class GetProviderOrderDetailQuery : IRequest<ProviderOrderDetailDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetProviderOrderDetailQueryHandler
        : IRequestHandler<GetProviderOrderDetailQuery, ProviderOrderDetailDto?>
    {
        private readonly IProviderOrderRepository _repository;

        public GetProviderOrderDetailQueryHandler(IProviderOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProviderOrderDetailDto?> Handle(
            GetProviderOrderDetailQuery request,
            CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (order == null)
                return null;

            return new ProviderOrderDetailDto
            {
                Id = order.Id,
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                ProviderId = order.ProviderId,
                ProviderName = order.Provider.Name,
                ProviderOrderCode = order.ProviderOrderCode,
                Status = order.Status,
                RetryCount = order.RetryCount,
                ErrorCode = order.ErrorCode,
                ErrorMessage = order.ErrorMessage,
                SentAt = order.SentAt,
                CompletedAt = order.CompletedAt,
                Items = order.Items.Select(x => new ProviderOrderItemDto
                {
                    Id = x.Id,
                    OrderItemId = x.OrderItemId,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    ProviderProductCode = x.ProviderProductCode,
                    Quantity = x.Quantity,
                    Iccid = x.Iccid,
                    Msisdn = x.Msisdn,
                    QrCodeUrl = x.QrCodeUrl,
                    QrCodeText = x.QrCodeText,
                    ActivationCode = x.ActivationCode,
                    Serial = x.Serial,
                    PinCode = x.PinCode,
                    ExpiredAt = x.ExpiredAt
                }).ToList()
            };
        }
    }
}
