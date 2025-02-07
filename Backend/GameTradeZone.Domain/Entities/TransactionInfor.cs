using GameTradeZone.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Domain.Entities
{
    public class TransactionInfor : EntityBase
    {
       
        public int? UserId { get; set; }
        public string? AccountNumber { get; set; }
        [Key]
        public string? ReferenceCode { get; set; }
        public decimal TransferAmount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
