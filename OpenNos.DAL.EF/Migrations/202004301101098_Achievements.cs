namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Achievements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Achievement",
                c => new
                    {
                        AchievementId = c.Long(nullable: false),
                        Name = c.String(maxLength: 255),
                        AchievementType = c.Int(nullable: false),
                        Data = c.Int(nullable: false),
                        LevelMin = c.Byte(nullable: false),
                        LevelMax = c.Byte(nullable: false),
                        IsDaily = c.Boolean(nullable: false),
                        Category = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.AchievementId);
            
            CreateTable(
                "dbo.AchievementReward",
                c => new
                    {
                        AchievementRewardId = c.Long(nullable: false, identity: true),
                        RewardType = c.Byte(nullable: false),
                        Data = c.Int(nullable: false),
                        Design = c.Byte(nullable: false),
                        Rarity = c.Byte(nullable: false),
                        Upgrade = c.Byte(nullable: false),
                        Amount = c.Int(nullable: false),
                        AchievementId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.AchievementRewardId);
            
            CreateTable(
                "dbo.CharacterAchievement",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CharacterId = c.Long(nullable: false),
                        AchievementId = c.Long(nullable: false),
                        FirstObjective = c.Int(nullable: false),
                        IsMainAchievement = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Achievement", t => t.AchievementId, cascadeDelete: true)
                .Index(t => t.AchievementId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CharacterAchievement", "AchievementId", "dbo.Achievement");
            DropIndex("dbo.CharacterAchievement", new[] { "AchievementId" });
            DropTable("dbo.CharacterAchievement");
            DropTable("dbo.AchievementReward");
            DropTable("dbo.Achievement");
        }
    }
}
