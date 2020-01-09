namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares08 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.I18NActDesc",
                c => new
                    {
                        I18NActDescId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NActDescId);
            
            CreateTable(
                "dbo.I18NBCard",
                c => new
                    {
                        I18NbCardId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NbCardId);
            
            CreateTable(
                "dbo.I18NCard",
                c => new
                    {
                        I18NCardId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NCardId);
            
            CreateTable(
                "dbo.I18NMapIdData",
                c => new
                    {
                        I18NMapIdDataId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NMapIdDataId);
            
            CreateTable(
                "dbo.I18NMapPointData",
                c => new
                    {
                        I18NMapPointDataId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NMapPointDataId);
            
            CreateTable(
                "dbo.I18NNpcMonster",
                c => new
                    {
                        I18NNpcMonsterId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NNpcMonsterId);
            
            CreateTable(
                "dbo.I18NNpcMonsterTalk",
                c => new
                    {
                        I18NNpcMonsterTalkId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NNpcMonsterTalkId);
            
            CreateTable(
                "dbo.I18NQuest",
                c => new
                    {
                        I18NQuestId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NQuestId);
            
            CreateTable(
                "dbo.I18NSkill",
                c => new
                    {
                        I18NSkillId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NSkillId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.I18NSkill");
            DropTable("dbo.I18NQuest");
            DropTable("dbo.I18NNpcMonsterTalk");
            DropTable("dbo.I18NNpcMonster");
            DropTable("dbo.I18NMapPointData");
            DropTable("dbo.I18NMapIdData");
            DropTable("dbo.I18NCard");
            DropTable("dbo.I18NBCard");
            DropTable("dbo.I18NActDesc");
        }
    }
}
