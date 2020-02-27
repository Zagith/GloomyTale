using Microsoft.EntityFrameworkCore.Migrations;

namespace GloomyTale.DAL.EF.Migrations
{
    public partial class GloomyTale02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ammo",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "Cellon",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "CloseDefence",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "Concentrate",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "CriticalDodge",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "CriticalLuckRate",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "CriticalRate",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "DamageMaximum",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "DamageMinimum",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "DarkElement",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "DarkResistance",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "DefenceDodge",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "DistanceDefence",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "DistanceDefenceDodge",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "ElementRate",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "FireElement",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "FireResistance",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "HP",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "HitRate",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "HoldingVNum",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "IsEmpty",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "IsFixed",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "IsPartnerEquipment",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "LightElement",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "LightResistance",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "MP",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "MagicDefence",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "MaxElementRate",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "ShellRarity",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SlDamage",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SlDefence",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SlElement",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SlHP",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpDamage",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpDark",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpDefence",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpElement",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpFire",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpHP",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpLevel",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpLight",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpStoneUpgrade",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "SpWater",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "WaterElement",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "WaterResistance",
                table: "ItemInstance");

            migrationBuilder.DropColumn(
                name: "XP",
                table: "ItemInstance");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Ammo",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Cellon",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CloseDefence",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Concentrate",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CriticalDodge",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CriticalLuckRate",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CriticalRate",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DamageMaximum",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DamageMinimum",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "DarkElement",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DarkResistance",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DefenceDodge",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DistanceDefence",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "DistanceDefenceDodge",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "ElementRate",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "FireElement",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "FireResistance",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "HP",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "HitRate",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "HoldingVNum",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmpty",
                table: "ItemInstance",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFixed",
                table: "ItemInstance",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPartnerEquipment",
                table: "ItemInstance",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "LightElement",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "LightResistance",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "MP",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "MagicDefence",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "MaxElementRate",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "ShellRarity",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SlDamage",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SlDefence",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SlElement",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SlHP",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpDamage",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpDark",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpDefence",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpElement",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpFire",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpHP",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpLevel",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpLight",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpStoneUpgrade",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "SpWater",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WaterElement",
                table: "ItemInstance",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "WaterResistance",
                table: "ItemInstance",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "XP",
                table: "ItemInstance",
                type: "bigint",
                nullable: true);
        }
    }
}
