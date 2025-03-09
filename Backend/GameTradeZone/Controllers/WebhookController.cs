using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Models.RechargeBank;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;
using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.WebSoketHUB; // Giả sử đây là namespace chứa TransactionHub
using Microsoft.AspNetCore.SignalR;

namespace GameTradeZone.Controllers
{
    [Route("api/webhook")]
    [ApiController]
    public class WebhookController : BaseController
    {
        private readonly DataContext _context;
        private readonly IHubContext<TransactionHub> _hubContext;

        public WebhookController(DataContext context, IHubContext<TransactionHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("sepay")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] WebhookPayload payload)
        {
            try
            {
                // Kiểm tra nội dung tin nhắn để lấy UserId
                var match = Regex.Match(payload.Content, @"USERID\s*(\d+)");
                if (!match.Success)
                {
                    return BadRequest(new { Message = "Không tìm thấy UserId trong nội dung!" });
                }

                int userId = int.Parse(match.Groups[1].Value);

                // Tìm người dùng theo UserId trong database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return BadRequest(new { Message = "Không tìm thấy User" });
                }

                // Cập nhật số dư của người dùng
                user.Balance += payload.TransferAmount;

                // Tạo bản ghi giao dịch
                var transaction = new TransactionInfor
                {
                    AccountNumber = payload.AccountNumber,
                    ReferenceCode = payload.ReferenceCode,
                    TransferAmount = payload.TransferAmount,
                    UserId = user.Id,
                };

                await _context.TransactionInfors.AddAsync(transaction);
                await _context.SaveChangesAsync();
                string notifyMessage = $"Giao dịch {transaction.ReferenceCode} thành công! Tổng tiền hiện tại: {user.Balance} VNĐ";
                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveTransactionStatus", notifyMessage);
                return Response(new { Message = "Thành công" });
            }
            catch (Exception e)
            {
                return Response(e.Message, 500);
            }
        }
    }
}
