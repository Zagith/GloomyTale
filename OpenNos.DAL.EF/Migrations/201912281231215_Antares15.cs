namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "Contributi", c => c.Int(nullable: false));
            AddColumn("dbo.NpcMonster", "MinLevelXP", c => c.Byte(nullable: false));
            AddColumn("dbo.NpcMonster", "MaxLevelXP", c => c.Byte(nullable: false));
            AddColumn("dbo.NpcMonster", "Contributi", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NpcMonster", "Contributi");
            DropColumn("dbo.NpcMonster", "MaxLevelXP");
            DropColumn("dbo.NpcMonster", "MinLevelXP");
            DropColumn("dbo.Character", "Contributi");
        }
    }
}
