using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GameTradeZone.Service.Interfaces;
using GameTradeZone.Service.Models;
using GameTradeZone.Service.Models.RechargeBank;
using Microsoft.Extensions.Configuration;

public class RechargeBankService : IRechargeBankService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public RechargeBankService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<ApiResult> GetAllRechargeBankTransactions()
    {

        try
        {
            var baseUrl = _configuration["SepayApi:BaseUrl"];
            var apiKey = _configuration["SepayApi:ApiKey"];
            var fullUrl = $"{baseUrl}/userapi/transactions/list";

            Console.WriteLine($"Requesting: {fullUrl}");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            var response = await _httpClient.GetAsync(fullUrl);

            Console.WriteLine($"Response Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult { Message = $"Request failed: {response.StatusCode}" };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var transactionsResponse = JsonSerializer.Deserialize<SepayTransactionsResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var responseContentcheck = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Response: " + responseContentcheck);

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
