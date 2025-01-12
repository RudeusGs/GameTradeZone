using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.Service
{
    public class UpdateServiceModel
    {
        public int Id { get; set; }
        public string GameInforID { get; set; }
        public string ServiceName { get; set; }
        public string Decription { get; set; }
        public decimal? ServicePrice { get; set; }
        public TimeSpan? ServiceTime { get; set; } // Thời gian ước tính khi làm dịch vụ đó 
        public List<IFormFile>? Files { get; set; }
    }
}
