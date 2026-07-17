using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderRedeem : EntityBase
    {
        public Guid ProviderOrderItemId { get; private set; }

        public Guid DtpOrderId { get; private set; }

        public Guid? DtpOrderItemId { get; private set; }

        public string Serial { get; private set; } = default!;

        public string Sku { get; private set; } = default!;

        public int RedeemStatus { get; private set; }
        // 0 INIT, 1 PROCESSING, 2 DONE, 3 FAIL

        public string Status { get; private set; } = default!;
        // Init, Processing, Done, Failed

        public int? ProductType { get; private set; }
        // 1 = eSIM, các type khác tùy Peacom

        public string? PackageName { get; private set; }

        public string? Model { get; private set; }

        public int RedeemInfoCallCount { get; private set; }

        public DateTime? LastRedeemInfoCallAt { get; private set; }


        // eSIM result
        public string? Iccid { get; private set; }

        public string? Imsi { get; private set; }

        public string? ActivationCode { get; private set; }

        public string? QrCodeUrl { get; private set; }

        public string? ShortUrlApple { get; private set; }

        public string? ShortUrlAndroid { get; private set; }

        public string? Apn { get; private set; }

        // Insurance result
        public string? PolicyNumber { get; private set; }

        public string? PolicyUrl { get; private set; }

        public string? PolicyCertificate { get; private set; }

        public bool EmailSent { get; private set; }

        public DateTime? EmailSentAt { get; private set; }

        public string? RawRedeemResponseJson { get; private set; }

        public string? RawRedeemInfoJson { get; private set; }

        public string? ErrorMessage { get; private set; }

        public DateTime? LastCheckedAt { get; private set; }


        public ProviderOrderItem? ProviderOrderItem { get; private set; }

        private ProviderRedeem()
        {
        }

        public ProviderRedeem(
            Guid providerOrderItemId,
            Guid dtpOrderId,
            Guid? dtpOrderItemId,
            string serial,
            string sku)
        {
            Id = Guid.NewGuid();
            ProviderOrderItemId = providerOrderItemId;
            DtpOrderId = dtpOrderId;
            DtpOrderItemId = dtpOrderItemId;
            Serial = serial;
            Sku = sku;
            RedeemStatus = 0;
            Status = "Init";
            RedeemInfoCallCount = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkRedeemRequested(
            int redeemStatus,
            string? rawResponseJson)
        {
            RedeemStatus = redeemStatus;
            Status = MapRedeemStatus(redeemStatus);
            RawRedeemResponseJson = rawResponseJson;
            ErrorMessage = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkRedeemInfo(
            int redeemStatus,
            int? productType,
            string? packageName,
            string? model,
            string? rawInfoJson)
        {
            RedeemStatus = redeemStatus;
            Status = MapRedeemStatus(redeemStatus);
            ProductType = productType;
            PackageName = packageName;
            Model = model;
            RawRedeemInfoJson = rawInfoJson;
            LastCheckedAt = DateTime.UtcNow;
            ErrorMessage = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetEsimResult(
            string? iccid,
            string? imsi,
            string? activationCode,
            string? qrCodeUrl,
            string? shortUrlApple,
            string? shortUrlAndroid,
            string? apn)
        {
            Iccid = iccid;
            Imsi = imsi;
            ActivationCode = activationCode;
            QrCodeUrl = qrCodeUrl;
            ShortUrlApple = shortUrlApple;
            ShortUrlAndroid = shortUrlAndroid;
            Apn = apn;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetInsuranceResult(
            string? policyNumber,
            string? policyUrl,
            string? policyCertificate)
        {
            PolicyNumber = policyNumber;
            PolicyUrl = policyUrl;
            PolicyCertificate = policyCertificate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string errorMessage)
        {
            RedeemStatus = 3;
            Status = "Failed";
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkEmailSent()
        {
            EmailSent = true;
            EmailSentAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        private static string MapRedeemStatus(int status)
        {
            return status switch
            {
                0 => "Init",
                1 => "Processing",
                2 => "Done",
                3 => "Failed",
                _ => "Init"
            };
        }

        public const int MaxRedeemInfoCallCount = 5;
        public bool CanCallRedeemInfo()
        {
            return RedeemStatus != 2 &&
                   RedeemInfoCallCount < MaxRedeemInfoCallCount;
        }

        public void MarkRedeemInfoCalled()
        {
            if (RedeemStatus == 2)
                throw new InvalidOperationException(
                    "Redeem đã hoàn thành, không cần gọi lại API.");

            if (RedeemInfoCallCount >= MaxRedeemInfoCallCount)
                throw new InvalidOperationException(
                    $"Đã đạt số lần gọi API tối đa: {MaxRedeemInfoCallCount} lần.");

            RedeemInfoCallCount++;
            LastRedeemInfoCallAt = DateTime.UtcNow;
            LastCheckedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }


        public void MarkRedeemInfoError(string errorMessage)
        {
            ErrorMessage = errorMessage;
            LastCheckedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            if (RedeemInfoCallCount >= MaxRedeemInfoCallCount)
            {
                RedeemStatus = 3;
                Status = "Failed";
            }
            else
            {
                RedeemStatus = 1;
                Status = "Processing";
            }
        }

    }
}
