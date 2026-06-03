using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class Carrier : EntityBase
    {
        public string? Code { get;  set; }

        public string Name { get;  set; } = default!;

        public string Slug { get;  set; } = default!;

        public Guid CountryId { get;  set; }

        public string? LogoUrl { get;  set; }

        public int SortOrder { get;  set; }

        public bool IsActive { get;  set; }

        private Carrier()
        {
        }

        public Carrier(
            string? code,
            string name,
            string slug,
            Guid countryId,
            string? logoUrl,
            int sortOrder)
        {
            Id = Guid.NewGuid();
            Code = code?.Trim();
            Name = name.Trim();
            Slug = slug.Trim();
            CountryId = countryId;
            LogoUrl = logoUrl?.Trim();
            SortOrder = sortOrder;
            IsActive = true;
        }

        public void Update(
            string? code,
            string name,
            string slug,
            Guid countryId,
            string? logoUrl,
            int sortOrder,
            bool isActive)
        {
            Code = code?.Trim();
            Name = name.Trim();
            Slug = slug.Trim();
            CountryId = countryId;
            LogoUrl = logoUrl?.Trim();
            SortOrder = sortOrder;
            IsActive = isActive;

            SetUpdated();
        }
    }
}
