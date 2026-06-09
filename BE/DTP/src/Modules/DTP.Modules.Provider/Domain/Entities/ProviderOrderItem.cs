using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderOrderItem : EntityBase
    {
        private ProviderOrderItem()
        {
        }

        public ProviderOrderItem(
            Guid providerOrderId,
            Guid orderItemId,
            Guid productId,
            Guid productVariantId,
            string providerProductCode,
            int quantity)
        {
            Id = Guid.NewGuid();
            ProviderOrderId = providerOrderId;
            OrderItemId = orderItemId;
            ProductId = productId;
            ProductVariantId = productVariantId;
            ProviderProductCode = providerProductCode.Trim();
            Quantity = quantity;
        }

        public Guid ProviderOrderId { get; private set; }

        public ProviderOrder ProviderOrder { get; private set; } = default!;

        public Guid OrderItemId { get; private set; }

        public Guid ProductId { get; private set; }

        public Guid ProductVariantId { get; private set; }

        public string ProviderProductCode { get; private set; } = default!;

        public int Quantity { get; private set; }

        public string? Iccid { get; private set; }

        public string? Msisdn { get; private set; }

        public string? QrCodeUrl { get; private set; }

        public string? QrCodeText { get; private set; }

        public string? ActivationCode { get; private set; }

        public string? Serial { get; private set; }

        public string? PinCode { get; private set; }

        public DateTime? ExpiredAt { get; private set; }

        public void SetEsimResult(
            string? iccid,
            string? msisdn,
            string? qrCodeUrl,
            string? qrCodeText,
            string? activationCode,
            DateTime? expiredAt)
        {
            Iccid = iccid;
            Msisdn = msisdn;
            QrCodeUrl = qrCodeUrl;
            QrCodeText = qrCodeText;
            ActivationCode = activationCode;
            ExpiredAt = expiredAt;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPhoneCardResult(
            string? serial,
            string? pinCode,
            DateTime? expiredAt)
        {
            Serial = serial;
            PinCode = pinCode;
            ExpiredAt = expiredAt;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
