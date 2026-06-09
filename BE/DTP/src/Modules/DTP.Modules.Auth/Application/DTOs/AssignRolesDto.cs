using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Application.DTOs
{
    public class AssignRolesDto
    {
        public List<Guid> RoleIds { get; set; } = new();
    }
}
