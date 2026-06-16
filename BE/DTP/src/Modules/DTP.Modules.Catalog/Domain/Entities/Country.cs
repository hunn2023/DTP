using DTP.Shared.Domain;


namespace DTP.Modules.Catalog.Domain.Entities
{
    public class Country : EntityBase
    {
        public string Code { get; private set; } = default!;

        public string Name { get; private set; } = default!;

        public string Slug { get; private set; } = default!;

        public string? FlagUrl { get; private set; }

        public string? FlagKey { get; private set; }

        public string? Region { get; private set; }

        public string? Description { get; private set; }

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }

        private Country()
        {
        }

        public Country(
            string code,
            string name,
            string slug,
            string? flagUrl,
            string? region,
            string? description,
            int sortOrder,
            bool isActive)
        {
            Id = Guid.NewGuid();
            Code = code;
            Name = name;
            Slug = slug;
            FlagUrl = flagUrl;
            Region = region;
            Description = description;
            SortOrder = sortOrder;
            IsActive = isActive;
        }

       

        public void Update(
            string code,
            string name,
            string slug,
            string? flagUrl,
            string? region,
            string? description,
            int sortOrder,
            bool isActive)
        {
            Code = code;
            Name = name;
            Slug = slug;
            FlagUrl = flagUrl;
            Region = region;
            Description = description;
            SortOrder = sortOrder;
            IsActive = isActive;
        }
     
        public void Update(
            string code,
            string name,
            string slug,
            string? flagUrl,
            int sortOrder,
            bool isActive)
        {
            Code = code.Trim().ToUpper();
            Name = name.Trim();
            Slug = slug.Trim().ToLower();
            FlagUrl = flagUrl?.Trim();
            SortOrder = sortOrder;
            IsActive = isActive;

            SetUpdated();
        }

        public void UpdateFlag(string flagUrl, string flagKey)
        {
            FlagUrl = flagUrl;
            FlagKey = flagKey;
            UpdatedAt = DateTime.Now;
        }

        public void Activate()
        {
            IsActive = true;
            SetUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdated();
        }
    }
}