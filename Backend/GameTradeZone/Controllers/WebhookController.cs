using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Models.RechargeBank;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;
using GameTradeZone.Domain.Entities;

namespace GameTradeZone.Controllers
{
    [Route("api/webhook")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly DataContext _context;

        public WebhookController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("sepay")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] WebhookPayload payload)
        {
            try
            {
                Console.WriteLine($"📩 Received Webhook: RefCode {payload.ReferenceCode} - Amount {payload.TransferAmount} - Content {payload.Content}");

                var match = Regex.Match(payload.Content, @"USERID\s*(\d+)");
                if (!match.Success)
                {
                    return BadRequest(new { Message = "❌ Nội dung giao dịch không hợp lệ, không tìm thấy UserId!" });
                }

                int userId = int.Parse(match.Groups[1].Value);

         
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return BadRequest(new { Message = "❌ Không tìm thấy User!" });
                }

 
                user.Balance += payload.TransferAmount;

                var transaction = new TransactionInfor
                {
                    AccountNumber = payload.AccountNumber,
                    ReferenceCode = payload.ReferenceCode,
                    TransferAmount = payload.TransferAmount,
      
                    UserId = user.Id
                };

                await _context.TransactionInfors.AddAsync(transaction);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "✅ Nạp tiền thành công!", UserId = user.Id, NewBalance = user.Balance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "🚨 Lỗi Server!", Error = ex.Message });
            }
        }
    }
}
