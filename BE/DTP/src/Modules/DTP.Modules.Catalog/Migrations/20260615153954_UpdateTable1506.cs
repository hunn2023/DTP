using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable1506 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EsimPackages_Providers_ProviderId",
                table: "EsimPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_PhoneCards_Providers_ProviderId",
                table: "PhoneCards");

            migrationBuilder.DropTable(
                name: "Providers");

            migrationBuilder.DropIndex(
                name: "IX_PhoneCards_ProviderId",
                table: "PhoneCards");

            migrationBuilder.DropIndex(
                name: "IX_EsimPackages_ProviderId",
                table: "EsimPackages");

            migrationBuilder.CreateTable(
                name: "EsimPackageCoverages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EsimPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EsimPackageCoverages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EsimPackageCoverages_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EsimPackageCoverages_EsimPackages_EsimPackageId",
                        column: x => x.EsimPackageId,
                        principalTable: "EsimPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EsimPackageCoverages_CountryId",
                table: "EsimPackageCoverages",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_EsimPackageCoverages_EsimPackageId",
                table: "EsimPackageCoverages",
                column: "EsimPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_EsimPackageCoverages_EsimPackageId_CountryId",
                table: "EsimPackageCoverages",
                columns: new[] { "EsimPackageId", "CountryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EsimPackageCoverages");

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiBaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupportEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WebhookUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhoneCards_ProviderId",
                table: "PhoneCards",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_EsimPackages_ProviderId",
                table: "EsimPackages",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_EsimPackages_Providers_ProviderId",
                table: "EsimPackages",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneCards_Providers_ProviderId",
                table: "PhoneCards",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
