using System;
using System.Text.Json.Serialization;

public class WebhookPayload
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("gateway")]
    public string? Gateway { get; set; }

    [JsonPropertyName("transactionDate")]
    public string? TransactionDate { get; set; }

    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("transferType")]
    public string? TransferType { get; set; }

    [JsonPropertyName("transferAmount")]
    public decimal TransferAmount { get; set; }

    [JsonPropertyName("accumulated")]
    public decimal Accumulated { get; set; }

    [JsonPropertyName("subAccount")]
    public string? SubAccount { get; set; }

    [JsonPropertyName("referenceCode")]
    public string? ReferenceCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
