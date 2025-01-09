using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.GameInfor
{
    public class AddGameInforModel
    {
        public string GameName { get; set; }
        public string Genre { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
