using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.WebSoketHUB
{
    public class TransactionHub : Hub
    {
        public async Task SendPaymentConfirmation(string userId)
        {
            await Clients.User(userId).SendAsync("PaymentCompleted");
        }
    }
}
