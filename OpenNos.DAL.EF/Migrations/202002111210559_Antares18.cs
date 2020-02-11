namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "ReferToken", c => c.String());
            AlterColumn("dbo.Account", "ReferrerId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Account", "ReferrerId", c => c.Long(nullable: false));
            DropColumn("dbo.Account", "ReferToken");
        }
    }
}
