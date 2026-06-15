using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Clients;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.DTOs.Peacoms;
using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Services
{
    public class ProviderReservationService : IProviderReservationService
    {
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderOrderRepository _providerOrderRepository;
        private readonly IProviderProductMappingRepository _mappingRepository;
        private readonly IProviderOrderReader _orderReader;
        private readonly IPeacomProviderClient _peacomClient;
        private readonly IProviderUnitOfWork _unitOfWork;

        public ProviderReservationService(
            IProviderRepository providerRepository,
            IProviderOrderRepository providerOrderRepository,
            IProviderProductMappingRepository mappingRepository,
            IProviderOrderReader orderReader,
            IPeacomProviderClient peacomClient,
            IProviderUnitOfWork unitOfWork)
        {
            _providerRepository = providerRepository;
            _providerOrderRepository = providerOrderRepository;
            _mappingRepository = mappingRepository;
            _orderReader = orderReader;
            _peacomClient = peacomClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProviderReservationResult> ReserveOrderAsync(
            Guid dtpOrderId,
            CancellationToken cancellationToken = default)
        {
            var existingProviderOrder = await _providerOrderRepository.GetByDtpOrderIdAsync(
                dtpOrderId,
                cancellationToken);

            if (existingProviderOrder is not null && !existingProviderOrder.IsExpired())
            {
                return new ProviderReservationResult
                {
                    ProviderOrderId = existingProviderOrder.Id,
                    ProviderOrderPublicId = existingProviderOrder.ProviderOrderPublicId,
                    ReservedUntil = existingProviderOrder.ReservedUntil
                };
            }

            if (existingProviderOrder is not null && existingProviderOrder.IsExpired())
            {
                existingProviderOrder.MarkExpired();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var provider = await _providerRepository.GetByCodeAsync(
                "PEACOM",
                cancellationToken);

            if (provider is null)
                throw new InvalidOperationException("Provider PEACOM chưa được cấu hình.");

            if (!provider.IsActive)
                throw new InvalidOperationException("Provider PEACOM đang inactive.");

            var dtpOrder = await _orderReader.GetOrderForProviderAsync(
                dtpOrderId,
                cancellationToken);

            if (dtpOrder is null)
                throw new InvalidOperationException("Không tìm thấy order nội bộ DTP.");

            if (dtpOrder.Items.Count == 0)
                throw new InvalidOperationException("Order không có item.");

            var createOrderRequest = new PeacomCreateOrderRequest
            {
                Description = $"DTP Order {dtpOrder.OrderCode}"
            };

            foreach (var item in dtpOrder.Items)
            {
                var mapping = await _mappingRepository.GetByEsimPackageIdAsync(
                    item.EsimPackageId,
                    cancellationToken);

                if (mapping is null)
                    throw new InvalidOperationException($"Không tìm thấy provider mapping cho package {item.EsimPackageId}.");

                if (!int.TryParse(mapping.ProviderProductId, out var providerProductId))
                    throw new InvalidOperationException($"ProviderProductId không hợp lệ cho SKU {mapping.ProviderSku}.");

                createOrderRequest.Products.Add(new PeacomCreateOrderItemRequest
                {
                    ProductId = providerProductId,
                    Sku = mapping.ProviderSku,
                    Qty = item.Quantity <= 0 ? 1 : item.Quantity
                });
            }

            var response = await _peacomClient.CreateOrderAsync(
                createOrderRequest,
                cancellationToken);

            var providerOrder = new ProviderOrder(
                provider.Id,
                dtpOrder.OrderId,
                response.OrderPublicId,
                response.Amount,
                response.NumOfProduct,
                response.Status,
                response.RawJson);

            await _providerOrderRepository.AddAsync(
                providerOrder,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ProviderReservationResult
            {
                ProviderOrderId = providerOrder.Id,
                ProviderOrderPublicId = providerOrder.ProviderOrderPublicId,
                ReservedUntil = providerOrder.ReservedUntil
            };
        }
    }
}
