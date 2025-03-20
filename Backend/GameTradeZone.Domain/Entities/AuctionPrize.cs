using GameTradeZone.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Domain.Entities
{
    public class AuctionPrize : EntityBase
    {
        public int? AuctionId { get; set; }
        public string? PrizeName { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? PrizeInfo { get; set; }
        public bool? Status { get; set; } = false; // da trao thuong hay chua
    }
}
