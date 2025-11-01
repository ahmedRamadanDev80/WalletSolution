using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var userId = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
            var walletId = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "UserId", "Balance"},
                values: new object[] { walletId, userId, 1000m }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")
            );
        }
    }
}
