namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OptimizeSecondaryAuth : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Account", "TotpVerified");
            DropColumn("dbo.Character", "SecondPassword");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Character", "SecondPassword", c => c.String());
            AddColumn("dbo.Account", "TotpVerified", c => c.Boolean(nullable: false));
        }
    }
}
