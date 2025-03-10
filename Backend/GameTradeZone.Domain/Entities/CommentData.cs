using GameTradeZone.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Domain.Entities
{
    public class CommentData : EntityBase
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string? Content { get; set; }  
        public User? User { get; set; }
    }
}
