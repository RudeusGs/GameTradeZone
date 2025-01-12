using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class LevelIcon : EntityBase
    {
        public string? IconName { get; set; }
        public string? IconImage { get; set; }
        public int? UserID { get; set; }
        public bool? IsDelete { get; set; }
    }
}
