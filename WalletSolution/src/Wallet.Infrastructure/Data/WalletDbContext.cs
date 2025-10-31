using Microsoft.EntityFrameworkCore;
using Wallet.Core.Entities;

namespace Wallet.Infrastructure.Data
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

        public DbSet<WalletEntity> Wallets { get; set; } = null!;
        public DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WalletEntity>(b =>
            {
                b.ToTable("Wallets");
                b.HasKey(w => w.Id);
                b.Property(w => w.UserId).IsRequired();
                b.Property(w => w.Balance).IsRequired();
                b.Property(w => w.RowVersion).IsRowVersion();
                b.HasIndex(w => w.UserId).IsUnique();
            });

            modelBuilder.Entity<WalletTransaction>(b =>
            {
                b.ToTable("WalletTransactions");
                b.HasKey(t => t.Id);
                b.Property(t => t.Type).HasMaxLength(20).IsRequired();
                b.Property(t => t.Amount).IsRequired();
                b.Property(t => t.BalanceAfter).IsRequired();
                b.Property(t => t.CreatedAt).IsRequired();
                b.HasIndex(t => new { t.WalletId, t.ExternalReference }).IsUnique().HasFilter("[ExternalReference] IS NOT NULL");
                b.HasOne<WalletEntity>().WithMany().HasForeignKey(t => t.WalletId).OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}