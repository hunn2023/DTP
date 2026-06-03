using DTP.Shared.Domain;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class Provider : EntityBase
    {
        public string Code { get; private set; } = default!;
        public string Name { get; private set; } = default!;

        public string? ApiBaseUrl { get; private set; }
        public string? ApiKey { get; private set; }
        public string? ApiSecret { get; private set; }
        public string? WebhookUrl { get; private set; }
        public string? SupportEmail { get; private set; }
        public bool IsActive { get; private set; }

        private Provider() { }

        public Provider(
            string code,
            string name,
            string? apiBaseUrl,
            string? apiKey,
            string? apiSecret,
            string? webhookUrl,
            string? supportEmail)
        {
            Id = Guid.NewGuid();
            Code = code.Trim();
            Name = name.Trim();
            ApiBaseUrl = apiBaseUrl?.Trim();
            ApiKey = apiKey?.Trim();
            ApiSecret = apiSecret?.Trim();
            WebhookUrl = webhookUrl?.Trim();
            SupportEmail = supportEmail?.Trim();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string code,
            string name,
            string? apiBaseUrl,
            string? apiKey,
            string? apiSecret,
            string? webhookUrl,
            string? supportEmail,
            bool isActive)
        {
            Code = code.Trim();
            Name = name.Trim();
            ApiBaseUrl = apiBaseUrl?.Trim();
            ApiKey = apiKey?.Trim();
            ApiSecret = apiSecret?.Trim();
            WebhookUrl = webhookUrl?.Trim();
            SupportEmail = supportEmail?.Trim();
            IsActive = isActive;

            SetUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdated();
        }
    }
}
