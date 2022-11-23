using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoBot.DAL.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.TelegramId);
                });

            migrationBuilder.CreateTable(
                name: "UserPostsInfo",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Timer = table.Column<int>(type: "int", nullable: false),
                    CryptoSet = table.Column<string>(type: "varchar(2048)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPostsInfo", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPostsInfo_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "TelegramId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPostsInfo");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
