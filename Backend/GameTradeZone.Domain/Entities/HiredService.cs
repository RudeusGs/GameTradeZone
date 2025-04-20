using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class HiredService : EntityBase
    {
        // Dành cho người tạo dịch vụ
        public int? ServiceID { get; set; }
        public int? UserID { get; set; } // ID người thuê dịch vụ
        public string? Status { get; set; } // Đã xong, chưa xong, đang chờ nhận, từ chối
        public string? Reason { get; set; } // Lý do từ chối
        public string? Decriptions { get; set; }
        public string? Image { get; set; }
        public bool? IsDelete { get; set; }
    }
}
