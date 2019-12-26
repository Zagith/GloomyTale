namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "SideReputation", c => c.Int(nullable: false));
            AddColumn("dbo.RollGeneratedItem", "ItemGeneratedUpgrade", c => c.Byte(nullable: false));
            AddColumn("dbo.RollGeneratedItem", "IsSuperReward", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RollGeneratedItem", "IsSuperReward");
            DropColumn("dbo.RollGeneratedItem", "ItemGeneratedUpgrade");
            DropColumn("dbo.Item", "SideReputation");
        }
    }
}
