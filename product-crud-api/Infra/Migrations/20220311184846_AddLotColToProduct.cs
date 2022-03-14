using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace product_crud_api.Migrations
{
    public partial class AddLotColToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lot",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lot",
                table: "Products");
        }
    }
}
