namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogChat",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        CharacterId = c.Long(),
                        ChatType = c.Byte(nullable: false),
                        ChatMessage = c.String(maxLength: 255),
                        IpAddress = c.String(maxLength: 255),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId);
            
            CreateTable(
                "dbo.LogCommands",
                c => new
                    {
                        CommandId = c.Long(nullable: false, identity: true),
                        CharacterId = c.Long(),
                        Command = c.String(),
                        Data = c.String(),
                        IpAddress = c.String(maxLength: 255),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CommandId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId);
            
            CreateTable(
                "dbo.LogDrop",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        CharacterId = c.Long(),
                        ItemVNum = c.Short(nullable: false),
                        ItemName = c.String(maxLength: 255),
                        Amount = c.Short(nullable: false),
                        Map = c.Short(nullable: false),
                        X = c.Byte(nullable: false),
                        Y = c.Byte(nullable: false),
                        IpAddress = c.String(maxLength: 255),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.CharacterId);
            
            CreateTable(
                "dbo.LogPutItem",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        CharacterId = c.Long(),
                        ItemVNum = c.Short(nullable: false),
                        Amount = c.Short(nullable: false),
                        Map = c.Short(nullable: false),
                        X = c.Byte(nullable: false),
                        Y = c.Byte(nullable: false),
                        IpAddress = c.String(maxLength: 255),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LogDrop", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.LogCommands", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.LogChat", "CharacterId", "dbo.Character");
            DropIndex("dbo.LogDrop", new[] { "CharacterId" });
            DropIndex("dbo.LogCommands", new[] { "CharacterId" });
            DropIndex("dbo.LogChat", new[] { "CharacterId" });
            DropTable("dbo.LogPutItem");
            DropTable("dbo.LogDrop");
            DropTable("dbo.LogCommands");
            DropTable("dbo.LogChat");
        }
    }
}
