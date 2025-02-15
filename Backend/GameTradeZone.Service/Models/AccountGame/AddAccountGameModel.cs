using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.AccountGame
{
    public class AddAccountGameModel
    {
        public int GameInforID { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public decimal Price { get; set; }
        public decimal PriceMin { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
