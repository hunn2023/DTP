using DTP.Modules.Provider.Domain.Enums.DTP.Modules.Provider.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class ExternalProvider : EntityBase
    {
        private ExternalProvider()
        {
        }

        public ExternalProvider(
            string code,
            string name,
            ProviderType type,
            string? apiBaseUrl,
            bool isActive)
        {
            Id = Guid.NewGuid();
            Code = code.Trim();
            Name = name.Trim();
            Type = type;
            ApiBaseUrl = apiBaseUrl?.Trim();
            IsActive = isActive;
        }

        public string Code { get; private set; } = default!;

        public string Name { get; private set; } = default!;

        public ProviderType Type { get; private set; }

        public string? ApiBaseUrl { get; private set; }

        public bool IsActive { get; private set; }

        public string? Description { get; private set; }

        public ICollection<ProviderCredential> Credentials { get; private set; } = new List<ProviderCredential>();

        public ICollection<ProviderProductMapping> ProductMappings { get; private set; } = new List<ProviderProductMapping>();

        public void Update(
            string name,
            ProviderType type,
            string? apiBaseUrl,
            bool isActive,
            string? description)
        {
            Name = name.Trim();
            Type = type;
            ApiBaseUrl = apiBaseUrl?.Trim();
            IsActive = isActive;
            Description = description?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
