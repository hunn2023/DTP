using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DTP.Modules.Payment.Migrations
{
    /// <inheritdoc />
    public partial class AddProvinder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_Provider_ProviderTransactionId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ProviderCode",
                schema: "payment",
                table: "PaymentCallbacks");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "PaymentCallbackLogs");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentProviderId",
                table: "PaymentTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentProviderId",
                schema: "payment",
                table: "PaymentCallbacks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentProviderId",
                table: "PaymentCallbackLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PaymentProviders",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentProviders", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "payment",
                table: "PaymentProviders",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Currency", "Description", "IsActive", "IsDefault", "IsDeleted", "LogoUrl", "MaxAmount", "MinAmount", "Name", "PaymentMethod", "SortOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "SEPAY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "VND", "Thanh toán bằng mã QR ngân hàng qua SePay.", true, true, false, "/images/payment/sepay.png", 50000000m, 1000m, "SePay QR", "BankQr", 1, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "VNPT_EPAY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "VND", "Thanh toán QR qua VNPT ePay.", true, false, false, "/images/payment/vnpt-epay.png", 50000000m, 1000m, "VNPT ePay", "BankQr", 2, null, null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "MOMO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "VND", "Thanh toán ví điện tử MoMo.", false, false, false, "/images/payment/momo.png", 20000000m, 1000m, "MoMo", "EWallet", 3, null, null },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "VNPAY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "VND", "Thanh toán qua VNPAY.", false, false, false, "/images/payment/vnpay.png", 50000000m, 1000m, "VNPAY", "BankQr", 4, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentProviderId_ProviderTransactionId",
                table: "PaymentTransactions",
                columns: new[] { "PaymentProviderId", "ProviderTransactionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviders_Code",
                schema: "payment",
                table: "PaymentProviders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviders_IsActive_SortOrder",
                schema: "payment",
                table: "PaymentProviders",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviders_IsDefault",
                schema: "payment",
                table: "PaymentProviders",
                column: "IsDefault",
                unique: true,
                filter: "[IsDefault] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentProviders",
                schema: "payment");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_PaymentProviderId_ProviderTransactionId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentProviderId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentProviderId",
                schema: "payment",
                table: "PaymentCallbacks");

            migrationBuilder.DropColumn(
                name: "PaymentProviderId",
                table: "PaymentCallbackLogs");

            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "PaymentTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProviderCode",
                schema: "payment",
                table: "PaymentCallbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "PaymentCallbackLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Provider_ProviderTransactionId",
                table: "PaymentTransactions",
                columns: new[] { "Provider", "ProviderTransactionId" });
        }
    }
}
