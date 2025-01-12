using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class GameField : EntityBase
    {
        public int? GameInforID { get; set; }
        public string? FieldName { get; set; }
        public bool? IsDelete { get; set; }
    }
}
