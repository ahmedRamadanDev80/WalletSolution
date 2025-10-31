using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Wallet.Infrastructure.Data;


namespace Wallet.Infrastructure.Design
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WalletDbContext>
    {
        public WalletDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<WalletDbContext>();
            var conn = config.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=WalletDemoDb;Trusted_Connection=True;";
            builder.UseSqlServer(conn);
            return new WalletDbContext(builder.Options);
        }
    }
}
