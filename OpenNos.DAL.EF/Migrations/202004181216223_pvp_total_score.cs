namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pvp_total_score : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "PvpScoreTotal", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "PvpScoreTotal");
        }
    }
}
