using Microsoft.AspNetCore.Identity;

namespace GameTradeZone.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public decimal? Balance { get; set; }
        public int? Coin { get; set; }
        //public int Experience { get; set; } //100, 400, 1000, 2000, 3500, 5600, 8500, 12000, 21000, 46000
        public int? Level { get; set; }
        public bool Status { get; set; }
        public string? Avatar { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual DateTime? DeleteDate { get; set; }
    }
}
