using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Persistence.ReadModels
{
    public class ProductKnowledgeReadModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? LocationText { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
