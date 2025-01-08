using Microsoft.AspNetCore.Identity;

namespace GameTradeZone.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public decimal? Balance { get; set; }
        public int? Coin { get; set; }
        public string? Level { get; set; }
        public bool Status { get; set; }
        public string? Reliability { get; set; }
        public string? Avatar { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual DateTime? UpdatedDate { get; set; }
        public virtual DateTime? DeleteDate { get; set; }
    }
}
