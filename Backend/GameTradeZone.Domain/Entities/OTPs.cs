namespace GameTradeZone.Domain.Entities
{
    public class OTPs
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OTP { get; set; }
        public string Email { get; set; }
        public DateTime? ExpirationTime { get; set; }
    }
}