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
        public ProviderRedeemPollingService(
            IProviderRedeemRepository redeemRepository,
            IPeacomProviderClient peacomClient,
            IProviderDeliveryEmailService emailService,
            IProviderUnitOfWork unitOfWork,
            ILogger<ProviderRedeemPollingService> logger,
            IProviderRepository providerRepository)
        {
            _redeemRepository = redeemRepository;
            _peacomClient = peacomClient;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _providerRepository = providerRepository;
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
             "Bluecom",
             cancellationToken);

            if (provider is null)
                throw new InvalidOperationException("Provider PEACOM chưa được cấu hình.");

            if (!provider.IsActive)
                throw new InvalidOperationException("Provider PEACOM đang inactive.");


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

            var redeems = await _redeemRepository.GetDoneNotEmailSentAsync(
                take,
                cancellationToken);

            if (redeems.Count == 0)
            {
                _logger.LogDebug("No done provider redeems waiting for email.");
                return;
            }

            foreach (var redeem in redeems)
            {
                try
                {
                    _logger.LogInformation(
                        "Sending provider redeem email. Serial={Serial}, ProductType={ProductType}",
                        redeem.Serial,
                        redeem.ProductType);

                    if (redeem.ProductType == 1)
                    {
                        if (string.IsNullOrWhiteSpace(redeem.QrCodeUrl) &&
                            string.IsNullOrWhiteSpace(redeem.ActivationCode))
                        {
                            _logger.LogWarning(
                                "Redeem is DONE but missing eSIM QR/ActivationCode. Serial={Serial}",
                                redeem.Serial);

                            continue;
                        }

                        await _emailService.SendEsimEmailAsync(
                            redeem,
                            cancellationToken);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(redeem.PolicyUrl) &&
                            string.IsNullOrWhiteSpace(redeem.PolicyNumber))
                        {
                            _logger.LogWarning(
                                "Redeem is DONE but missing insurance policy info. Serial={Serial}",
                                redeem.Serial);

                            continue;
                        }

                        await _emailService.SendInsuranceEmailAsync(
                            redeem,
                            cancellationToken);
                    }

                    redeem.MarkEmailSent();

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation(
                        "Provider redeem email sent. Serial={Serial}",
                        redeem.Serial);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // Không MarkFailed để lần sau gửi email lại.
                    _logger.LogError(
                        ex,
                        "Send provider redeem email failed. Serial={Serial}",
                        redeem.Serial);
                }
            }
        }
    }
}
