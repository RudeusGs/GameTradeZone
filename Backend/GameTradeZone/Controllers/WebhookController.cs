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
             // cái này check nội dung tin nhắn xem user id bao nhiêu để gửi  
                var match = Regex.Match(payload.Content, @"USERID\s*(\d+)");
                if (!match.Success)
                {
                    return BadRequest(new { Message = " không tìm thấy UserId!" });
                }

                int userId = int.Parse(match.Groups[1].Value);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return BadRequest(new { Message = "Không tìm thấy User" });
                }
                user.Balance += payload.TransferAmount;
                var transaction = new TransactionInfor
                {
                    AccountNumber = payload.AccountNumber,
                    ReferenceCode = payload.ReferenceCode,
                    TransferAmount = payload.TransferAmount,
                    /*chưa cập nhật ngày tháng :>> bị lỗi 400 datetime ko hiểu kiểu gì để string thì nhận chắc
                    do nó ko trả về date time */
                UserId = user.Id
                };
                await _context.TransactionInfors.AddAsync(transaction);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Nạp tiền thành công!", UserId = user.Id, NewBalance = user.Balance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error!", Error = ex.Message });
            }
        }
    }
}
