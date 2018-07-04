namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOwnedVehicles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OwnedVehicles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OwnerUser = c.Int(nullable: false),
                        OwnerGroup = c.Int(nullable: false),
                        ModelName = c.String(unicode: false),
                        ModelHash = c.Int(nullable: false),
                        ColorPrimary = c.Int(nullable: false),
                        ColorSecondary = c.Int(nullable: false),
                        NumberPlate = c.String(unicode: false),
                        Health = c.Int(nullable: false),
                        Fuel = c.Double(nullable: false),
                        IsDeath = c.Boolean(nullable: false),
                        DirtLevel = c.Single(nullable: false),
                        DamageString = c.String(unicode: false),
                        TuningString = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OwnedVehicles");
        }
    }
}
