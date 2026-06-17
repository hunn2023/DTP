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
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Services
{
    public class ProviderFulfillmentService : IProviderFulfillmentService
    {
        private readonly IProviderOrderRepository _providerOrderRepository;
        private readonly IProviderOrderItemRepository _providerOrderItemRepository;
        private readonly IProviderRedeemRepository _redeemRepository;
        private readonly IProviderOrderReader _orderReader;
        private readonly IPeacomProviderClient _peacomClient;
        private readonly IProviderUnitOfWork _unitOfWork;

        public ProviderFulfillmentService(
            IProviderOrderRepository providerOrderRepository,
            IProviderOrderItemRepository providerOrderItemRepository,
            IProviderRedeemRepository redeemRepository,
            IProviderOrderReader orderReader,
            IPeacomProviderClient peacomClient,
            IProviderUnitOfWork unitOfWork)
        {
            _providerOrderRepository = providerOrderRepository;
            _providerOrderItemRepository = providerOrderItemRepository;
            _redeemRepository = redeemRepository;
            _orderReader = orderReader;
            _peacomClient = peacomClient;
            _unitOfWork = unitOfWork;
        }

        public async Task ConfirmAndRedeemAsync(
            Guid dtpOrderId,
            CancellationToken cancellationToken = default)
        {
            var providerOrder = await _providerOrderRepository.GetByDtpOrderIdAsync(
                dtpOrderId,
                cancellationToken);

            if (providerOrder is null)
                throw new InvalidOperationException("Chưa có ProviderOrder. Cần gọi CREATE ORDER trước khi tạo QR.");

            if (providerOrder.Status == "Done" || providerOrder.Status == "Confirmed")
                return;

            if (providerOrder.IsExpired())
            {
                providerOrder.MarkExpired();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new InvalidOperationException("ProviderOrder đã quá hạn giữ hàng 15 phút.");
            }

            var dtpOrder = await _orderReader.GetOrderForReservationAsync(
                dtpOrderId,
                cancellationToken);

            if (dtpOrder is null)
                throw new InvalidOperationException("Không tìm thấy order nội bộ DTP.");

            try
            {
                var confirmResponse = await _peacomClient.ConfirmOrderAsync(
                    providerOrder.ProviderOrderPublicId,
                    isConfirm: true,
                    cancellationToken);

                providerOrder.MarkConfirmed(
                    confirmResponse.Amount,
                    confirmResponse.NumOfProduct,
                    confirmResponse.Status,
                    confirmResponse.RawJson);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                foreach (var confirmItem in confirmResponse.Data)
                {
                    var dtpItem = dtpOrder.Items.FirstOrDefault(x =>
                        x.Sku == confirmItem.Sku ||
                        string.Equals(x.Sku, confirmItem.Sku, StringComparison.OrdinalIgnoreCase));

                    var rawSerialsJson = JsonSerializer.Serialize(confirmItem.Serials);

                    var providerOrderItem = await _providerOrderItemRepository.GetByProviderOrderAndSkuAsync(
                        providerOrder.Id,
                        confirmItem.Sku,
                        cancellationToken);

                    if (providerOrderItem is null)
                    {
                        providerOrderItem = new ProviderOrderItem(
                            providerOrder.Id,
                            dtpItem?.OrderItemId,
                            confirmItem.ProductId,
                            confirmItem.Sku,
                            confirmItem.ProductName,
                            confirmItem.Qty,
                            rawSerialsJson);

                        await _providerOrderItemRepository.AddAsync(
                            providerOrderItem,
                            cancellationToken);

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }

                    foreach (var serial in confirmItem.Serials)
                    {
                        var existingRedeem = await _redeemRepository.GetBySerialAsync(
                            serial,
                            cancellationToken);

                        if (existingRedeem is not null)
                            continue;

                        var redeem = new ProviderRedeem(
                            providerOrderItem.Id,
                            dtpOrder.OrderId,
                            dtpItem?.OrderItemId,
                            serial,
                            confirmItem.Sku);

                        await _redeemRepository.AddAsync(
                            redeem,
                            cancellationToken);

                        await _unitOfWork.SaveChangesAsync(cancellationToken);

                        var redeemRequest = new PeacomRedeemRequest
                        {
                            Serial = serial,
                            InfoCustomer = new PeacomInfoCustomerDto
                            {
                                CustomerEmail = dtpOrder.CustomerEmail,
                                CustomerName = dtpOrder.CustomerName ?? "",
                                CustomerPhone = dtpOrder.CustomerPhone ?? ""
                            }
                        };

                        try
                        {
                            var redeemResponse = await _peacomClient.RedeemAsync(
                                redeemRequest,
                                cancellationToken);

                            redeem.MarkRedeemRequested(
                                redeemResponse.Status,
                                redeemResponse.RawJson);
                        }
                        catch (Exception ex)
                        {
                            redeem.MarkFailed(ex.Message);
                        }

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                providerOrder.MarkFailed(ex.Message);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw;
            }
        }
    }
}
