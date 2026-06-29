using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Clients;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Services
{
    public class ProviderRedeemPollingService : IProviderRedeemPollingService
    {
        private readonly IProviderRedeemRepository _redeemRepository;
        private readonly IPeacomProviderClient _peacomClient;
        private readonly IProviderDeliveryEmailService _emailService;
        private readonly IProviderUnitOfWork _unitOfWork;
        private readonly ILogger<ProviderRedeemPollingService> _logger;
        private readonly IProviderRepository _providerRepository;
        private readonly IDeliveryService _deliveryService;
        private const string ProviderCode = "BLUECOM";
        private const string WorkerIpAddress = "worker";
        public ProviderRedeemPollingService(
            IProviderRedeemRepository redeemRepository,
            IPeacomProviderClient peacomClient,
            IProviderDeliveryEmailService emailService,
            IProviderUnitOfWork unitOfWork,
            ILogger<ProviderRedeemPollingService> logger,
            IProviderRepository providerRepository,
            IDeliveryService deliveryService)
        {
            _redeemRepository = redeemRepository;
            _peacomClient = peacomClient;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _providerRepository = providerRepository;
            _deliveryService = deliveryService;
        }

        public async Task PollPendingRedeemsAsync(
            int take,
            CancellationToken cancellationToken = default)
        {
            if (take <= 0)
                take = 20;

            var redeems = await _redeemRepository.GetPendingAsync(
                take,
                cancellationToken);

            if (redeems.Count == 0)
            {
                _logger.LogDebug("No pending provider redeems found.");
                return;
            }

            var provider = await _providerRepository.GetByCodeAsync(
             ProviderCode,
             cancellationToken);

            if (provider is null)
                throw new InvalidOperationException($"Provider {ProviderCode} chưa được cấu hình.");

            foreach (var redeem in redeems)
            {
                try
                {
                    _logger.LogInformation(
                        "Polling redeem info. Serial={Serial}, Status={Status}",
                        redeem.Serial,
                        redeem.Status);

                    var info = await _peacomClient.GetRedeemInfoAsync(
                        provider,
                        redeem.Serial,
                        cancellationToken);

                    redeem.MarkRedeemInfo(
                        redeemStatus: info.Status,
                        productType: info.ProductType,
                        packageName: info.PackageName,
                        model: info.Model,
                        rawInfoJson: info.RawJson);

                    if (info.Status == 2)
                    {
                        var data = info.Data.FirstOrDefault();

                        if (data is null)
                        {
                            redeem.MarkFailed("Peacom redeem DONE nhưng không có data.");
                        }
                        else if (info.ProductType == 1 || data.ProductType == 1)
                        {
                            redeem.SetEsimResult(
                                iccid: data.Iccid,
                                imsi: data.Imsi,
                                activationCode: data.Ac,
                                qrCodeUrl: data.QrCodeUrl,
                                shortUrlApple: data.ShortUrlApple,
                                shortUrlAndroid: data.ShortUrlAndroid,
                                apn: data.Apn);
                        }
                        else
                        {
                            redeem.SetInsuranceResult(
                                policyNumber: data.PolicyNumber,
                                policyUrl: data.UrlResp,
                                policyCertificate: data.PolicyGCN);
                        }
                    }
                    else if (info.Status == 3)
                    {
                        redeem.MarkFailed("Peacom redeem status = FAIL.");
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // Không mark Failed ngay vì có thể chỉ là lỗi mạng tạm thời.
                    // Worker lần sau sẽ thử lại.
                    _logger.LogWarning(
                        ex,
                        "Poll redeem info failed. Serial={Serial}",
                        redeem.Serial);
                }
            }
        }

        public async Task SendDoneRedeemEmailsAsync(
      int take,
      CancellationToken cancellationToken = default)
        {
            if (take <= 0)
                take = 20;

            var orderIds = await _redeemRepository.GetOrderIdsReadyForDeliveryAsync(
                take,
                cancellationToken);

            if (orderIds.Count == 0)
            {
                _logger.LogDebug("No provider redeem orders ready for delivery.");
                return;
            }

            foreach (var orderId in orderIds)
            {
                try
                {
                    _logger.LogInformation(
                        "Processing delivery for provider redeem order. OrderId={OrderId}",
                        orderId);

                    var deliveryId = await GetOrCreateDeliveryIdAsync(
                        orderId,
                        cancellationToken);

                    if (deliveryId == null)
                    {
                        _logger.LogWarning(
                            "Cannot create or get delivery for order. OrderId={OrderId}",
                            orderId);

                        continue;
                    }

                    var processResult = await _deliveryService.ProcessAsync(
                        deliveryId.Value,
                        WorkerIpAddress,
                        cancellationToken);

                    if (!processResult.IsSuccess)
                    {
                        await _deliveryService.MarkFailedAsync(
                            deliveryId.Value,
                            processResult.Error ?? "Delivery process failed.",
                            cancellationToken);

                        _logger.LogWarning(
                            "Delivery process failed. OrderId={OrderId}, DeliveryId={DeliveryId}, Error={Error}",
                            orderId,
                            deliveryId.Value,
                            processResult.Error);

                        continue;
                    }

                    await _redeemRepository.MarkEmailSentByOrderIdAsync(
                        orderId,
                        cancellationToken);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation(
                        "Delivery processed successfully for provider redeem order. OrderId={OrderId}, DeliveryId={DeliveryId}",
                        orderId,
                        deliveryId.Value);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Process delivery failed for provider redeem order. OrderId={OrderId}",
                        orderId);
                }
            }
        }

        private async Task<Guid?> GetOrCreateDeliveryIdAsync(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var existingDeliveryResult = await _deliveryService.GetByOrderIdAsync(
                orderId,
                cancellationToken);

            if (existingDeliveryResult.IsSuccess &&
                existingDeliveryResult.Data != null)
            {
                return existingDeliveryResult.Data.Id;
            }

            var createResult = await _deliveryService.CreatePendingAsync(
                orderId,
                WorkerIpAddress,
                cancellationToken);

            if (!createResult.IsSuccess)
            {
                _logger.LogWarning(
                    "Create pending delivery failed. OrderId={OrderId}, Error={Error}",
                    orderId,
                    createResult.Error);

                return null;
            }

            return createResult.Data;
        }
    }
}
