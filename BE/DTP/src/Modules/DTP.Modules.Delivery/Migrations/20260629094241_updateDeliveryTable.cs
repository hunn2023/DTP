using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Delivery.Migrations
{
    /// <inheritdoc />
    public partial class updateDeliveryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProviderRedeemId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderRedeemId",
                table: "Deliveries");
        }
    }
}
