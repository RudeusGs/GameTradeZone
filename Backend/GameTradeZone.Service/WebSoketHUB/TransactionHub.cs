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
        public async Task SendTransactionStatus(int userId, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveTransactionStatus", message);
        }
    }
}
