using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Domain.Entities
{
    public class EsimProfile : EntityBase
    {
        private EsimProfile()
        {
        }

        public EsimProfile(
            Guid productId,
            Guid? productVariantId,
            Guid? esimPackageId,
            Guid? providerId,
            string iccid,
            string? imsi,
            string? msisdn,
            string activationCode,
            string? qrCodeUrl,
            string? qrContent,
            string? smdpAddress,
            string? matchingId,
            DateTime? expiredAt)
        {
            Id = Guid.NewGuid();

            ProductId = productId;
            ProductVariantId = productVariantId;
            EsimPackageId = esimPackageId;
            ProviderId = providerId;

            Iccid = iccid;
            Imsi = imsi;
            Msisdn = msisdn;
            ActivationCode = activationCode;
            QrCodeUrl = qrCodeUrl;
            QrContent = qrContent;
            SmdpAddress = smdpAddress;
            MatchingId = matchingId;

            Status = EsimProfileStatus.Available;
            ExpiredAt = expiredAt;

            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid ProductId { get; private set; }
        public Guid? ProductVariantId { get; private set; }
        public Guid? EsimPackageId { get; private set; }
        public Guid? ProviderId { get; private set; }

        public Guid? OrderId { get; private set; }
        public Guid? OrderItemId { get; private set; }

        public string Iccid { get; private set; } = default!;
        public string? Imsi { get; private set; }
        public string? Msisdn { get; private set; }

        public string ActivationCode { get; private set; } = default!;
        public string? QrCodeUrl { get; private set; }
        public string? QrContent { get; private set; }

        public string? SmdpAddress { get; private set; }
        public string? MatchingId { get; private set; }

        public EsimProfileStatus Status { get; private set; }

        public DateTime? AssignedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? ActivatedAt { get; private set; }
        public DateTime? ExpiredAt { get; private set; }

        public void AssignToOrder(Guid orderId, Guid orderItemId)
        {
            if (Status != EsimProfileStatus.Available)
                throw new InvalidOperationException("eSIM profile is not available.");

            OrderId = orderId;
            OrderItemId = orderItemId;
            Status = EsimProfileStatus.Assigned;
            AssignedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkDelivered()
        {
            if (Status != EsimProfileStatus.Assigned &&
                Status != EsimProfileStatus.Available)
            {
                throw new InvalidOperationException("Only assigned eSIM can be delivered.");
            }

            Status = EsimProfileStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkActivated()
        {
            Status = EsimProfileStatus.Activated;
            ActivatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkExpired()
        {
            Status = EsimProfileStatus.Expired;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            Status = EsimProfileStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
