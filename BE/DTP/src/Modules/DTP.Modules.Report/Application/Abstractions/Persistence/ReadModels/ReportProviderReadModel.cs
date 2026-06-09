using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels
{
    public class ReportProviderReadModel
    {
        public Guid Id { get; set; }

        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
