using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Payment.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable2906 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentProviderCode",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentProviderCode",
                table: "PaymentTransactions");
        }
    }
}
