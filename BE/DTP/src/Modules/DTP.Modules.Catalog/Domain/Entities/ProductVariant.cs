using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductVariant : EntityBase
    {
        public Guid ProductId { get; private set; }

        public string? Sku { get; private set; }

        public string Name { get; private set; } = default!;

        public decimal Price { get; private set; }

        public decimal? OriginalPrice { get; private set; }

        public int? DurationDays { get; private set; }

        public decimal? DataAmount { get; private set; }

        public string? DataUnit { get; private set; }

        public bool IsUnlimited { get; private set; }

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }

        private ProductVariant() { }

        public ProductVariant(
            Guid productId,
            string? sku,
            string name,
            decimal price,
            decimal? originalPrice,
            int? durationDays,
            decimal? dataAmount,
            string? dataUnit,
            bool isUnlimited,
            int sortOrder)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Sku = sku?.Trim();
            Name = name.Trim();
            Price = price;
            OriginalPrice = originalPrice;
            DurationDays = durationDays;
            DataAmount = dataAmount;
            DataUnit = dataUnit?.Trim();
            IsUnlimited = isUnlimited;
            SortOrder = sortOrder;
            IsActive = true;
        }

        public void Update(
            string? sku,
            string name,
            decimal price,
            decimal? originalPrice,
            int? durationDays,
            decimal? dataAmount,
            string? dataUnit,
            bool isUnlimited,
            int sortOrder,
            bool isActive)
        {
            Sku = sku?.Trim();
            Name = name.Trim();
            Price = price;
            OriginalPrice = originalPrice;
            DurationDays = durationDays;
            DataAmount = dataAmount;
            DataUnit = dataUnit?.Trim();
            IsUnlimited = isUnlimited;
            SortOrder = sortOrder;
            IsActive = isActive;

            SetUpdated();
        }
    }
}
