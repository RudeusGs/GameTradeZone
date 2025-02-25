using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.Dispute
{
    public class AddDisputeModel
    {
        public int Id { get; set; }
        public int? PurchasedAccountID { get; set; }
        public int? OnGoingServiceID { get; set; }
        public int? HiredServiceID { get; set; }
        public string? Reason { get; set; }
        public List<IFormFile>? Files { get; set; }
        public string? Reply { get; set; }
    }
}
