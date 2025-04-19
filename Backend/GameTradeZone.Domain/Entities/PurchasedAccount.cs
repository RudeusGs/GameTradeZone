using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class PurchasedAccount : EntityBase
    {
        public int? UserID { get; set; } // ID của người mua
        public int? SellerID { get; set; } // ID của người bán
        public string? Email { get; set; } // Email của tài khoản đó
        public string? OTPEmail { get; set; } // Một số game cần phải xác nhận qua email mới đổi được mật khẩu
        public int? AccountGameId { get; set; }
        public string? GameName { get; set; }
        public string? AccountName { get; set; }
        public string? Password { get; set; }
        public decimal Price { get; set; }
        public string? StatusBuyer { get; set; } // Đồng ý, Không đồng ý
        public string? StatusSeller { get; set; } // Đang chờ, hoàn thành, từ chối
        public string? Reason { get; set; } // Lý do từ chối
        public DateTime? OTPSentTime{ get; set;}
        public bool? IsDelete { get; set; }
    }
}
