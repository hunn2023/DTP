using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTP.Modules.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetOtpExpiredAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetOtpHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PasswordResetVerifyFailedCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetOtpExpiredAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetOtpHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetVerifyFailedCount",
                table: "Users");
        }
    }
}
