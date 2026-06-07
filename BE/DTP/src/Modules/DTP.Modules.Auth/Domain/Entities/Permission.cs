using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Domain.Entities
{
    public class Permission : EntityBase
    {

        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Module { get; set; } = default!;
        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
