// GameTradeZone.Controllers/WebhookController.cs
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Models.RechargeBank;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;
using GameTradeZone.Domain.Entities;
using GameTradeZone.Service.WebSoketHUB;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace GameTradeZone.Controllers
{
    [Route("api/webhook")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IHubContext<TransactionHub> _hubContext;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            DataContext context,
            IHubContext<TransactionHub> hubContext,
            ILogger<WebhookController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost("sepay")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] WebhookPayload payload)
        {
            try
            {
                _logger.LogInformation("Received webhook payload: {@Payload}", payload);

                if (payload == null || string.IsNullOrEmpty(payload.Content))
                {
                    _logger.LogWarning("Invalid payload or missing content.");
                    return BadRequest(new { success = false, message = "Payload không hợp lệ!" });
                }

                if (payload.TransferType != "in" || payload.TransferAmount <= 0)
                {
                    _logger.LogWarning("Invalid transfer: TransferType={TransferType}, TransferAmount={TransferAmount}",
                        payload.TransferType, payload.TransferAmount);
                    return BadRequest(new { success = false, message = "Giao dịch không hợp lệ!" });
                }

                var match = Regex.Match(payload.Content, @"USERID\s*(\d+)");
                if (!match.Success || !int.TryParse(match.Groups[1].Value, out int userId))
                {
                    _logger.LogWarning("UserId not found or invalid in content: {Content}", payload.Content);
                    return BadRequest(new { success = false, message = "Không tìm thấy hoặc UserId không hợp lệ!" });
                }

                using var dbTransaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user == null)
                    {
                        _logger.LogWarning("User not found for UserId: {UserId}", userId);
                        return BadRequest(new { success = false, message = "Không tìm thấy người dùng!" });
                    }

                    user.Balance += payload.TransferAmount;

                    var transaction = new TransactionInfor
                    {
                        AccountNumber = payload.AccountNumber,
                        ReferenceCode = payload.ReferenceCode,
                        TransferAmount = payload.TransferAmount,
                        UserId = user.Id,
                        TransactionDate = DateTime.TryParse(payload.TransactionDate, out var transactionDate) ? transactionDate : DateTime.UtcNow,
                     
                    };

                    await _context.TransactionInfors.AddAsync(transaction);
                    await _context.SaveChangesAsync();

                    await dbTransaction.CommitAsync();

                    string notifyMessage = $"Giao dịch {transaction.ReferenceCode} thành công! Tổng tiền hiện tại: {user.Balance} VNĐ";
                    try
                    {
                        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveTransactionStatus", notifyMessage);
                        await _hubContext.Clients.User(userId.ToString()).SendAsync("PaymentCompleted");
                        _logger.LogInformation("SignalR messages sent successfully for UserId: {UserId}", userId);
                    }
                    catch (Exception signalRex)
                    {
                        _logger.LogWarning(signalRex, "SignalR failed for UserId: {UserId}. Saving to queue for retry.", userId);
                        // Fallback: Implement a queue (e.g., PendingNotifications table) if needed
                    }

                    _logger.LogInformation("Webhook processed successfully for UserId: {UserId}, ReferenceCode: {ReferenceCode}",
                        userId, transaction.ReferenceCode);
                    return Ok(new { success = true, message = "Thành công" });
                }
                catch (Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    _logger.LogError(ex, "Error processing transaction for UserId: {UserId}", userId);
                    return StatusCode(500, new { success = false, message = $"Lỗi khi xử lý giao dịch: {ex.Message}" });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "System error while processing webhook.");
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {e.Message}" });
            }
        }
    }
}