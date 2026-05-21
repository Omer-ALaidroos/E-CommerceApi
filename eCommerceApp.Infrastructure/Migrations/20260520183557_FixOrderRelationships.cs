using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId1",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingAddressId1",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodId1",
                table: "Orders",
                column: "PaymentMethodId1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddressId1",
                table: "Orders",
                column: "ShippingAddressId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId1",
                table: "Orders",
                column: "ShippingAddressId1",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodId1",
                table: "Orders",
                column: "PaymentMethodId1",
                principalTable: "PaymentMethods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId1",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentMethodId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingAddressId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddressId1",
                table: "Orders");
        }
    }
}
