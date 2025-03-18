using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class GameAccountField : EntityBase
    {
        public int GameAccountId { get; set; }
        public int GameFieldId { get; set; }
        public string? FieldValue { get; set; }
    }
}
