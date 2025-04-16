using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Hubs
{
    public class AuctionHub : Hub
    {
        public async Task JoinAuctionDetailGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AuctionDetailGroup");
            await Clients.Group("AuctionDetailGroup").SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task LeaveAuctionDetailGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AuctionDetailGroup");
            await Clients.Group("AuctionDetailGroup").SendAsync("UserLeft", Context.ConnectionId);
        }
    }
}