using DTP.Shared.Domain;


namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductFaq : EntityBase
    {
        private ProductFaq()
        {
        }

        public ProductFaq(
            Guid productId,
            string question,
            string answer,
            int sortOrder,
            bool isActive)
        {
            ProductId = productId;
            Question = question.Trim();
            Answer = answer.Trim();
            SortOrder = sortOrder;
            IsActive = isActive;
        }


        public Guid ProductId { get; private set; }

        public string Question { get; private set; } = null!;

        public string Answer { get; private set; } = null!;

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }


        public Product Product { get; private set; } = null!;

        public void Update(
            string question,
            string answer,
            int sortOrder,
            bool isActive)
        {
            Question = question.Trim();
            Answer = answer.Trim();
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
