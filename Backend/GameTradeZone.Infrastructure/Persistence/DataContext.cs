using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GameTradeZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        public DbSet<HiredService> HiredServices { get; set; }
        public DbSet<LevelIcon> LevelIcons { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OnGoingService> OnGoingServices { get; set; }
        public DbSet<PurchasedAccount> PurchasedAccounts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<PostInfo> PostInfos { get; set; }
        public DbSet<WithDrawnMoney> WithDrawnMoneys { get; set; }
        public DbSet<RechargeCard> RechargeCards { get; set; }
        public DbSet<RechargeBank> RechargeBanks { get; set; }
        public DbSet<BankTransactionInfor> BankTransactionInfors { get; set; }
        public DbSet<SepayWebHooksReceiver> SepayWebHooksReceivers { get; set; }
        public DbSet<TransactionInfor> TransactionInfors { get; set; }
        public DbSet<CommentData> CommentDatas { get; set; }
        public DbSet<ForumsCategory> ForumsCategories { get; set; }
        public DbSet<GameAccountField> GameAccountFields { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionDetail> AuctionDetails { get; set; }
        public DbSet<AuctionPrize> AuctionPrizes { get; set; }
        public DbSet<OTPs> OTPs { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SepayWebHooksReceiver>().ToTable("SepayWebHooksReceivers");
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            builder.Entity<EmailLog>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmailLog>()
                .Property(e => e.Subject)
                .HasMaxLength(255);

            builder.Entity<EmailLog>()
                .Property(e => e.SenderEmail)
                .HasMaxLength(255);

            builder.Entity<EmailLog>()
                .Property(e => e.ReceiverEmail)
                .HasMaxLength(255);

            builder.Entity<EmailLog>()
                .Property(e => e.ErrorMessage)
                .HasMaxLength(1000);
        }
    }
}