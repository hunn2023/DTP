using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Hubs
{
    public sealed class PaymentHub : Hub
    {
        public async Task JoinOrderPaymentGroup(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return;

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                GetOrderPaymentGroup(orderId));
        }

        public async Task LeaveOrderPaymentGroup(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return;

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                GetOrderPaymentGroup(orderId));
        }

        public static string GetOrderPaymentGroup(string orderId)
        {
            return $"payment:order:{orderId}";
        }
    }
}
