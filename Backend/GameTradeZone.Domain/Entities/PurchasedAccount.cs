using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class PurchasedAccount : EntityBase
    {
        public int? UserID { get; set; } // ID của người mua
        public int? SellerID { get; set; } // ID của người bán
        public string? GameName { get; set; }
        public string? AccountName { get; set; }
        public string? Password { get; set; }
        public string? StatusBuyer { get; set; } // Đồng ý, Không đồng ý
        public string? StatusSeller { get; set; } // Đang chờ, hoàn thành, từ chối
        public string? Reason { get; set; } // Lý do từ chối
        public int? MyFeedback { get; set; } // Đánh giá người bán chỉ xảy ra khi đã Đồng ý dành cho buyer
        public bool? IsDelete { get; set; }
    }
}
