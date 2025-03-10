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
        public override Task OnConnectedAsync()
        {
            string userId = Context.UserIdentifier;
            Console.WriteLine($"User connected with ID: {userId}");
            return base.OnConnectedAsync();
        }
        public async Task SendTransactionStatus(int userId, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveTransactionStatus", message);
        }
    }
}
