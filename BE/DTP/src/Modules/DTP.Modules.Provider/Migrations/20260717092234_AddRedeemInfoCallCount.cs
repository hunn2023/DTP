using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Provider.Migrations
{
    /// <inheritdoc />
    public partial class AddRedeemInfoCallCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRedeemInfoCallAt",
                table: "ProviderRedeems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RedeemInfoCallCount",
                table: "ProviderRedeems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRedeemInfoCallAt",
                table: "ProviderRedeems");

            migrationBuilder.DropColumn(
                name: "RedeemInfoCallCount",
                table: "ProviderRedeems");
        }
    }
}
