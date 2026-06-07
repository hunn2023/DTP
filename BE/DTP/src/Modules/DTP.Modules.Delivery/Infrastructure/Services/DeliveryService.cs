using DTP.Modules.Delivery.Application.Abstractions.Repositories;
using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Modules.Delivery.Domain.Entities;
using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Infrastructure.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IEsimProfileRepository _esimProfileRepository;
        private readonly IDigitalDeliveryRepository _digitalDeliveryRepository;
        private readonly IDeliveryUnitOfWork _unitOfWork;
        private readonly IDeliveryOrderingService _orderingService;
        private readonly IDeliveryNotificationService _notificationService;

        public DeliveryService(
            IEsimProfileRepository esimProfileRepository,
            IDigitalDeliveryRepository digitalDeliveryRepository,
            IDeliveryUnitOfWork unitOfWork,
            IDeliveryOrderingService orderingService,
            IDeliveryNotificationService notificationService)
        {
            _esimProfileRepository = esimProfileRepository;
            _digitalDeliveryRepository = digitalDeliveryRepository;
            _unitOfWork = unitOfWork;
            _orderingService = orderingService;
            _notificationService = notificationService;
        }

        public async Task<DeliverOrderResultDto> DeliverOrderAsync(
            DeliverOrderDto request,
            CancellationToken cancellationToken = default)
        {
            if (request.OrderId == Guid.Empty)
                throw new Exception("OrderId is required.");

            var order = await _orderingService.GetOrderForDeliveryAsync(
                request.OrderId,
                cancellationToken);

            if (order == null)
                throw new Exception("Order not found.");

            var deliveredProfiles = new List<EsimProfileDto>();

            foreach (var item in order.Items)
            {
                if (!item.EsimPackageId.HasValue)
                {
                    await AddLogAsync(
                        order.OrderId,
                        item.OrderItemId,
                        "SkipNonEsimItem",
                        DeliveryLogStatus.Info,
                        "Order item is not eSIM package.",
                        null,
                        cancellationToken);

                    continue;
                }

                for (var i = 0; i < item.Quantity; i++)
                {
                    var profile = await _esimProfileRepository.GetAvailableForOrderItemAsync(
                        item.ProductId,
                        item.ProductVariantId,
                        item.EsimPackageId,
                        cancellationToken);

                    if (profile == null)
                    {
                        await AddLogAsync(
                            order.OrderId,
                            item.OrderItemId,
                            "AssignEsimProfile",
                            DeliveryLogStatus.Failed,
                            "No available eSIM profile.",
                            JsonSerializer.Serialize(item),
                            cancellationToken);

                        throw new Exception($"No available eSIM profile for product {item.ProductName}.");
                    }

                    profile.AssignToOrder(order.OrderId, item.OrderItemId);
                    profile.MarkDelivered();

                    _esimProfileRepository.Update(profile);

                    var delivery = new DigitalDelivery(
                        order.OrderId,
                        item.OrderItemId,
                        profile.Id,
                        order.CustomerEmail);

                    delivery.MarkDelivered();

                    await _digitalDeliveryRepository.AddAsync(
                        delivery,
                        cancellationToken);

                    await AddLogAsync(
                        order.OrderId,
                        item.OrderItemId,
                        "DeliverEsimProfile",
                        DeliveryLogStatus.Success,
                        "eSIM profile delivered.",
                        JsonSerializer.Serialize(new
                        {
                            profile.Id,
                            profile.Iccid,
                            profile.ActivationCode
                        }),
                        cancellationToken);

                    deliveredProfiles.Add(MapEsimProfile(profile));
                }
            }

            if (!deliveredProfiles.Any())
                throw new Exception("No eSIM profile delivered.");

            await _notificationService.SendEsimDeliveryEmailAsync(
                order.CustomerEmail,
                order.CustomerName,
                order.OrderCode,
                deliveredProfiles,
                cancellationToken);

            await _orderingService.MarkOrderDeliveredAsync(
                order.OrderId,
                request.Note ?? "eSIM delivered.",
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DeliverOrderResultDto
            {
                OrderId = order.OrderId,
                Success = true,
                Message = "eSIM delivered successfully.",
                EsimProfileIds = deliveredProfiles.Select(x => x.Id).ToList()
            };
        }

        public async Task<int> ImportEsimProfilesAsync(
            List<ImportEsimProfileDto> items,
            CancellationToken cancellationToken = default)
        {
            if (items == null || items.Count == 0)
                throw new Exception("No eSIM profiles to import.");

            var profiles = new List<EsimProfile>();

            foreach (var item in items)
            {
                if (item.ProductId == Guid.Empty)
                    throw new Exception("ProductId is required.");

                if (string.IsNullOrWhiteSpace(item.Iccid))
                    throw new Exception("ICCID is required.");

                if (string.IsNullOrWhiteSpace(item.ActivationCode))
                    throw new Exception("ActivationCode is required.");

                var exists = await _esimProfileRepository.ExistsIccidAsync(
                    item.Iccid,
                    cancellationToken);

                if (exists)
                    continue;

                var profile = new EsimProfile(
                    item.ProductId,
                    item.ProductVariantId,
                    item.EsimPackageId,
                    item.ProviderId,
                    item.Iccid.Trim(),
                    item.Imsi,
                    item.Msisdn,
                    item.ActivationCode.Trim(),
                    item.QrCodeUrl,
                    item.QrContent,
                    item.SmdpAddress,
                    item.MatchingId,
                    item.ExpiredAt);

                profiles.Add(profile);
            }

            if (profiles.Count == 0)
                return 0;

            await _esimProfileRepository.AddRangeAsync(
                profiles,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return profiles.Count;
        }

        public async Task<PagedResultDto<EsimProfileDto>> GetEsimProfilesAsync(
            Guid? productId,
            Guid? productVariantId,
            EsimProfileStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            if (pageSize > 100)
                pageSize = 100;

            var query = _esimProfileRepository.Query()
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (productId.HasValue)
                query = query.Where(x => x.ProductId == productId.Value);

            if (productVariantId.HasValue)
                query = query.Where(x => x.ProductVariantId == productVariantId.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EsimProfileDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    EsimPackageId = x.EsimPackageId,
                    ProviderId = x.ProviderId,
                    OrderId = x.OrderId,
                    OrderItemId = x.OrderItemId,
                    Iccid = x.Iccid,
                    Imsi = x.Imsi,
                    Msisdn = x.Msisdn,
                    ActivationCode = x.ActivationCode,
                    QrCodeUrl = x.QrCodeUrl,
                    QrContent = x.QrContent,
                    SmdpAddress = x.SmdpAddress,
                    MatchingId = x.MatchingId,
                    Status = x.Status,
                    StatusName = x.Status.ToString(),
                    CreatedAt = x.CreatedAt,
                    AssignedAt = x.AssignedAt,
                    DeliveredAt = x.DeliveredAt,
                    ExpiredAt = x.ExpiredAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<EsimProfileDto>
            {
                Items = items,
                TotalItems = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<DigitalDeliveryDto>> GetDeliveriesByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var deliveries = await _digitalDeliveryRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

            return deliveries.Select(x => new DigitalDeliveryDto
            {
                Id = x.Id,
                OrderId = x.OrderId,
                OrderItemId = x.OrderItemId,
                EsimProfileId = x.EsimProfileId,
                RecipientEmail = x.RecipientEmail,
                Status = x.Status,
                StatusName = x.Status.ToString(),
                DeliveredAt = x.DeliveredAt,
                FailedReason = x.FailedReason,
                CreatedAt = x.CreatedAt
            }).ToList();
        }

        private async Task AddLogAsync(
            Guid orderId,
            Guid? orderItemId,
            string action,
            DeliveryLogStatus status,
            string? message,
            string? rawData,
            CancellationToken cancellationToken)
        {
            var log = new DeliveryLog(
                orderId,
                orderItemId,
                action,
                status,
                message,
                rawData);

            await _digitalDeliveryRepository.AddLogAsync(
                log,
                cancellationToken);
        }

        private static EsimProfileDto MapEsimProfile(EsimProfile profile)
        {
            return new EsimProfileDto
            {
                Id = profile.Id,
                ProductId = profile.ProductId,
                ProductVariantId = profile.ProductVariantId,
                EsimPackageId = profile.EsimPackageId,
                ProviderId = profile.ProviderId,
                OrderId = profile.OrderId,
                OrderItemId = profile.OrderItemId,
                Iccid = profile.Iccid,
                Imsi = profile.Imsi,
                Msisdn = profile.Msisdn,
                ActivationCode = profile.ActivationCode,
                QrCodeUrl = profile.QrCodeUrl,
                QrContent = profile.QrContent,
                SmdpAddress = profile.SmdpAddress,
                MatchingId = profile.MatchingId,
                Status = profile.Status,
                StatusName = profile.Status.ToString(),
                CreatedAt = profile.CreatedAt,
                AssignedAt = profile.AssignedAt,
                DeliveredAt = profile.DeliveredAt,
                ExpiredAt = profile.ExpiredAt
            };
        }
    }
}
