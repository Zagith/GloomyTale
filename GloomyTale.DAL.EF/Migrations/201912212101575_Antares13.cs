namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares13 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Recipe", "MapNpcId");
            DropColumn("dbo.Recipe", "ProduceItemVNum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipe", "ProduceItemVNum", c => c.Short(nullable: false));
            AddColumn("dbo.Recipe", "MapNpcId", c => c.Int(nullable: false));
        }
    }
}
