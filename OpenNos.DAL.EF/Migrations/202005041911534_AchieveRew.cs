namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchieveRew : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievement", "FirstReward", c => c.Int(nullable: false));
            AddColumn("dbo.Achievement", "FirstRewardName", c => c.String(maxLength: 255));
            AddColumn("dbo.Achievement", "SecondReward", c => c.Int(nullable: false));
            AddColumn("dbo.Achievement", "SecondRewardName", c => c.String(maxLength: 255));
            AddColumn("dbo.Achievement", "ThirdReward", c => c.Int(nullable: false));
            AddColumn("dbo.Achievement", "ThirdRewardName", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Achievement", "ThirdRewardName");
            DropColumn("dbo.Achievement", "ThirdReward");
            DropColumn("dbo.Achievement", "SecondRewardName");
            DropColumn("dbo.Achievement", "SecondReward");
            DropColumn("dbo.Achievement", "FirstRewardName");
            DropColumn("dbo.Achievement", "FirstReward");
        }
    }
}
