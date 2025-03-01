namespace GameTradeZone.Service.Models.Authenticate
{
    public class RegisterModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
