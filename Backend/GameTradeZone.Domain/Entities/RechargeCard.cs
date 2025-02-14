using GameTradeZone.Domain.Base;
using System;

namespace GameTradeZone.Domain.Entities
{
    public class RechargeCard : EntityBase
    {
        public int UserId { get; set; }
        public string? CardNumber { get; set; }
        public string? CardSerial { get; set; }
        public int? Amount { get; set; }
        public string? Status { get; set; }
        public string? Provider { get; set; }
    }
}