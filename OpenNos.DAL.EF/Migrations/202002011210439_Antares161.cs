namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares161 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "AdminName", c => c.String(maxLength: 255));
            AddColumn("dbo.Map", "GoldMapRate", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Map", "GoldMapRate");
            DropColumn("dbo.Account", "AdminName");
        }
    }
}
