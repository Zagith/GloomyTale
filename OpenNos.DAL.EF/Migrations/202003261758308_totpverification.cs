namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class totpverification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "TotpSecret", c => c.String(maxLength: 32));
            AddColumn("dbo.Account", "TotpVerified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Account", "TotpVerified");
            DropColumn("dbo.Account", "TotpSecret");
        }
    }
}
