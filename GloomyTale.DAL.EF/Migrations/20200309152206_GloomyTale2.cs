using Microsoft.EntityFrameworkCore.Migrations;

namespace GloomyTale.DAL.EF.Migrations
{
    public partial class GloomyTale2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReferrerId",
                table: "Account",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "AdminName",
                table: "Account",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferToken",
                table: "Account",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminName",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ReferToken",
                table: "Account");

            migrationBuilder.AlterColumn<long>(
                name: "ReferrerId",
                table: "Account",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
