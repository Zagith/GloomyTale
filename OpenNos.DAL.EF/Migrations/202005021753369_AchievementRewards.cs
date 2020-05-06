namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementRewards : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AchievementReward", "Name", c => c.String(maxLength: 255));
            AddColumn("dbo.AchievementReward", "Description", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AchievementReward", "Description");
            DropColumn("dbo.AchievementReward", "Name");
        }
    }
}
