using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class BargainAccountGame : EntityBase
    {
        public int AccountGameID { get; set; }
        public int UserID { get; set; }
        public decimal BargainPrice { get; set; }

    }
}
