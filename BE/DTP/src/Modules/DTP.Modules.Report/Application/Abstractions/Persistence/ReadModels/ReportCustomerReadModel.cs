using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels
{
    public class ReportCustomerReadModel
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
