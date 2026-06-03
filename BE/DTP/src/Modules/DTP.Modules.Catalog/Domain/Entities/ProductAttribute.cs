using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductAttribute : EntityBase
    {
        public Guid ProductId { get; private set; }

        public string Name { get; private set; } = default!;

        public string Value { get; private set; } = default!;

        public int SortOrder { get; private set; }

        private ProductAttribute() { }

        public ProductAttribute(
            Guid productId,
            string name,
            string value,
            int sortOrder)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Name = name.Trim();
            Value = value.Trim();
            SortOrder = sortOrder;
        }

        public void Update(
            string name,
            string value,
            int sortOrder)
        {
            Name = name.Trim();
            Value = value.Trim();
            SortOrder = sortOrder;

            SetUpdated();
        }
    }
}
