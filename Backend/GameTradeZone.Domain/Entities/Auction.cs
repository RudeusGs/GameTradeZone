using GameTradeZone.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Domain.Entities
{
    public class Auction : EntityBase
    {
        public int? UserId { get; set; } // nguoi tao phien dau gia 
        public int? AuctionPrizeId { get; set; }
        public int? GameInforsId { get; set; }

        public string? AuctionName { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string? StartPrice { get; set; }
        public string? CurrentPrice { get; set; }

        public string? TimeToEnd { get; set; } = "5"; // time between each raise by user(usually 5min)
        public bool? EndStatus { get; set; } = false; // auction end or not(end if(WinnerId = true))
        public DateTime? EndDateTime { get; set; } // Endtime only has value when EndAuction function is activated
        public bool? IsApproved { get; set; } = false; // admin approve or not
        public int? WinnerId { get; set; } = 0;
    }
}
