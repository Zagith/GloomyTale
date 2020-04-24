namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOtherLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LevelLog",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        CharacterId = c.Long(nullable: false),
                        Level = c.Byte(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.RaidLog",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        CharacterId = c.Long(nullable: false),
                        RaidId = c.Long(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.UpgradeLog",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        CharacterId = c.Long(nullable: false),
                        EquipmentSerialized = c.Guid(nullable: false),
                        Upgrade = c.Short(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UpgradeLog");
            DropTable("dbo.RaidLog");
            DropTable("dbo.LevelLog");
        }
    }
}
