using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceSessionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserDevices",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationsEnabled",
                table: "UserDevices",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiresAt",
                table: "UserDevices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId_IsActive_NotificationsEnabled",
                table: "UserDevices",
                columns: new[] { "UserId", "IsActive", "NotificationsEnabled" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDevices_UserId_IsActive_NotificationsEnabled",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "NotificationsEnabled",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiresAt",
                table: "UserDevices");
        }
    }
}
