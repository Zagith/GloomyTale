namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Antares10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Map", "MeteoriteLevel", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Map", "MeteoriteLevel");
        }
    }
}
