using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Models.Auction
{
    public class AddAuctionDetailModel
    {
        public int AuctionId { get; set; }
        public string RaisePrice { get; set; }
        public DateTime RaiseDateTime { get; set; }
    }
}
