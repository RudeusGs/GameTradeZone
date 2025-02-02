using GameTradeZone.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Domain.Entities
{
    public class RechargeBank : EntityBase
    {
        public int? UserID { get; set; }
        public int Amount { get; set; }
        public string? BankName { get; set; }
        public int? BankNumber { get; set; }
        public string? Content { get; set; }
    }
}
