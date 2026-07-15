using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Persistence.ReadModels
{
    public class ProductContentKnowledgeReadModel
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Slug { get; set; }

        public string? Summary { get; set; }

        public string? BodyHtml { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
