namespace GameTradeZone.Service.Models.Service
{
    public class AddServiceModel
    {
        public string GameInforID { get; set; }
        public string ServiceName { get; set; }
        public string Decription { get; set; }
        public decimal ServicePrice { get; set; }
        public DateTime ServiceTime { get; set; } // Thời gian ước tính khi làm dịch vụ đó 
        public string Image { get; set; }
    }
}
