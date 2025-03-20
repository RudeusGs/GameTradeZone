using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Models.Auction
{
    public class AddAuctionModel
    {
        public string AuctionName { get; set; }
        public DateTime StartDateTime { get; set; }
        public string StartPrice { get; set; }

        // Thông tin phần thưởng (phần thưởng sẽ được liên kết với đấu giá này)
        public string PrizeName { get; set; }
        public string PrizeDescription { get; set; }
        public List<IFormFile>? PrizeImage { get; set; }
        public string PrizeInfo { get; set; }

    }
}
