using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels
{
    public class ReportRoleReadModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
