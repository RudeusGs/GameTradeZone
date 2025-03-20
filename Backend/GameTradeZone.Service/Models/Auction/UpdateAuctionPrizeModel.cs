using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Models.Auction
{
    public class UpdateAuctionPrizeModel
    {
        public int Id { get; set; }
        public int? AuctionId { get; set; }
        public string? PrizeName { get; set; }
        public string? Description { get; set; }
        public List<IFormFile>? Files { get; set; }
        public string? PrizeInfo { get; set; }
        public bool? Status { get; set; } = false; // da trao thuong hay chua
    }
}
