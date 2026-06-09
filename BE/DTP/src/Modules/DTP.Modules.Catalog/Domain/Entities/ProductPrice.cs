using DTP.Shared.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductPrice : EntityBase
    {
        public Guid ProductId { get; private set; }
        public Guid? ProductVariantId { get; private set; }

        public string Currency { get; private set; } = "VND";
        public decimal OriginalPrice { get; private set; }
        public decimal SalePrice { get; private set; }
        public decimal CostPrice { get; private set; }

        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public bool IsActive { get; private set; }
        public string PriceType { get; private set; } = "Retail";

        public int Priority { get; private set; }

        public string? Note { get; private set; }

        private ProductPrice() { }

        public ProductPrice(
            Guid productId,
            Guid? productVariantId,
            string currency,
            decimal originalPrice,
            decimal salePrice,
            decimal costPrice,
            DateTime? startDate,
            DateTime? endDate,
            string note)
        {
            ProductId = productId;
            ProductVariantId = productVariantId;
            Currency = currency.Trim().ToUpper();
            OriginalPrice = originalPrice;
            SalePrice = salePrice;
            CostPrice = costPrice;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = true;
            Note = note;
        }

        public void Update(
            string currency,
            decimal originalPrice,
            decimal salePrice,
            decimal costPrice,
            DateTime? startDate,
            DateTime? endDate,
            bool isActive,
            string note)
        {
            Currency = currency.Trim().ToUpper();
            OriginalPrice = originalPrice;
            SalePrice = salePrice;
            CostPrice = costPrice;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = isActive;
            Note = note;
            SetUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdated();
        }
    }
}
