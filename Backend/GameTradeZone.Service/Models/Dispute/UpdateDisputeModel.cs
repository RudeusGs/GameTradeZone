using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.Dispute
{
    public class UpdateDisputeModel
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public List<IFormFile>? Files { get; set; }
        public string Reply { get; set; }
    }
}
