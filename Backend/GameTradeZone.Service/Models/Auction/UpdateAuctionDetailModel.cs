using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Models.Auction
{
    public class UpdateAuctionDetailModel
    {
        public int Id { get; set; }
        public int? AuctionId { get; set; }
        public int? UserId { get; set; }
        public string? RaisePrice { get; set; }
        public DateTime? RaiseDateTime { get; set; }
    }
}
