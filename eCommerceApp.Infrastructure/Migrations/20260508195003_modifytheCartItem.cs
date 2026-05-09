using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modifytheCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         

           
            migrationBuilder.AlterColumn<string>(
             name: "UserId",
             table: "CheckoutAchieves",
             type: "nvarchar(max)",
             nullable: false,
             defaultValue: "",
             oldClrType: typeof(string),
             oldType: "nvarchar(max)",
             oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAtTime",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems");
        }
    }
}
