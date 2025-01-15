using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class Dispute : EntityBase
    {
        public int? PurchasedAccountID { get; set; }
        public int? OnGoingServiceID { get; set; }
        public int? HiredServiceID { get; set; }
        public int? UserID { get; set; } //ID người tố cáo
        public int? SellertID { get; set; } // ID người bị tố cáo
        public string? Reason { get; set; }
        public string? Proof {  get; set; }
        public string? Status { get; set; }
        public string? Reply {  get; set; }
        public bool? IsDelete { get; set; }
    }
}
