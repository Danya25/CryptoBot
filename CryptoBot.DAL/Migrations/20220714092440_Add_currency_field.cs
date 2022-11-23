using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoBot.DAL.Migrations
{
    public partial class Add_currency_field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "UserPostsInfo",
                type: "varchar(1024)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "UserPostsInfo");
        }
    }
}
