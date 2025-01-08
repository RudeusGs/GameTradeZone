using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class GameInfor : EntityBase
    {
        public int UserID { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string PasswordChange { get; set; }
        public string Price { get; set; }
        public string Status { get; set; } 
        public string Image {  get; set; }
        public int CustomerFeedback { get; set; } // nếu Status = "Đã bán" thì hiển thị cái này
    }
}
