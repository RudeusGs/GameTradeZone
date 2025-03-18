namespace GameTradeZone.Service.Models.GameAccountField
{
    public class AddGameAccountFieldModel
    {
        public int GameAccountId { get; set; }
        public int GameFieldId { get; set; }
        public string? FieldValue { get; set; }
    }
}
