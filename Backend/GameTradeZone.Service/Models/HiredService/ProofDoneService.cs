using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.HiredService
{
    public class ProofDoneService
    {
        public int Id { get; set; }
        public string Decription { get; set; }
        public List<IFormFile> Files {  get; set; }
    }
}
