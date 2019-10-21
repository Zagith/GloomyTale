namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares03 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CharacterTitle",
                c => new
                    {
                        CharacterTitleId = c.Long(nullable: false, identity: true),
                        CharacterId = c.Long(nullable: false),
                        TitleId = c.Long(nullable: false),
                        IsUsed = c.Boolean(nullable: false),
                        IsDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CharacterTitleId)
                .ForeignKey("dbo.Character", t => t.CharacterId, cascadeDelete: true)
                .Index(t => t.CharacterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CharacterTitle", "CharacterId", "dbo.Character");
            DropIndex("dbo.CharacterTitle", new[] { "CharacterId" });
            DropTable("dbo.CharacterTitle");
        }
    }
}
