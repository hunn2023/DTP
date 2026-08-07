using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverageOperators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Operators",
                table: "EsimPackageCoverages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Operators",
                table: "EsimPackageCoverages");
        }
    }
}
