using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.AccountGame
{
    public class UpdateAccountGameModel
    {
        public int Id { get; set; }
        public int? GameInforID { get; set; }
        public string GameName { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public decimal Price { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
