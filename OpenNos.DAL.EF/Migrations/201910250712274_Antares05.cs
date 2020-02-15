namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Antares05 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CharacterTitle", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.CharacterTitle", "Visible", c => c.Boolean(nullable: false));
            AddColumn("dbo.CharacterTitle", "TitleType", c => c.Short(nullable: false));
            DropColumn("dbo.CharacterTitle", "TitleId");
            DropColumn("dbo.CharacterTitle", "Stat");
        }

        public override void Down()
        {
            AddColumn("dbo.CharacterTitle", "Stat", c => c.Short(nullable: false));
            AddColumn("dbo.CharacterTitle", "TitleId", c => c.Short(nullable: false));
            DropColumn("dbo.CharacterTitle", "TitleType");
            DropColumn("dbo.CharacterTitle", "Visible");
            DropColumn("dbo.CharacterTitle", "Active");
        }
    }
}
