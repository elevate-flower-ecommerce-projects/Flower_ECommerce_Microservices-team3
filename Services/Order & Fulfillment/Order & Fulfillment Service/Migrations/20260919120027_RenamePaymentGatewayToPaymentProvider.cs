using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order___Fulfillment_Service.Migrations
{
    /// <inheritdoc />
    public partial class RenamePaymentGatewayToPaymentProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentGateway",
                table: "Orders",
                newName: "PaymentProvider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentProvider",
                table: "Orders",
                newName: "PaymentGateway");
        }
    }
}
