namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Antares12 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RecipeList", "RecipeId", "dbo.Recipe");
            AddColumn("dbo.Drop", "IsLevelPenalty", c => c.Boolean(nullable: false));
            AddColumn("dbo.Map", "Side", c => c.Int(nullable: false));
            AddColumn("dbo.Recipe", "MapNpcId", c => c.Int(nullable: false));
            AddColumn("dbo.Recipe", "ProduceItemVNum", c => c.Short(nullable: false));
            AddColumn("dbo.Recipe", "Rare", c => c.Short(nullable: false));
            AddColumn("dbo.Recipe", "Upgrade", c => c.Byte(nullable: false));
            AddColumn("dbo.Portal", "Side", c => c.Int(nullable: false));
            AddColumn("dbo.Portal", "RequiredItem", c => c.Short(nullable: false));
            AddColumn("dbo.Portal", "NomeOggetto", c => c.String());
            AddForeignKey("dbo.RecipeList", "RecipeId", "dbo.Recipe", "RecipeId", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.RecipeList", "RecipeId", "dbo.Recipe");
            DropColumn("dbo.Portal", "NomeOggetto");
            DropColumn("dbo.Portal", "RequiredItem");
            DropColumn("dbo.Portal", "Side");
            DropColumn("dbo.Recipe", "Upgrade");
            DropColumn("dbo.Recipe", "Rare");
            DropColumn("dbo.Recipe", "ProduceItemVNum");
            DropColumn("dbo.Recipe", "MapNpcId");
            DropColumn("dbo.Map", "Side");
            DropColumn("dbo.Drop", "IsLevelPenalty");
            AddForeignKey("dbo.RecipeList", "RecipeId", "dbo.Recipe", "RecipeId");
        }
    }
}
