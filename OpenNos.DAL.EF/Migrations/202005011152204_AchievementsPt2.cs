namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementsPt2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Achievement", "Data2", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Achievement", "Data2");
        }
    }
}
