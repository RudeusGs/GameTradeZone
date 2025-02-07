using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GameTradeZone.Domain.Entities;
using GameTradeZone.Infrastructure.Persistence;
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
    public RechargeBankService(IConfiguration configuration, HttpClient httpClient,DataContext context)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _context = context;
    }
    // xem giao dịch nạp tiền
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

}
