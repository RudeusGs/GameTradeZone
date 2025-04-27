using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Service.Common.IServices;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.RechargeBank;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class RechargeBankService : IRechargeBankService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly DataContext _context;
    private readonly IUserService _userService;
    public RechargeBankService(IConfiguration configuration, HttpClient httpClient,DataContext context,IUserService userService)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _context = context;
        _userService = userService;

    }
    
    public async Task<ApiResult> GetAllRechargeBankTransactions()
    {
        try
        {
            var baseUrl = _configuration["SepayApi:BaseUrl"];
            var apiKey = _configuration["SepayApi:ApiKey"];
            var fullUrl = $"{baseUrl}/userapi/transactions/list";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            var response = await _httpClient.GetAsync(fullUrl);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult { Message = $"Request failed: {response.StatusCode}" };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var transactionsResponse = JsonSerializer.Deserialize<SepayTransactionsResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
  
            return new ApiResult
            {
                Data = transactionsResponse?.Transactions,
                Message = transactionsResponse?.Messages?.Success == true ? null : transactionsResponse?.Error?.ToString()
            };
        }
        catch (Exception ex)
        {
            return new ApiResult { Message = ex.Message };
        }
    }

    public async Task<ApiResult> GetAllRechargeBankTransactionsByUser()
    {
        try
        {
            
            var userId = _userService.UserId;

         
            var transactions = await _context.TransactionInfors
                .Where(t => t.UserId == userId)
                .Select(t => new
                {
                    t.Id,
                    t.UserId,
                    t.AccountNumber,
                    t.TransferAmount,
                    t.TransactionDate,
                    t.CreatedDate,
                    t.UpdatedDate,
                    t.DeleteDate
                })
                .ToListAsync();

       
            if (transactions == null || transactions.Count == 0)
            {
                return new ApiResult { Message = "No recharge transactions found for this user." };
            }

     
            return new ApiResult
            {
                Data = transactions,
                Message = "Recharge transactions retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new ApiResult { Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task<ApiResult> GetAllWithDrawTransactionsByUser()
    {
        try
        {
            
            var userId = _userService.UserId;
            var withdrawals = await _context.WithDrawnMoneys
                .Where(w => w.UserID == userId)
                .Join(
                    _context.Users,
                    withdrawal => withdrawal.UserID,
                    user => user.Id,
                    (withdrawal, user) => new
                    {
                        withdrawal.Id,
                        withdrawal.UserID,
                        withdrawal.Amount,
                        withdrawal.Status,
                        withdrawal.DrawnType,
                        withdrawal.CreatedDate,
                        withdrawal.UpdatedDate,
                        withdrawal.DeleteDate,
                        user.BankNumber,
                        user.BankName,
                    }
                )
                .ToListAsync();

          
            if (withdrawals == null || withdrawals.Count == 0)
            {
                return new ApiResult { Message = "No withdrawals found for this user." };
            }

            
            return new ApiResult
            {
                Data = withdrawals,
                Message = "Withdrawals retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new ApiResult { Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task<ApiResult> WithdrawMoneyConfirmRequest(int id)
    {
        try
        {
    
            var withdrawal = await _context.WithDrawnMoneys.FirstOrDefaultAsync(w => w.Id == id);
            if (withdrawal == null)
            {
                return new ApiResult { Message = "Withdrawal not found." };
            }

            if (withdrawal.Status != "Đang chờ xác thực")
            {
                return new ApiResult { Message = "Withdrawal is not in a pending state." };
            }
            withdrawal.Status = "Hoàn thành";
            withdrawal.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new ApiResult
            {
                Data = withdrawal,
                Message = "Withdrawal status updated to Completed."
            };
        }
        catch (Exception ex)
        {
            return new ApiResult { Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task<ApiResult> GetAllWithdrawMoneyRequest()
    {
        try
        {
            // Truy vấn tất cả các yêu cầu rút tiền và kết hợp với thông tin người dùng
            var withdrawals = await _context.WithDrawnMoneys
                .Join(
                    _context.Users,
                    withdrawal => withdrawal.UserID,
                    user => user.Id,
                    (withdrawal, user) => new
                    {
                        withdrawal.Id,
                        withdrawal.UserID,
                        withdrawal.Amount,
                        withdrawal.Status,
                        withdrawal.DrawnType,
                        withdrawal.CreatedDate,
                        withdrawal.UpdatedDate,
                        withdrawal.DeleteDate,
                        user.UserName,    // Lấy tên người dùng
                        user.BankNumber,  // Lấy số tài khoản
                        user.BankName     // Lấy tên ngân hàng
                    }
                )
                .ToListAsync();

            // Kiểm tra nếu không có yêu cầu rút tiền nào
            if (withdrawals == null || withdrawals.Count == 0)
            {
                return new ApiResult { Message = "No withdrawal requests found." };
            }

            // Trả về danh sách yêu cầu rút tiền
            return new ApiResult
            {
                Data = withdrawals,
                Message = "Withdrawal requests retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new ApiResult { Message = $"An error occurred: {ex.Message}" };
        }
    }
    public async Task<ApiResult> WithdrawMoneyRequest(WithdrawMoneyModel model)
    {
        try
        {
            var userId = _userService.UserId;


            if (model.Amount <= 0)
            {
                return new ApiResult { Message = "Invalid withdrawal amount." };
            }

        
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return new ApiResult { Message = "User not found." };
            }

          
            if (user.Balance == null || user.Balance < model.Amount)
            {
                return new ApiResult { Message = "Insufficient balance." };
            }
            user.Balance -= model.Amount;
            var withdrawal = new WithDrawnMoney
            {
                UserID = userId,
                Amount = model.Amount,
                Status = "Đang chờ xác thực", 
                DrawnType = model.DrawnType,
                CreatedDate = DateTime.UtcNow
            };

            _context.WithDrawnMoneys.Add(withdrawal);

        
            await _context.SaveChangesAsync();

            return new ApiResult
            {
                Data = withdrawal,
                Message = "Withdrawal request submitted successfully."
            };
        }
        catch (Exception ex)
        {
            return new ApiResult { Message = $"An error occurred: {ex.Message}" };
        }
    }
    public async Task<ApiResult> GetStaticForAllUser()
    {
        try
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalWithdrawals = await _context.WithDrawnMoneys.CountAsync();
            var totalRechargeTransactions = await _context.TransactionInfors.CountAsync();

            // Calculate income, outcome, and include current balance for each user
            var userIncomeOutcome = await _context.Users
                .Select(user => new
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    CurrentBalance = user.Balance ?? 0, // Include current balance
                    TotalIncome = _context.WithDrawnMoneys
                        .Where(w => w.UserID == user.Id)
                        .Sum(w => (decimal?)w.Amount) ?? 0,
                    TotalOutcome = _context.TransactionInfors
                        .Where(t => t.UserId == user.Id)
                        .Sum(t => (decimal?)t.TransferAmount) ?? 0
                })
                .ToListAsync();

            var staticData = new
            {
                TotalUsers = totalUsers,
                TotalWithdrawals = totalWithdrawals,
                TotalRechargeTransactions = totalRechargeTransactions,
                UserIncomeOutcome = userIncomeOutcome
            };

            return new ApiResult
            {
                Data = staticData,
                Message = "Static data retrieved successfully, including income, outcome, and current balance for each user."
            };
        }
        catch (Exception ex)
        {
            return new ApiResult { Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task<ApiResult> GetIncomeAndOutcome()
    {
        try
        {
            var totalIncome = await _context.WithDrawnMoneys.SumAsync(w => w.Amount);
            var totalOutcome = await _context.TransactionInfors.SumAsync(t => t.TransferAmount);
            var incomeAndOutcome = new
            {
                TotalIncome = totalIncome,
                TotalOutcome = totalOutcome
            };
            return new ApiResult
            {
                Data = incomeAndOutcome,
                Message = "Income and outcome data retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new ApiResult { Message = $"An error occurred: {ex.Message}" };
        }
    }


}
