using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminLoginAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminLoginAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Outcome = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminLoginAudits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminLoginAudits_Email",
                table: "AdminLoginAudits",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AdminLoginAudits_IpAddress",
                table: "AdminLoginAudits",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_AdminLoginAudits_Timestamp",
                table: "AdminLoginAudits",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminLoginAudits");
        }
    }
}
