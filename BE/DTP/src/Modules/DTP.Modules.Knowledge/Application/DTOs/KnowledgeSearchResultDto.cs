using DTP.Modules.Knowledge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.DTOs
{
    public class KnowledgeSearchResultDto
    {
        public Guid Id { get; set; }

        public KnowledgeSourceType SourceType { get; set; }

        public Guid SourceId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Slug { get; set; }

        public string? SourceUrl { get; set; }

        public string Content { get; set; } = string.Empty;

        public double Score { get; set; }
    }
}
