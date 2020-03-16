namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TorfQuestions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrueOrFalse",
                c => new
                    {
                        TrueOrFalseId = c.Short(nullable: false, identity: true),
                        Question = c.String(),
                        Answer = c.Boolean(nullable: false),
                        QuestionType = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.TrueOrFalseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TrueOrFalse");
        }
    }
}
