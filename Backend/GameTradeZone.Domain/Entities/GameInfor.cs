using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class GameInfor : EntityBase
    {
        public string? GameName { get; set; }
        public string? Image { get; set; }
        public string? Genre { get; set; }
        public bool? IsDelete { get; set; }
    }
}
