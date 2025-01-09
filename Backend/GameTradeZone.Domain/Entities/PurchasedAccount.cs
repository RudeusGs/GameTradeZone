using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class PurchasedAccount : EntityBase
    {
        public int UserID { get; set; } // ID của người mua
        public int SellerID { get; set; } // ID của người bán
        public string GameName { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string Status { get; set; } // Đồng ý, Không đồng ý
        public int MyFeedback { get; set; } // Đánh giá tài khoản chỉ xảy ra khi đã Đồng ý
    }
}
