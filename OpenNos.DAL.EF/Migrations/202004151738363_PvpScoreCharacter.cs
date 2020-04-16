namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PvpScoreCharacter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "PvpScore", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "PvpScore");
        }
    }
}
