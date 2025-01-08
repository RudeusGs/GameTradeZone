using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class PurchasedAccount : EntityBase
    {
        public int UserID { get; set; } // ID của người mua
        public int SellerID { get; set; } // Ẩn danh, ID của người bán
        public int DisputeID { get; set; }
        public string GameName { get; set; }
        public string AccountName { get; set; }
        public string PasswordChange { get; set; }
        public int MyFeedback { get; set; } // Đánh giá tài khoản
    }
}
