namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Antares16 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimeSpacesLog",
                c => new
                {
                    LogId = c.Long(nullable: false, identity: true),
                    CharacterId = c.Long(),
                    TimeSpaceId = c.Long(nullable: false),
                    Timestamp = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.LogId);

        }

        public override void Down()
        {
            DropTable("dbo.TimeSpacesLog");
        }
    }
}
