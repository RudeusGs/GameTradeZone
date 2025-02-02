using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using GameTradeZone.Service.Models.RechargeBank.Converter;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Models.RechargeBank
{
    public class SepayTransactionDetailResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("bank_brand_name")]
        public string? BankBrandName { get; set; }

        [JsonPropertyName("account_number")]
        public string? AccountNumber { get; set; }

        [JsonPropertyName("transaction_date")]
        [JsonConverter(typeof(RechargeBank.Converter.DateTimeConverter))]
        public DateTime? TransactionDate { get; set; }

        [JsonPropertyName("amount_out")]
        public string? AmountOut { get; set; } // Để kiểu string nếu API trả về dạng "0.00"

        [JsonPropertyName("amount_in")]
        public string? AmountIn { get; set; }

        [JsonPropertyName("accumulated")]
        public string? Accumulated { get; set; }

        [JsonPropertyName("transaction_content")]
        public string? TransactionContent { get; set; }

        [JsonPropertyName("reference_number")]
        public string? ReferenceNumber { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("sub_account")]
        public string? SubAccount { get; set; }

        [JsonPropertyName("bank_account_id")]
        public string? BankAccountId { get; set; }
    }

    public class SepayTransactionsResponse
    {
        public int Status { get; set; }
        public object Error { get; set; }
        public Messages Messages { get; set; }
        public List<SepayTransactionDetailResponse> Transactions { get; set; }
    }

    public class Messages
    {
        public bool Success { get; set; }
    }
}
