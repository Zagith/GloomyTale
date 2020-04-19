namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RuneEff : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RuneEffect",
                c => new
                    {
                        RuneEffectId = c.Long(nullable: false, identity: true),
                        EquipmentSerialId = c.Guid(nullable: false),
                        EffectType = c.Byte(nullable: false),
                        Effect = c.Byte(nullable: false),
                        Value = c.Short(nullable: false),
                        CardId = c.Short(nullable: false),
                        EffectUpgrade = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RuneEffectId);
            
            AddColumn("dbo.ItemInstance", "IsCarveRuneFixed", c => c.Boolean());
            AddColumn("dbo.ItemInstance", "CarveRuneUpgrade", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemInstance", "CarveRuneUpgrade");
            DropColumn("dbo.ItemInstance", "IsCarveRuneFixed");
            DropTable("dbo.RuneEffect");
        }
    }
}
