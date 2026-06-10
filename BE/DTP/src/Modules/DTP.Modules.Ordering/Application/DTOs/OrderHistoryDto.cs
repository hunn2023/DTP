using DTP.Modules.Ordering.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class OrderHistoryDto
    {
        public Guid Id { get; set; }

        public OrderStatus FromStatus { get; set; }

        public OrderStatus ToStatus { get; set; }

        public string? Note { get; set; }

        public Guid? ChangedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
