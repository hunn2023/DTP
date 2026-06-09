using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductVariantFeature : EntityBase
    {
        private ProductVariantFeature()
        {
        }

        public ProductVariantFeature(
            Guid productVariantId,
            string text,
            string? icon,
            int sortOrder,
            bool isActive)
        {
            Id = Guid.NewGuid();
            ProductVariantId = productVariantId;
            Text = text;
            Icon = icon;
            SortOrder = sortOrder;
            IsActive = isActive;
        }

        public Guid ProductVariantId { get; private set; }

        public ProductVariant ProductVariant { get; private set; } = default!;

        public string Text { get; private set; } = default!;

        public string? Icon { get; private set; }

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }

        public void Update(
            string text,
            string? icon,
            int sortOrder,
            bool isActive)
        {
            Text = text;
            Icon = icon;
            SortOrder = sortOrder;
            IsActive = isActive;
        }
    }
}
