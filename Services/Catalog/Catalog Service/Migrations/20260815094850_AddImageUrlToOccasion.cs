using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog_Service.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToOccasion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "catalog",
                table: "Occasions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "catalog",
                table: "Occasions");
        }
    }
}
