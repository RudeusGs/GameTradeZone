using GameTradeZone.Domain.Base;

namespace GameTradeZone.Domain.Entities
{
    public class EmailLog : EntityBase
    {
        public int UserId { get; set; } 
        public string? Subject { get; set; } 
        public string? MessageBody { get; set; }
        public string? SenderEmail { get; set; }
        public string? ReceiverEmail { get; set; } 
        public DateTime SentDate { get; set; } 
        public bool IsSuccess { get; set; } 
        public string? ErrorMessage { get; set; } 
    }
}