using DTP.Modules.Delivery.Application.DTOs;
using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Abstractions.Services
{
    public interface IDeliveryService
    {
        Task<DeliverOrderResultDto> DeliverOrderAsync(
            DeliverOrderDto request,
            CancellationToken cancellationToken = default);

        Task<int> ImportEsimProfilesAsync(
            List<ImportEsimProfileDto> items,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<EsimProfileDto>> GetEsimProfilesAsync(
            Guid? productId,
            Guid? productVariantId,
            EsimProfileStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<List<DigitalDeliveryDto>> GetDeliveriesByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}
