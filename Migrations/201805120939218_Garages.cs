namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Garages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Garages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.String(unicode: false),
                        OwnerUser = c.Int(nullable: false),
                        OwnerGroup = c.Int(nullable: false),
                        OwnerProperty = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        NeedKey = c.Boolean(nullable: false),
                        MaxTotalVehicle = c.Int(nullable: false),
                        MaxBigVehicle = c.Int(nullable: false),
                        MaxMediumVehicle = c.Int(nullable: false),
                        MaxSmallVehicle = c.Int(nullable: false),
                        ParkOutSpotsString = c.String(unicode: false),
                        ParkInSpotsString = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Garages");
        }
    }
}
