using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserAndServicesRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) insert demo user with fixed GUID
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "email", "DisplayName", "CreatedAt" },
                values: new object[]
                {
                    new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                    "demo@wallet.local",
                    "Demo User",
                    DateTime.UtcNow
                });

            // 2) insert 3 services
            var svc1 = new Guid("11111111-1111-1111-1111-111111111111"); // Car Wash
            var svc2 = new Guid("22222222-2222-2222-2222-222222222222"); // Towing
            var svc3 = new Guid("33333333-3333-3333-3333-333333333333"); // Rental

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Name", "Description", "CreatedAt" },
                values: new object[,]
                {
                    { svc1, "Car Wash", "Standard car wash service", DateTime.UtcNow },
                    { svc2, "Towing", "Roadside towing service", DateTime.UtcNow },
                    { svc3, "Rentals", "Vehicle rental service", DateTime.UtcNow }
                });

            // 3) insert configuration rules (PointsPerBaseAmount: int, BaseAmount: decimal)
            //    Example rules:
            //    - Car Wash: 10 points per 100 SAR
            //    - Towing: 5 points per 100 SAR
            //    - Rentals: 2 points per 100 SAR

            migrationBuilder.InsertData(
                table: "ConfigurationRules",
                columns: new[] { "Id", "ServiceId", "RuleType", "PointsPerBaseAmount", "BaseAmount", "IsDefault", "CreatedAt" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0001-0001-0001-000000000001"), svc1, "EARNING", 10, 100.00m, false, DateTime.UtcNow },
                    { new Guid("aaaaaaaa-0001-0001-0001-000000000002"), svc2, "EARNING", 5, 100.00m, false, DateTime.UtcNow },
                    { new Guid("aaaaaaaa-0001-0001-0001-000000000003"), svc3, "EARNING", 2, 100.00m, false, DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // delete rules
            migrationBuilder.DeleteData(
                table: "ConfigurationRules",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new Guid("aaaaaaaa-0001-0001-0001-000000000001"),
                    new Guid("aaaaaaaa-0001-0001-0001-000000000002"),
                    new Guid("aaaaaaaa-0001-0001-0001-000000000003")
                });

            // delete services
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new Guid("11111111-1111-1111-1111-111111111111"),
                    new Guid("22222222-2222-2222-2222-222222222222"),
                    new Guid("33333333-3333-3333-3333-333333333333")
                });

            // delete user
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"));
        }
    }
}
