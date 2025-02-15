namespace GameTradeZone.Service.Models.PurchasedAccount
{
    public class ComfirmModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string? Reason { get; set; }
        public string? Feedback { get; set; }
    }
}
