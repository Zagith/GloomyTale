namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchieveRewardRew : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AchievementReward", "ItemVNum", c => c.Int(nullable: false));
            AddColumn("dbo.AchievementReward", "ItemName", c => c.String(maxLength: 255));
            DropColumn("dbo.AchievementReward", "Data");
            DropColumn("dbo.AchievementReward", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AchievementReward", "Name", c => c.String(maxLength: 255));
            AddColumn("dbo.AchievementReward", "Data", c => c.Int(nullable: false));
            DropColumn("dbo.AchievementReward", "ItemName");
            DropColumn("dbo.AchievementReward", "ItemVNum");
        }
    }
}
