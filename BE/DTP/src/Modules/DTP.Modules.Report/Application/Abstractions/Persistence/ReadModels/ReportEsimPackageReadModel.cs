using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels
{
    public class ReportEsimPackageReadModel
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProviderId { get; set; }

        public bool IsActive { get; set; }
    }
}
