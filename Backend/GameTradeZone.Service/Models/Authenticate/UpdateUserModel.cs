namespace GameTradeZone.Service.Models.Authenticate
{
    public class UpdateUserModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string BankName { get; set; }
        public string BankNumber { get; set; }
    }
}