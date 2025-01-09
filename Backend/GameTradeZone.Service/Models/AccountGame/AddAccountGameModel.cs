using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.Models.AccountGame
{
    public class AddAccountGameModel
    {
        public int GameInforID { get; set; }
        public int UserID { get; set; }
        public string GameName { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }
        public List<IFormFile>? Files { get; set; }
        public int CustomerFeedback { get; set; } // nếu Status = "Đã bán" thì hiển thị cái này
    }
}
