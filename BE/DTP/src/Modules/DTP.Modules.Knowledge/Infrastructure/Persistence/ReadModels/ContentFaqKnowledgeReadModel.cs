using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Persistence.ReadModels
{
    public class ContentFaqKnowledgeReadModel
    {
        public Guid Id { get; set; }

        public Guid ContentId { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
