using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ProviderCredential : EntityBase
    {
        private ProviderCredential()
        {
        }

        public ProviderCredential(
            Guid providerId,
            string key,
            string value,
            bool isEncrypted,
            bool isActive)
        {
            Id = Guid.NewGuid();
            ProviderId = providerId;
            Key = key.Trim();
            Value = value.Trim();
            IsEncrypted = isEncrypted;
            IsActive = isActive;
        }

        public Guid ProviderId { get; private set; }

        public ExternalProvider Provider { get; private set; } = default!;

        public string Key { get; private set; } = default!;

        public string Value { get; private set; } = default!;

        public bool IsEncrypted { get; private set; }

        public bool IsActive { get; private set; }

        public void Update(string value, bool isEncrypted, bool isActive)
        {
            Value = value.Trim();
            IsEncrypted = isEncrypted;
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
