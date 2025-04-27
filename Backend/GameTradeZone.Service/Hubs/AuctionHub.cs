using Microsoft.AspNetCore.SignalR;
using GameTradeZone.Service.Interfaces;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Hubs
{
    public class AuctionHub : Hub
    {
        private readonly IChatService _chatService;

        public AuctionHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // Log khi client kết nối
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        // Log khi client ngắt kết nối
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}, Exception: {exception?.Message}");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinAuctionDetailGroup(int auctionId)
        {
            string groupName = $"Auction_{auctionId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Client {Context.ConnectionId} joined group {groupName}");
            await Clients.Group(groupName).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task LeaveAuctionDetailGroup(int auctionId)
        {
            string groupName = $"Auction_{auctionId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Client {Context.ConnectionId} left group {groupName}");
            await Clients.Group(groupName).SendAsync("UserLeft", Context.ConnectionId);
        }

        public async Task SendMessage(int auctionId, int userId, string message)
        {
            string groupName = $"Auction_{auctionId}";
            Console.WriteLine($"Sending message to group {groupName}: {message}");

            // Lưu tin nhắn qua ChatService
            await _chatService.SaveChatMessageAsync(auctionId, userId, message);

            // Gửi tin nhắn đến nhóm
            await Clients.Group(groupName).SendAsync(
                "ReceiveMessage",
                userId,
                message,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            );
        }
    }
}