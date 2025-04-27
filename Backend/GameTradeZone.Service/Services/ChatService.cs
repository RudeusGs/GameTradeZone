using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Services
{
    public class ChatService : IChatService
    {
        private readonly DataContext _dataContext;

        public ChatService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task SaveChatMessageAsync(int auctionId, int userId, string messageText)
        {
            var chatMessage = new ChatMessage
            {
                AuctionId = auctionId,
                UserId = userId,
                MessageText = messageText,
                SentAt = DateTime.Now
            };

            _dataContext.ChatMessages.Add(chatMessage);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<ChatMessage>> GetChatMessagesAsync(int auctionId)
        {
            return await _dataContext.ChatMessages
                .Where(m => m.AuctionId == auctionId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
