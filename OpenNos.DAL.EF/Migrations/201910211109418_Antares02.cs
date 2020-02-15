namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Antares02 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LogDrop", "X", c => c.Short(nullable: false));
            AlterColumn("dbo.LogDrop", "Y", c => c.Short(nullable: false));
            AlterColumn("dbo.LogPutItem", "X", c => c.Short(nullable: false));
            AlterColumn("dbo.LogPutItem", "Y", c => c.Short(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.LogPutItem", "Y", c => c.Byte(nullable: false));
            AlterColumn("dbo.LogPutItem", "X", c => c.Byte(nullable: false));
            AlterColumn("dbo.LogDrop", "Y", c => c.Byte(nullable: false));
            AlterColumn("dbo.LogDrop", "X", c => c.Byte(nullable: false));
        }
    }
}
