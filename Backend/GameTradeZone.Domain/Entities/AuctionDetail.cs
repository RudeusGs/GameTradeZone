using GameTradeZone.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Domain.Entities
{
    public class AuctionDetail : EntityBase
    {
        public int? AuctionId { get; set; }
        public int? UserId { get; set; }
        public string? RaisePrice { get; set; }
        public DateTime? RaiseDateTime { get; set; }
    }
}
