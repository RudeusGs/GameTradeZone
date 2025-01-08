using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class Dispute : EntityBase
    {
        public int ReporterID { get; set; }
        public int RespondentID { get; set; }
        public string Reason { get; set; }
        public string Proof {  get; set; }
        public string Status { get; set; }
        public string Reply {  get; set; }
    }
}
