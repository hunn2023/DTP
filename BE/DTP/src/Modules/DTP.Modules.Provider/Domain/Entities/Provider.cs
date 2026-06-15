using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Domain.Entities
{
    public class Provider : EntityBase
    {

        public string Code { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string ProviderType { get; private set; } = default!; // ESIM, INSURANCE

        public string? BaseUrl { get; private set; }
        public string? ApiKey { get; private set; }
        public string? SecretKey { get; private set; }

        public bool IsActive { get; private set; }


        private Provider()
        {
        }

        public Provider(
            string code,
            string name,
            string providerType,
            string? baseUrl,
            string? apiKey,
            string? secretKey,
            bool isActive = true)
        {
            Code = NormalizeCode(code);
            Name = name.Trim();
            ProviderType = providerType.Trim().ToUpperInvariant();
            BaseUrl = baseUrl;
            ApiKey = apiKey;
            SecretKey = secretKey;
            IsActive = isActive;
        }

        public void Update(
            string name,
            string? baseUrl,
            string? apiKey,
            string? secretKey,
            bool isActive)
        {
            Name = name.Trim();
            BaseUrl = baseUrl;
            ApiKey = apiKey;
            SecretKey = secretKey;
            IsActive = isActive;
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

        private static string NormalizeCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Mã provider không được rỗng.");

            return code.Trim().ToUpperInvariant();
        }
    }
}
