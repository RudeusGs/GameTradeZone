using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Models.Post
{
     public class CommentRequest
    {
        public int PostId { get; set; }
        public string? Content { get; set; }
    }
}
