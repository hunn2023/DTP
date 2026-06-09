using DTP.Modules.Delivery.Application.Abstractions.Repositories;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Queries
{
    public record GetDeliveriesPagedQuery(
      string? Keyword,
      DeliveryStatus? Status,
      int Page,
      int PageSize) : IRequest<Result<PagedResultDto<DeliveryDto>>>;

    public class GetDeliveriesPagedQueryHandler
        : IRequestHandler<GetDeliveriesPagedQuery, Result<PagedResultDto<DeliveryDto>>>
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public GetDeliveriesPagedQueryHandler(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<Result<PagedResultDto<DeliveryDto>>> Handle(
            GetDeliveriesPagedQuery request,
            CancellationToken cancellationToken)
        {
            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

            var (items, total) = await _deliveryRepository.GetPagedAsync(
                request.Keyword,
                request.Status,
                page,
                pageSize,
                cancellationToken);

            var data = items.Select(x => new DeliveryDto
            {
                Id = x.Id,
                OrderId = x.OrderId,
                OrderCode = x.OrderCode,
                CustomerId = x.CustomerId,
                CustomerName = x.CustomerName,
                CustomerEmail = x.CustomerEmail,
                DeliveryType = x.DeliveryType,
                Status = x.Status,
                AttemptCount = x.AttemptCount,
                LastError = x.LastError,
                IpAddress = x.IpAddress,
                Note = x.Note,
                DeliveredAt = x.DeliveredAt,
                FailedAt = x.FailedAt,
                CreatedAt = x.CreatedAt,
                Items = x.Items.Select(i => new DeliveryItemDto
                {
                    Id = i.Id,
                    OrderItemId = i.OrderItemId,
                    ProductId = i.ProductId,
                    ProductVariantId = i.ProductVariantId,
                    ProductName = i.ProductName,
                    Sku = i.Sku,
                    Quantity = i.Quantity,
                    QrCodeUrl = i.QrCodeUrl,
                    ActivationCode = i.ActivationCode,
                    SerialNumber = i.SerialNumber,
                    ProviderReference = i.ProviderReference,
                    IsDelivered = i.IsDelivered,
                    DeliveredAt = i.DeliveredAt
                }).ToList()
            }).ToList();

            var result = new PagedResultDto<DeliveryDto>
            {
                Items = data,
                TotalCount = total,
                PageIndex = page,
                PageSize = pageSize
            };

            return Result<PagedResultDto<DeliveryDto>>.Success(result);
        }
    }
}
