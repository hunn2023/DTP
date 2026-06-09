using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Gateways;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Services
{
    public class ProviderProvisioningService : IProviderProvisioningService
    {
        private readonly IProviderProductMappingRepository _mappingRepository;
        private readonly IProviderOrderRepository _providerOrderRepository;
        private readonly IExternalProviderRepository _providerRepository;
        private readonly IProviderGatewayFactory _gatewayFactory;
        private readonly IProviderUnitOfWork _unitOfWork;

        public ProviderProvisioningService(
            IProviderProductMappingRepository mappingRepository,
            IProviderOrderRepository providerOrderRepository,
            IExternalProviderRepository providerRepository,
            IProviderGatewayFactory gatewayFactory,
            IProviderUnitOfWork unitOfWork)
        {
            _mappingRepository = mappingRepository;
            _providerOrderRepository = providerOrderRepository;
            _providerRepository = providerRepository;
            _gatewayFactory = gatewayFactory;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProviderProvisionResultDto> ProvisionOrderAsync(
            Guid orderId,
            string orderCode,
            List<ProviderProvisionOrderItemInput> items,
            CancellationToken cancellationToken = default)
        {
            if (items == null || items.Count == 0)
                throw new Exception("Provider order item is empty.");

            var existingProviderOrder = await _providerOrderRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

            if (existingProviderOrder != null)
            {
                return new ProviderProvisionResultDto
                {
                    Success = existingProviderOrder.Status == ProviderOrderStatus.Success,
                    ProviderOrderId = existingProviderOrder.Id,
                    ProviderOrderCode = existingProviderOrder.ProviderOrderCode,
                    ErrorCode = existingProviderOrder.ErrorCode,
                    ErrorMessage = existingProviderOrder.ErrorMessage
                };
            }

            var firstItem = items.First();

            var firstMapping = await _mappingRepository.GetActiveMappingAsync(
                ProviderProductType.Esim,
                firstItem.ProductId,
                firstItem.ProductVariantId,
                cancellationToken);

            if (firstMapping == null)
                throw new Exception("Provider mapping not found.");

            var provider = await _providerRepository.GetByIdAsync(
                firstMapping.ProviderId,
                cancellationToken);

            if (provider == null || !provider.IsActive)
                throw new Exception("Provider not found or inactive.");

            var providerOrder = new ProviderOrder(
                orderId,
                orderCode,
                provider.Id);

            foreach (var item in items)
            {
                var mapping = await _mappingRepository.GetActiveMappingAsync(
                    ProviderProductType.Esim,
                    item.ProductId,
                    item.ProductVariantId,
                    cancellationToken);

                if (mapping == null)
                    throw new Exception($"Provider mapping not found for variant {item.ProductVariantId}.");

                if (mapping.ProviderId != provider.Id)
                    throw new Exception("All items in one provider order must use the same provider.");

                providerOrder.AddItem(
                    item.OrderItemId,
                    item.ProductId,
                    item.ProductVariantId,
                    mapping.ProviderProductCode,
                    item.Quantity);
            }

            await _providerOrderRepository.AddAsync(providerOrder, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            providerOrder.MarkProcessing();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var gateway = _gatewayFactory.Create(provider.Code);

            var request = new ProviderCreateOrderRequest
            {
                ProviderId = provider.Id,
                OrderId = orderId,
                OrderCode = orderCode,
                Items = providerOrder.Items.Select(x => new ProviderCreateOrderItemRequest
                {
                    OrderItemId = x.OrderItemId,
                    ProviderProductCode = x.ProviderProductCode,
                    Quantity = x.Quantity
                }).ToList()
            };

            var result = await gateway.CreateOrderAsync(request, cancellationToken);

            if (result.Success)
            {
                providerOrder.MarkSuccess(result.ProviderOrderCode ?? string.Empty);

                foreach (var resultItem in result.Items)
                {
                    var providerOrderItem = providerOrder.Items
                        .FirstOrDefault(x => x.OrderItemId == resultItem.OrderItemId);

                    if (providerOrderItem == null)
                        continue;

                    providerOrderItem.SetEsimResult(
                        resultItem.Iccid,
                        resultItem.Msisdn,
                        resultItem.QrCodeUrl,
                        resultItem.QrCodeText,
                        resultItem.ActivationCode,
                        resultItem.ExpiredAt);
                }
            }
            else
            {
                providerOrder.MarkFailed(
                    result.ErrorCode,
                    result.ErrorMessage ?? "Provider order failed.");
            }

            await _providerOrderRepository.AddAsync(providerOrder, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await SendProviderOrderAsync(providerOrder, cancellationToken);
        }

        public async Task<ProviderProvisionResultDto> RetryAsync(
       Guid providerOrderId,
       CancellationToken cancellationToken = default)
        {
            var providerOrder = await _providerOrderRepository.GetByIdAsync(
                providerOrderId,
                cancellationToken);

            if (providerOrder == null)
                throw new Exception("Provider order not found.");

            if (providerOrder.Status == ProviderOrderStatus.Success)
            {
                return new ProviderProvisionResultDto
                {
                    Success = true,
                    ProviderOrderId = providerOrder.Id,
                    ProviderOrderCode = providerOrder.ProviderOrderCode
                };
            }

            return await SendProviderOrderAsync(providerOrder, cancellationToken);
        }


        private async Task<ProviderProvisionResultDto> SendProviderOrderAsync(
    ProviderOrder providerOrder,
    CancellationToken cancellationToken = default)
        {
            var provider = await _providerRepository.GetByIdAsync(
                providerOrder.ProviderId,
                cancellationToken);

            if (provider == null || !provider.IsActive)
                throw new Exception("Provider not found or inactive.");

            providerOrder.MarkProcessing();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var gateway = _gatewayFactory.Create(provider.Code);

            var request = new ProviderCreateOrderRequest
            {
                ProviderId = provider.Id,
                OrderId = providerOrder.OrderId,
                OrderCode = providerOrder.OrderCode,
                Items = providerOrder.Items.Select(x => new ProviderCreateOrderItemRequest
                {
                    OrderItemId = x.OrderItemId,
                    ProviderProductCode = x.ProviderProductCode,
                    Quantity = x.Quantity
                }).ToList()
            };

            var result = await gateway.CreateOrderAsync(request, cancellationToken);

            if (result.Success)
            {
                providerOrder.MarkSuccess(result.ProviderOrderCode ?? string.Empty);

                foreach (var resultItem in result.Items)
                {
                    var providerOrderItem = providerOrder.Items
                        .FirstOrDefault(x => x.OrderItemId == resultItem.OrderItemId);

                    if (providerOrderItem == null)
                        continue;

                    providerOrderItem.SetEsimResult(
                        resultItem.Iccid,
                        resultItem.Msisdn,
                        resultItem.QrCodeUrl,
                        resultItem.QrCodeText,
                        resultItem.ActivationCode,
                        resultItem.ExpiredAt);
                }
            }
            else
            {
                providerOrder.MarkFailed(
                    result.ErrorCode,
                    result.ErrorMessage ?? "Provider order failed.");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            result.ProviderOrderId = providerOrder.Id;

            return result;
        }
    }
}
