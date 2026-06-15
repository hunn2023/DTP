using DTP.Shared.Domain;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderPackageProduct : EntityBase
    {

        public Guid ProviderId { get; private set; }

        public string ProviderSku { get; private set; } = default!;
        public string? ProviderProductId { get; private set; }

        public string Name { get; private set; } = default!;
        public string? Model { get; private set; }

        public string? Regional { get; private set; }
        public int? RegionId { get; private set; }

        public decimal Price { get; private set; }
        public string CurrencyCode { get; private set; } = "VND";

        public string? ImageUrl { get; private set; }

        public string SyncStatus { get; private set; } = default!;
        // Synced, DetailSynced, Provisioned, Failed, Activated

        public string? ErrorMessage { get; private set; }

        public string RawPackageJson { get; private set; } = default!;
        public string? RawDetailJson { get; private set; }

        public DateTime LastSyncedAt { get; private set; }


        private ProviderPackageProduct()
        {
        }

        public ProviderPackageProduct(
            Guid providerId,
            string providerSku,
            string? providerProductId,
            string name,
            string? model,
            string? regional,
            int? regionId,
            decimal price,
            string currencyCode,
            string? imageUrl,
            string rawPackageJson)
        {
            Id = Guid.NewGuid();
            ProviderId = providerId;
            ProviderSku = providerSku.Trim();
            ProviderProductId = providerProductId;
            Name = name.Trim();
            Model = model;
            Regional = regional;
            RegionId = regionId;
            Price = price;
            CurrencyCode = string.IsNullOrWhiteSpace(currencyCode)
                ? "VND"
                : currencyCode.Trim().ToUpperInvariant();
            ImageUrl = imageUrl;
            RawPackageJson = rawPackageJson;
            SyncStatus = "Synced";
            LastSyncedAt = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdatePackageInfo(
            string? providerProductId,
            string name,
            string? model,
            string? regional,
            int? regionId,
            decimal price,
            string currencyCode,
            string? imageUrl,
            string rawPackageJson)
        {
            ProviderProductId = providerProductId;
            Name = name.Trim();
            Model = model;
            Regional = regional;
            RegionId = regionId;
            Price = price;
            CurrencyCode = string.IsNullOrWhiteSpace(currencyCode)
                ? "VND"
                : currencyCode.Trim().ToUpperInvariant();
            ImageUrl = imageUrl;
            RawPackageJson = rawPackageJson;

            SyncStatus = "Synced";
            ErrorMessage = null;
            LastSyncedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkDetailSynced(string rawDetailJson)
        {
            RawDetailJson = rawDetailJson;
            SyncStatus = "DetailSynced";
            ErrorMessage = null;
            LastSyncedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkProvisioned()
        {
            SyncStatus = "Provisioned";
            ErrorMessage = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkActivated()
        {
            SyncStatus = "Activated";
            ErrorMessage = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkFailed(string errorMessage)
        {
            SyncStatus = "Failed";
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
