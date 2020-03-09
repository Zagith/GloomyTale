using Microsoft.EntityFrameworkCore.Migrations;

namespace GloomyTale.DAL.EF.Migrations
{
    public partial class GloomyTale3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoxItem");

            migrationBuilder.DropColumn(
                name: "WearableInstance_HP",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "WearableInstance_MP",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ItemInstance");

            migrationBuilder.AddColumn<short>(
                name: "HoldingVNum",
                table: "ItemInstance",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoldingVNum",
                table: "ItemInstance");

            migrationBuilder.AddColumn<short>(
                name: "WearableInstance_HP",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "WearableInstance_MP",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ItemInstance",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BoxItem",
                columns: table => new
                {
                    BoxItemId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemGeneratedAmount = table.Column<short>(type: "smallint", nullable: false),
                    ItemGeneratedDesign = table.Column<short>(type: "smallint", nullable: false),
                    ItemGeneratedRare = table.Column<byte>(type: "tinyint", nullable: false),
                    ItemGeneratedUpgrade = table.Column<byte>(type: "tinyint", nullable: false),
                    ItemGeneratedVNum = table.Column<short>(type: "smallint", nullable: false),
                    OriginalItemDesign = table.Column<short>(type: "smallint", nullable: false),
                    OriginalItemVNum = table.Column<short>(type: "smallint", nullable: false),
                    Probability = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxItem", x => x.BoxItemId);
                });
        }
    }
}
