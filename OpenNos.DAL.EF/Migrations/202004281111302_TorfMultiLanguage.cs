namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TorfMultiLanguage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.I18NTorF",
                c => new
                    {
                        I18NTorFId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NTorFId);
            
            AddColumn("dbo.TrueOrFalse", "Name", c => c.String(maxLength: 255));
            DropColumn("dbo.TrueOrFalse", "Question");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrueOrFalse", "Question", c => c.String());
            DropColumn("dbo.TrueOrFalse", "Name");
            DropTable("dbo.I18NTorF");
        }
    }
}
