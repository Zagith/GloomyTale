namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HeroicShells : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemInstance", "IsHeroicShell", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemInstance", "IsHeroicShell");
        }
    }
}
