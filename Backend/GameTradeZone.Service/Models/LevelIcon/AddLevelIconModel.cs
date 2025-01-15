using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.LevelIcon
{
    public class AddLevelIconModel
    {
        public string IconName { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
