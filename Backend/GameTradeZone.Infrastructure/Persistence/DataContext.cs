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
        public DbSet<GameInfor> GameInfors { get; set; }
        public DbSet<GameField> GameFields { get; set; }
        public DbSet<AccountGame> AccountGames { get; set; }
        public DbSet<Dispute> Disputes { get; set; }

    }
}
