namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Antares06 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "SecondPassword", c => c.String());
            DropColumn("dbo.Character", "LockCode");
            DropColumn("dbo.Character", "VerifiedLock");
        }

        public override void Down()
        {
            AddColumn("dbo.Character", "VerifiedLock", c => c.Boolean(nullable: false));
            AddColumn("dbo.Character", "LockCode", c => c.String());
            DropColumn("dbo.Character", "SecondPassword");
        }
    }
}
