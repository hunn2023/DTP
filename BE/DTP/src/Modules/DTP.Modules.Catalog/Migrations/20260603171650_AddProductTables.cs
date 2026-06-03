using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarrierId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CardType",
                table: "PhoneCards");

            migrationBuilder.DropColumn(
                name: "Instruction",
                table: "PhoneCards");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "PhoneCards");

            migrationBuilder.DropColumn(
                name: "ActivationPolicy",
                table: "EsimPackages");

            migrationBuilder.DropColumn(
                name: "HotspotSupported",
                table: "EsimPackages");

            migrationBuilder.DropColumn(
                name: "KycRequired",
                table: "EsimPackages");

            migrationBuilder.DropColumn(
                name: "PhoneNumberSupported",
                table: "EsimPackages");

            migrationBuilder.DropColumn(
                name: "SpeedPolicy",
                table: "EsimPackages");

            migrationBuilder.RenameColumn(
                name: "ProviderProductCode",
                table: "PhoneCards",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "DeliveryType",
                table: "PhoneCards",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SmsSupported",
                table: "EsimPackages",
                newName: "IsUnlimited");

            migrationBuilder.RenameColumn(
                name: "QrDeliveryType",
                table: "EsimPackages",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "ProviderPackageCode",
                table: "EsimPackages",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "EsimPackages",
                newName: "CountryId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "EsimPackages",
                newName: "CarrierId");

            migrationBuilder.RenameColumn(
                name: "CoverageType",
                table: "EsimPackages",
                newName: "Currency");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "PhoneCards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "PhoneCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "DataAmount",
                table: "EsimPackages",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "EsimPackages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "EsimPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PhoneCards_ProductVariantId",
                table: "PhoneCards",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneCards_ProviderId",
                table: "PhoneCards",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_EsimPackages_CarrierId",
                table: "EsimPackages",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_EsimPackages_CountryId",
                table: "EsimPackages",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_EsimPackages_ProductVariantId",
                table: "EsimPackages",
                column: "ProductVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_EsimPackages_Carriers_CarrierId",
                table: "EsimPackages",
                column: "CarrierId",
                principalTable: "Carriers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EsimPackages_Country_CountryId",
                table: "EsimPackages",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EsimPackages_ProductVariants_ProductVariantId",
                table: "EsimPackages",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneCards_ProductVariants_ProductVariantId",
                table: "PhoneCards",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneCards_Providers_ProviderId",
                table: "PhoneCards",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EsimPackages_Carriers_CarrierId",
                table: "EsimPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_EsimPackages_Country_CountryId",
                table: "EsimPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_EsimPackages_ProductVariants_ProductVariantId",
                table: "EsimPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_PhoneCards_ProductVariants_ProductVariantId",
                table: "PhoneCards");

            migrationBuilder.DropForeignKey(
                name: "FK_PhoneCards_Providers_ProviderId",
                table: "PhoneCards");

            migrationBuilder.DropIndex(
                name: "IX_PhoneCards_ProductVariantId",
                table: "PhoneCards");

            migrationBuilder.DropIndex(
                name: "IX_PhoneCards_ProviderId",
                table: "PhoneCards");

            migrationBuilder.DropIndex(
                name: "IX_EsimPackages_CarrierId",
                table: "EsimPackages");

            migrationBuilder.DropIndex(
                name: "IX_EsimPackages_CountryId",
                table: "EsimPackages");

            migrationBuilder.DropIndex(
                name: "IX_EsimPackages_ProductVariantId",
                table: "EsimPackages");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "PhoneCards");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "PhoneCards");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "EsimPackages");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "EsimPackages");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "PhoneCards",
                newName: "ProviderProductCode");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PhoneCards",
                newName: "DeliveryType");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "EsimPackages",
                newName: "QrDeliveryType");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "EsimPackages",
                newName: "ProviderPackageCode");

            migrationBuilder.RenameColumn(
                name: "IsUnlimited",
                table: "EsimPackages",
                newName: "SmsSupported");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "EsimPackages",
                newName: "CoverageType");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "EsimPackages",
                newName: "ProviderId");

            migrationBuilder.RenameColumn(
                name: "CarrierId",
                table: "EsimPackages",
                newName: "ProductId");

            migrationBuilder.AddColumn<Guid>(
                name: "CarrierId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardType",
                table: "PhoneCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instruction",
                table: "PhoneCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "PhoneCards",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "DataAmount",
                table: "EsimPackages",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ActivationPolicy",
                table: "EsimPackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HotspotSupported",
                table: "EsimPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "KycRequired",
                table: "EsimPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberSupported",
                table: "EsimPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SpeedPolicy",
                table: "EsimPackages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
