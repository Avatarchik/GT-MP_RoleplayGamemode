namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendOwnedVehicle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OwnedVehicles", "Position", c => c.String(unicode: false));
            AddColumn("dbo.OwnedVehicles", "Rotation", c => c.String(unicode: false));
            AddColumn("dbo.OwnedVehicles", "LastGarageId", c => c.Int(nullable: false));
            AddColumn("dbo.OwnedVehicles", "CanParkOutEverywhere", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OwnedVehicles", "CanParkOutEverywhere");
            DropColumn("dbo.OwnedVehicles", "LastGarageId");
            DropColumn("dbo.OwnedVehicles", "Rotation");
            DropColumn("dbo.OwnedVehicles", "Position");
        }
    }
}
