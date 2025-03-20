using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Models.Auction
{
    public class UpdateAuctionModel
    {
        public int? Id { get; set; }
        public int? UserId { get; set; } // nguoi tao phien dau gia 
        //update phần thưởng sẽ không cần mà thay vào đó update phần thưởng trong phần updateAuctionPrize
        public string? AuctionName { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string? StartPrice { get; set; }
        public string? CurrentPrice { get; set; }

        public string? TimeToEnd { get; set; } 
        public bool? EndStatus { get; set; }
        public bool? IsApproved { get; set; } 
        public int? WinnerId { get; set; } 
    }
}
