using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order___Fulfillment_Service.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverFulfillment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedDriverId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DeliveryLatitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DeliveryLongitude",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "DriverLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActiveOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Lat = table.Column<double>(type: "float", nullable: false),
                    Lng = table.Column<double>(type: "float", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverLocations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AssignedDriverId",
                table: "Orders",
                column: "AssignedDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_AssignedDriverId",
                table: "Orders",
                columns: new[] { "Status", "AssignedDriverId" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverLocations_ActiveOrderId",
                table: "DriverLocations",
                column: "ActiveOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverLocations_DriverId",
                table: "DriverLocations",
                column: "DriverId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverLocations");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AssignedDriverId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_AssignedDriverId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AssignedDriverId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryLatitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryLongitude",
                table: "Orders");
        }
    }
}
