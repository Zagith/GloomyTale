namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Antares07 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.I18NItem",
                c => new
                {
                    I18NItemId = c.Int(nullable: false, identity: true),
                    Key = c.String(),
                    RegionType = c.Int(nullable: false),
                    Text = c.String(),
                })
                .PrimaryKey(t => t.I18NItemId);

            AddColumn("dbo.Account", "Language", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Account", "Language");
            DropTable("dbo.I18NItem");
        }
    }
}
