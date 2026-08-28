using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Address___Store_Coverage_Service.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverageAreaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoverageAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoundaryType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RadiusMeters = table.Column<double>(type: "float", nullable: true),
                    Polygon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Areas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverageAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoverageAreas_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoverageAreas_StoreId",
                table: "CoverageAreas",
                column: "StoreId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoverageAreas");
        }
    }
}
