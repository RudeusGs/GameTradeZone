using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTradeZone.Service.Models.User
{
    public class UpdateAvatarModel
    {
        public int UserId { get; set; }
        public string ImageIrl { get; set; }
    }
}
