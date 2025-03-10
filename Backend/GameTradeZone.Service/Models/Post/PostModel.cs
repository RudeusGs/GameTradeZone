using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Models.Post
{
    class PostModel
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
    }
}
