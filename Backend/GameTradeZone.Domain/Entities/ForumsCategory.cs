using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Domain.Entities
{
    public class ForumsCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int PostCount { get; set; }
        public string? IconClass { get; set; }
    }
}
