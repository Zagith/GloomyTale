namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultiAccountCheck : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MultiAccountException",
                c => new
                    {
                        ExceptionId = c.Long(nullable: false, identity: true),
                        AccountId = c.Long(nullable: false),
                        ExceptionLimit = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.ExceptionId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MultiAccountException", "AccountId", "dbo.Account");
            DropIndex("dbo.MultiAccountException", new[] { "AccountId" });
            DropTable("dbo.MultiAccountException");
        }
    }
}
