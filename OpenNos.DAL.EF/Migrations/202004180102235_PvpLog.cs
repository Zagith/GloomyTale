namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PvpLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PvPLog",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        CharacterId = c.Long(nullable: false),
                        TargetId = c.Long(nullable: false),
                        IpAddress = c.String(maxLength: 255),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Character", t => t.CharacterId, cascadeDelete: true)
                .Index(t => t.CharacterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PvPLog", "CharacterId", "dbo.Character");
            DropIndex("dbo.PvPLog", new[] { "CharacterId" });
            DropTable("dbo.PvPLog");
        }
    }
}
