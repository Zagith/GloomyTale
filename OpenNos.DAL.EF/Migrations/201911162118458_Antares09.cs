namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares09 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CharacterTitle", "CharacterId", "dbo.Character");
            CreateTable(
                "dbo.FortuneWheel",
                c => new
                    {
                        TentaLaFortunaId = c.Short(nullable: false, identity: true),
                        IsRareRandom = c.Boolean(nullable: false),
                        Rare = c.Byte(nullable: false),
                        Upgrade = c.Byte(nullable: false),
                        ItemGeneratedVNum = c.Short(nullable: false),
                        Probability = c.Short(nullable: false),
                        ShopId = c.Short(nullable: false),
                        Shop_ShopId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TentaLaFortunaId)
                .ForeignKey("dbo.Shop", t => t.Shop_ShopId)
                .ForeignKey("dbo.Item", t => t.ItemGeneratedVNum)
                .Index(t => t.ItemGeneratedVNum)
                .Index(t => t.Shop_ShopId);
            
            AddForeignKey("dbo.CharacterTitle", "CharacterId", "dbo.Character", "CharacterId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CharacterTitle", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.FortuneWheel", "ItemGeneratedVNum", "dbo.Item");
            DropForeignKey("dbo.FortuneWheel", "Shop_ShopId", "dbo.Shop");
            DropIndex("dbo.FortuneWheel", new[] { "Shop_ShopId" });
            DropIndex("dbo.FortuneWheel", new[] { "ItemGeneratedVNum" });
            DropTable("dbo.FortuneWheel");
            AddForeignKey("dbo.CharacterTitle", "CharacterId", "dbo.Character", "CharacterId", cascadeDelete: true);
        }
    }
}
