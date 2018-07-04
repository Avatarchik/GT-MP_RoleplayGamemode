namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedItemInformations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemInformations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Name = c.String(unicode: false),
                        NamePlural = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        Weight = c.Int(nullable: false),
                        BuyPrice = c.Double(nullable: false),
                        SellPrice = c.Double(nullable: false),
                        UsageTrigger = c.String(unicode: false),
                        DisposeTrigger = c.String(unicode: false),
                        Usable = c.Boolean(nullable: false),
                        Disposable = c.Boolean(nullable: false),
                        Buyable = c.Boolean(nullable: false),
                        Sellable = c.Boolean(nullable: false),
                        Giftable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ItemInformations");
        }
    }
}
