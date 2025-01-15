using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.LevelIcon
{
    public class UpdateLevelIconModel
    {
        public int Id { get; set; }
        public string? IconName { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
