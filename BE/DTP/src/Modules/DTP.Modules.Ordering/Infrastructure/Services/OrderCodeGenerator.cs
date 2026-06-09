using DTP.Modules.Ordering.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Infrastructure.Services
{
    public class OrderCodeGenerator : IOrderCodeGenerator
    {
        public string Generate()
        {
            return $"DTP{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }
    }
}
