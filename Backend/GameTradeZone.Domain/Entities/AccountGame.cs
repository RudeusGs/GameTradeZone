using GameTradeZone.Domain.Base;
using Microsoft.Identity.Client;

namespace GameTradeZone.Domain.Entities
{
    public class AccountGame : EntityBase
    {
        public int? GameInforID { get; set; }
        public int? UserID { get; set; }
        public string? AccountName { get; set; }
        public string? Password { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal Price { get; set; }
        public string? Status { get; set; }
        public string? Image { get; set; }
        public string? CustomerFeedback { get; set; } // nếu Status = "Đã bán" thì hiển thị cái này
        public bool? IsDelete { get; set; }
    }
}
