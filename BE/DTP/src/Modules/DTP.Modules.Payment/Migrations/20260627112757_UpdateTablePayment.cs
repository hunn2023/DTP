using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Payment.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PaymentProviders",
                schema: "payment",
                newName: "PaymentProviders");

            migrationBuilder.RenameTable(
                name: "PaymentCallbacks",
                schema: "payment",
                newName: "PaymentCallbacks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "payment");

            migrationBuilder.RenameTable(
                name: "PaymentProviders",
                newName: "PaymentProviders",
                newSchema: "payment");

            migrationBuilder.RenameTable(
                name: "PaymentCallbacks",
                newName: "PaymentCallbacks",
                newSchema: "payment");
        }
    }
}
