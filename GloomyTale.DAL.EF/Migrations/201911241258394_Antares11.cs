namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Antares11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.I18NShopName",
                c => new
                    {
                        I18NShopNameId = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        RegionType = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.I18NShopNameId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.I18NShopName");
        }
    }
}
