using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GameTradeZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameTradeZone.Infrastructure.Persistence
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
