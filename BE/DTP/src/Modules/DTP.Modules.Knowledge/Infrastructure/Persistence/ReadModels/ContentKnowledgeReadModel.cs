using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Persistence.ReadModels
{
    public class ContentKnowledgeReadModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string? Body { get; set; }

        public bool IsPublished { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
