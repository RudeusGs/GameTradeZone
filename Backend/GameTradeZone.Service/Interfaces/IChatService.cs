using GameTradeZone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Interfaces
{
    public interface IChatService
    {
        Task SaveChatMessageAsync(int auctionId, int userId, string messageText);
        Task<List<ChatMessage>> GetChatMessagesAsync(int auctionId);
    }
}
