using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Report.Application.Abstractions.Persistence.ReadModels
{
    public class ReportUserRoleReadModel
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
