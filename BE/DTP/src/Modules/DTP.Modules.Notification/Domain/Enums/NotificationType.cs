using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Domain.Enums
{
    public enum NotificationType
    {
        General = 1,

        OrderCreated = 100,
        OrderPaid = 101,
        OrderCancelled = 102,
        OrderCompleted = 103,

        PaymentSuccess = 200,
        PaymentFailed = 201,

        DeliverySuccess = 300,
        DeliveryFailed = 301,

        AdminAlert = 900
    }
}
