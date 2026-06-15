using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Provider.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProviderApiLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderApiLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderFulfillmentLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderSku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QrCodeUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ActivationCode = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderFulfillmentLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DtpOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderOrderPublicId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NumOfProduct = table.Column<int>(type: "int", nullable: false),
                    ProviderStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReservedUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RawCreateResponseJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawConfirmResponseJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderPackageProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderSku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderProductId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Regional = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RegionId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SyncStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RawPackageJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RawDetailJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderPackageProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderProductMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderSku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderProductId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EsimPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MappingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderProductMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApiKey = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SecretKey = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DtpOrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderProductId = table.Column<int>(type: "int", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    RawSerialsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderOrderItems_ProviderOrders_ProviderOrderId",
                        column: x => x.ProviderOrderId,
                        principalTable: "ProviderOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderRedeems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderOrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DtpOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DtpOrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Serial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RedeemStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductType = table.Column<int>(type: "int", nullable: true),
                    PackageName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Iccid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Imsi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActivationCode = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    QrCodeUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ShortUrlApple = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ShortUrlAndroid = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Apn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PolicyNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PolicyUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PolicyCertificate = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EmailSent = table.Column<bool>(type: "bit", nullable: false),
                    EmailSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RawRedeemResponseJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawRedeemInfoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastCheckedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderRedeems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderRedeems_ProviderOrderItems_ProviderOrderItemId",
                        column: x => x.ProviderOrderItemId,
                        principalTable: "ProviderOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderApiLogs_CreatedAt",
                table: "ProviderApiLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderApiLogs_ProviderId",
                table: "ProviderApiLogs",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderFulfillmentLogs_OrderItemId",
                table: "ProviderFulfillmentLogs",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderFulfillmentLogs_ProviderSku",
                table: "ProviderFulfillmentLogs",
                column: "ProviderSku");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOrderItems_DtpOrderItemId",
                table: "ProviderOrderItems",
                column: "DtpOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOrderItems_ProviderOrderId",
                table: "ProviderOrderItems",
                column: "ProviderOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOrderItems_ProviderOrderId_Sku",
                table: "ProviderOrderItems",
                columns: new[] { "ProviderOrderId", "Sku" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOrders_DtpOrderId",
                table: "ProviderOrders",
                column: "DtpOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderOrders_ProviderOrderPublicId",
                table: "ProviderOrders",
                column: "ProviderOrderPublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPackageProducts_ProviderId_ProviderSku",
                table: "ProviderPackageProducts",
                columns: new[] { "ProviderId", "ProviderSku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProductMappings_EsimPackageId",
                table: "ProviderProductMappings",
                column: "EsimPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProductMappings_ProviderId_ProviderSku",
                table: "ProviderProductMappings",
                columns: new[] { "ProviderId", "ProviderSku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRedeems_DtpOrderId",
                table: "ProviderRedeems",
                column: "DtpOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRedeems_DtpOrderItemId",
                table: "ProviderRedeems",
                column: "DtpOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRedeems_ProviderOrderItemId",
                table: "ProviderRedeems",
                column: "ProviderOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRedeems_Serial",
                table: "ProviderRedeems",
                column: "Serial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderRedeems_Status_EmailSent",
                table: "ProviderRedeems",
                columns: new[] { "Status", "EmailSent" });

            migrationBuilder.CreateIndex(
                name: "IX_Providers_Code",
                table: "Providers",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderApiLogs");

            migrationBuilder.DropTable(
                name: "ProviderFulfillmentLogs");

            migrationBuilder.DropTable(
                name: "ProviderPackageProducts");

            migrationBuilder.DropTable(
                name: "ProviderProductMappings");

            migrationBuilder.DropTable(
                name: "ProviderRedeems");

            migrationBuilder.DropTable(
                name: "Providers");

            migrationBuilder.DropTable(
                name: "ProviderOrderItems");

            migrationBuilder.DropTable(
                name: "ProviderOrders");
        }
    }
}
