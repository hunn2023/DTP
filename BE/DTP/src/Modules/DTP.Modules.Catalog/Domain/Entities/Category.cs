using DTP.Shared.Domain;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class Category : EntityBase
    {
        public string? Code { get; set; }

        public string Name { get;  set; } = default!;

        public string Slug { get;  set; } = default!;

        public Guid? ParentId { get;  set; }

        public int SortOrder { get;  set; }

        public bool IsActive { get;  set; }

        public Category() { }


        public Category(
            string code,
            string name,
            string slug,
            Guid? parentId,
            int sortOrder)
        {
            Code = code.Trim();
            Name = name.Trim();
            Slug = slug.Trim();
            ParentId = parentId;
            SortOrder = sortOrder;
            IsActive = true;
        }

        public void Update(
            string code,
            string name,
            string slug,
            Guid? parentId,
            int sortOrder,
            bool isActive)
        {
            Code = code.Trim();
            Name = name.Trim();
            Slug = slug.Trim();
            ParentId = parentId;
            SortOrder = sortOrder;
            IsActive = isActive;

            SetUpdated();
        }
    }
}
