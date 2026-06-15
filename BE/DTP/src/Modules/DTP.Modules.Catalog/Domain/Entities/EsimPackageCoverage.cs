using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class EsimPackageCoverage : EntityBase
    {
        public Guid Id { get; private set; }

        public Guid EsimPackageId { get; private set; }
        public Guid CountryId { get; private set; }

        public string CountryCode { get; private set; } = default!;
        public string CountryName { get; private set; } = default!;

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public EsimPackage? EsimPackage { get; private set; }
        public Country? Country { get; private set; }

        private EsimPackageCoverage()
        {
        }

        public EsimPackageCoverage(
            Guid esimPackageId,
            Guid countryId,
            string countryCode,
            string countryName,
            bool isActive = false)
        {
            Id = Guid.NewGuid();
            EsimPackageId = esimPackageId;
            CountryId = countryId;
            CountryCode = NormalizeCountryCode(countryCode);
            CountryName = countryName.Trim();
            IsActive = isActive;
            CreatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private static string NormalizeCountryCode(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("CountryCode không được rỗng.");

            return countryCode.Trim().ToUpperInvariant();
        }
    }
}
