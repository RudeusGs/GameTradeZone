using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class Service : EntityBase
    {
        public int? GameInforID { get; set; }
        public string? ServiceName { get; set; }
        public int? CreaterID { get; set; } // Người tạo dịch vụ
        public string? Decription { get; set; }
        public int? ServiceLevel { get; set; } // Số lượng thuê càng nhiều level càng cao
        public decimal ServicePrice { get; set; }
        public TimeSpan? ServiceTime { get; set; } // Thời gian ước tính khi làm dịch vụ đó
        public int? RentedC {  get; set; } // Số lượng dịch vụ đã được thuê
        public string? Feedback { get; set; } // Chỉ những người được thuê   
        public string? Image { get; set; }
        public bool? IsDelete { get; set; }
    }
}
