namespace GameTradeZone.Service.Models.OngoingService
{
    public class ConfirmServiceModel
    {
        public int? Id { get; set; }
        public int? ServiceID { get; set; }
        public int? UserID { get; set; }
        public string? Status { get; set; } // Đồng ý, không đồng ý, đang chờ xác nhận
        public string? Reason { get; set; } // Lý do không đồng ý
        public string? FeedBack { get; set; }
        public bool? IsDelete { get; set; }
    }
}
