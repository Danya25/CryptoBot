using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CryptoBot.DAL.Migrations
{
    public partial class Correct_id_token : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Tokens",
                newName: "PriceUsd");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Tokens",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Tokens");

            migrationBuilder.RenameColumn(
                name: "PriceUsd",
                table: "Tokens",
                newName: "Price");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens",
                columns: new[] { "Name", "Price" });
        }
    }
}
