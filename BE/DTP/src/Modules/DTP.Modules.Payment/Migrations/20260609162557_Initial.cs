using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Payment.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "payment");

            migrationBuilder.CreateTable(
                name: "PaymentCallbackLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    PaymentTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderTransactionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RawBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCallbackLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentCallbacks",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderCode = table.Column<int>(type: "int", nullable: false),
                    OrderCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderTransactionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsValidSignature = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCallbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderTransactionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProviderPaymentCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QrCode = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    QrImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PaymentUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankAccountNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankAccountName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransferContent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProviderResponseCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderResponseMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RawProviderRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawProviderResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawCallbackData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCallbackLogs_PaymentTransactionId",
                table: "PaymentCallbackLogs",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCallbackLogs_ProviderTransactionId",
                table: "PaymentCallbackLogs",
                column: "ProviderTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCallbackLogs_RequestId",
                table: "PaymentCallbackLogs",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCallbackLogs_Status",
                table: "PaymentCallbackLogs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCallbacks_OrderCode",
                schema: "payment",
                table: "PaymentCallbacks",
                column: "OrderCode");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCallbacks_PaymentTransactionId",
                schema: "payment",
                table: "PaymentCallbacks",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCallbacks_TransactionCode",
                schema: "payment",
                table: "PaymentCallbacks",
                column: "TransactionCode");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Provider_ProviderTransactionId",
                table: "PaymentTransactions",
                columns: new[] { "Provider", "ProviderTransactionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_RequestId",
                table: "PaymentTransactions",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Status",
                table: "PaymentTransactions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentCallbackLogs");

            migrationBuilder.DropTable(
                name: "PaymentCallbacks",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");
        }
    }
}
