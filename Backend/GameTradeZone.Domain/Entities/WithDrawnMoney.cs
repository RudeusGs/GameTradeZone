using GameTradeZone.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Domain.Entities
{
    public class WithDrawnMoney : EntityBase
    {
        public int UserID { get; set; }
        public int Amount { get; set; }
        public string? Status { get; set; }
        public string? DrawnType { get; set; }
    }
}
