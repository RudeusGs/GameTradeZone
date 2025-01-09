using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class Notification : EntityBase
    {
        public string TypeNoti { get; set; }
        public string Content { get; set; }
        public int SenderID { get; set; }
        public int UserID { get; set; }
        public bool IsRead { get; set; }
    }
}
