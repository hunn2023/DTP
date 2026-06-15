using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderOrder : EntityBase
    {
        public Guid ProviderId { get; private set; }

        public Guid DtpOrderId { get; private set; }

        public string ProviderOrderPublicId { get; private set; } = default!;

        public decimal Amount { get; private set; }

        public int NumOfProduct { get; private set; }

        public int ProviderStatus { get; private set; }

        public string Status { get; private set; } = default!;
        // Created, Confirmed, Processing, Done, Failed, Cancelled, Expired

        public DateTime ReservedUntil { get; private set; }

        public string? RawCreateResponseJson { get; private set; }

        public string? RawConfirmResponseJson { get; private set; }

        public string? ErrorMessage { get; private set; }

        private readonly List<ProviderOrderItem> _items = new();
        public IReadOnlyCollection<ProviderOrderItem> Items => _items.AsReadOnly();

        private ProviderOrder()
        {
        }

        public ProviderOrder(
            Guid providerId,
            Guid dtpOrderId,
            string providerOrderPublicId,
            decimal amount,
            int numOfProduct,
            int providerStatus,
            string? rawCreateResponseJson)
        {
            Id = Guid.NewGuid();
            ProviderId = providerId;
            DtpOrderId = dtpOrderId;
            ProviderOrderPublicId = providerOrderPublicId;
            Amount = amount;
            NumOfProduct = numOfProduct;
            ProviderStatus = providerStatus;
            Status = "Created";
            ReservedUntil = DateTime.UtcNow.AddMinutes(15);
            RawCreateResponseJson = rawCreateResponseJson;
            CreatedAt = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ReservedUntil;
        }

        public void MarkConfirmed(
            decimal amount,
            int numOfProduct,
            int providerStatus,
            string? rawConfirmResponseJson)
        {
            Amount = amount;
            NumOfProduct = numOfProduct;
            ProviderStatus = providerStatus;
            RawConfirmResponseJson = rawConfirmResponseJson;
            Status = MapOrderStatus(providerStatus);
            ErrorMessage = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkExpired()
        {
            Status = "Expired";
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string errorMessage)
        {
            Status = "Failed";
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddItem(ProviderOrderItem item)
        {
            _items.Add(item);
        }

        private static string MapOrderStatus(int status)
        {
            return status switch
            {
                1 => "Cancelled",
                2 => "Confirmed",
                3 => "Processing",
                4 => "Failed",
                6 => "Done",
                _ => "Processing"
            };
        }
    }
}
