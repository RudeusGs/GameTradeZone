using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class OnGoingService : EntityBase
    {
        // Dành cho người thuê dịch vụ
            public int? ServiceID { get; set; }
            public int? UserID { get; set; }
            public string? Status { get; set; } // Đồng ý, không đồng ý, đang chờ xác nhận
            public string? Reason { get; set; } // Lý do không đồng ý
            public string? Decriptions { get; set; }
            public DateTime? EndTime { get; set; }
            public bool? IsDelete { get; set; }
    }
}
