using Microsoft.EntityFrameworkCore;
using Wallet.Core.Entities;

namespace Wallet.Infrastructure.Data
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

        public DbSet<WalletEntity> Wallets { get; set; } = null!;
        public DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ServiceEntity> Services { get; set; } = null!;
        public DbSet<ConfigurationRule> ConfigurationRules { get; set; } = null!;

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

            // User
            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.DisplayName).HasMaxLength(200);
                b.Property(x => x.email).HasMaxLength(200);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Service
            modelBuilder.Entity<ServiceEntity>(b =>
            {
                b.ToTable("Services");
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired().HasMaxLength(200);
                b.Property(x => x.Description).HasMaxLength(500);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // ConfigurationRule
            modelBuilder.Entity<ConfigurationRule>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.RuleType).IsRequired().HasMaxLength(50);
                b.Property(x => x.PointsPerBaseAmount).IsRequired();
                b.Property(x => x.BaseAmount).HasColumnType("decimal(18,2)").IsRequired();
                b.Property(x => x.IsDefault).HasDefaultValue(false);
                b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                b.HasIndex(x => new { x.ServiceId, x.RuleType });
            });

            // If you want a FK constraint from ConfigurationRule.ServiceId to Services:
            modelBuilder.Entity<ConfigurationRule>()
                .HasOne<ServiceEntity>()
                .WithMany()
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
    }
}