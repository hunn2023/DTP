using DTP.Modules.Catalog.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductContent : EntityBase
    {
        private ProductContent()
        {
        }

        public ProductContent(
            Guid productId,
            ProductContentType contentType,
            string title,
            string? summary,
            string bodyHtml,
            int sortOrder,
            bool isActive)
        {
            ProductId = productId;
            ContentType = contentType;
            Title = title.Trim();
            Summary = string.IsNullOrWhiteSpace(summary) ? null : summary.Trim();
            BodyHtml = bodyHtml.Trim();
            SortOrder = sortOrder;
            IsActive = isActive;
        }


        public Guid ProductId { get; private set; }

        public ProductContentType ContentType { get; private set; }

        public string Title { get; private set; } = null!;

        public string? Summary { get; private set; }

        public string BodyHtml { get; private set; } = null!;

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }


        public Product Product { get; private set; } = null!;

        public void Update(
            ProductContentType contentType,
            string title,
            string? summary,
            string bodyHtml,
            int sortOrder,
            bool isActive)
        {
            ContentType = contentType;
            Title = title.Trim();
            Summary = string.IsNullOrWhiteSpace(summary) ? null : summary.Trim();
            BodyHtml = bodyHtml.Trim();
            SortOrder = sortOrder;
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
    }
}
