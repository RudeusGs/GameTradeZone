using GameTradeZone.Service.Hubs;
using GameTradeZone.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace GameTradeZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<AuctionHub> _hubContext;

        public ChatController(IChatService chatService, IHubContext<AuctionHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        [Authorize]
        [HttpPost("SendChatMessage")]
        public async Task<IActionResult> SendChatMessage([FromBody] SendChatMessageModel model)
        {
            try
            {
                // Lưu tin nhắn qua ChatService
                await _chatService.SaveChatMessageAsync(model.AuctionId, model.UserId, model.Message);

                // Gửi tin nhắn đến nhóm qua SignalR
                string groupName = $"Auction_{model.AuctionId}";
                await _hubContext.Clients.Group(groupName).SendAsync(
                    "ReceiveMessage",
                    model.UserId,
                    model.Message,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                );

                return Response(new { IsSuccess = true, Message = "Message sent successfully" });
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }

        [Authorize]
        [HttpGet("GetChatMessages/{auctionId}")]
        public async Task<IActionResult> GetChatMessages(int auctionId)
        {
            try
            {
                var messages = await _chatService.GetChatMessagesAsync(auctionId);
                var response = messages.Select(m => new
                {
                    m.UserId,
                    m.MessageText,
                    SentAt = m.SentAt.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();

                return Response(new { IsSuccess = true, Data = response });
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }

    public class SendChatMessageModel
    {
        public int AuctionId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
    }
}