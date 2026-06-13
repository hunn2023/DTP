using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Domain.Entities
{
    public class ContentFaq : EntityBase
    {
        private ContentFaq()
        {
        }

        public ContentFaq(
            string question,
            string answer,
            string? categoryCode,
            int sortOrder,
            bool isActive)
        {
            Id = Guid.NewGuid();
            Question = question.Trim();
            Answer = answer.Trim();
            CategoryCode = categoryCode?.Trim();
            SortOrder = sortOrder;
            IsActive = isActive;
            CreatedAt = DateTime.UtcNow;
        }

        public string Question { get; private set; } = default!;
        public string Answer { get; private set; } = default!;
        public string? CategoryCode { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }

        public void Update(
            string question,
            string answer,
            string? categoryCode,
            int sortOrder,
            bool isActive)
        {
            Question = question.Trim();
            Answer = answer.Trim();
            CategoryCode = categoryCode?.Trim();
            SortOrder = sortOrder;
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Disable()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Enable()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
