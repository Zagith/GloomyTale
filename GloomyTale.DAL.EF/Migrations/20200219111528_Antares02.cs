using Microsoft.EntityFrameworkCore.Migrations;

namespace GloomyTale.DAL.EF.Migrations
{
    public partial class Antares02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MapTypeMap",
                table: "MapTypeMap");

            migrationBuilder.AddColumn<short>(
                name: "MapTypeMapId",
                table: "MapTypeMap",
                nullable: false,
                defaultValue: (short)0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MapTypeMap",
                table: "MapTypeMap",
                column: "MapTypeMapId");

            migrationBuilder.CreateIndex(
                name: "IX_MapTypeMap_MapTypeId",
                table: "MapTypeMap",
                column: "MapTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MapTypeMap",
                table: "MapTypeMap");

            migrationBuilder.DropIndex(
                name: "IX_MapTypeMap_MapTypeId",
                table: "MapTypeMap");

            migrationBuilder.DropColumn(
                name: "MapTypeMapId",
                table: "MapTypeMap");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MapTypeMap",
                table: "MapTypeMap",
                column: "MapTypeId");
        }
    }
}
