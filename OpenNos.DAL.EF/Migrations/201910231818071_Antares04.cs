namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Antares04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CharacterTitle", "Stat", c => c.Short(nullable: false));
            AlterColumn("dbo.CharacterTitle", "TitleId", c => c.Short(nullable: false));
            DropColumn("dbo.CharacterTitle", "IsUsed");
            DropColumn("dbo.CharacterTitle", "IsDisplay");
        }

        public override void Down()
        {
            AddColumn("dbo.CharacterTitle", "IsDisplay", c => c.Boolean(nullable: false));
            AddColumn("dbo.CharacterTitle", "IsUsed", c => c.Boolean(nullable: false));
            AlterColumn("dbo.CharacterTitle", "TitleId", c => c.Long(nullable: false));
            DropColumn("dbo.CharacterTitle", "Stat");
        }
    }
}
