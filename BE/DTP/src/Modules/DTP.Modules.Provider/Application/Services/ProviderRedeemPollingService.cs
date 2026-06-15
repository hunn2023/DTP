using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Clients;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.Abstractions.Services;
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

        public ProviderRedeemPollingService(
            IProviderRedeemRepository redeemRepository,
            IPeacomProviderClient peacomClient,
            IProviderDeliveryEmailService emailService,
            IProviderUnitOfWork unitOfWork)
        {
            _redeemRepository = redeemRepository;
            _peacomClient = peacomClient;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task PollPendingRedeemsAsync(
            int take,
            CancellationToken cancellationToken = default)
        {
            var redeems = await _redeemRepository.GetPendingAsync(
                take,
                cancellationToken);

            foreach (var redeem in redeems)
            {
                try
                {
                    var info = await _peacomClient.GetRedeemInfoAsync(
                        redeem.Serial,
                        cancellationToken);

                    redeem.MarkRedeemInfo(
                        info.Status,
                        info.ProductType,
                        info.PackageName,
                        info.Model,
                        info.RawJson);

                    var data = info.Data.FirstOrDefault();

                    if (data is not null && info.Status == 2)
                    {
                        if (info.ProductType == 1)
                        {
                            redeem.SetEsimResult(
                                data.Iccid,
                                data.Imsi,
                                data.Ac,
                                data.QrCodeUrl,
                                data.ShortUrlApple,
                                data.ShortUrlAndroid,
                                data.Apn);
                        }
                        else
                        {
                            redeem.SetInsuranceResult(
                                data.PolicyNumber,
                                data.UrlResp,
                                data.PolicyGCN);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    redeem.MarkFailed(ex.Message);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public async Task SendDoneRedeemEmailsAsync(
            int take,
            CancellationToken cancellationToken = default)
        {
            var redeems = await _redeemRepository.GetDoneNotEmailSentAsync(
                take,
                cancellationToken);

            foreach (var redeem in redeems)
            {
                try
                {
                    if (redeem.ProductType == 1)
                    {
                        await _emailService.SendEsimEmailAsync(
                            redeem,
                            cancellationToken);
                    }
                    else
                    {
                        await _emailService.SendInsuranceEmailAsync(
                            redeem,
                            cancellationToken);
                    }

                    redeem.MarkEmailSent();

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    redeem.MarkFailed($"Gửi email thất bại: {ex.Message}");
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
