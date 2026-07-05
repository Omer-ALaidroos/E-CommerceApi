using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteColumnImageUrlFromProductClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
