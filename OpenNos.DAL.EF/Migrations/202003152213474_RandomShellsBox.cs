namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RandomShellsBox : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RollGeneratedItem", "ItemGeneratedRare", c => c.Byte(nullable: false));
            AddColumn("dbo.RollGeneratedItem", "ItemGeneratedUpgradeMax", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RollGeneratedItem", "ItemGeneratedUpgradeMax");
            DropColumn("dbo.RollGeneratedItem", "ItemGeneratedRare");
        }
    }
}
