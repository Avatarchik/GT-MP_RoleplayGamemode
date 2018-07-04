namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedExtraFieldsToOVeh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OwnedVehicles", "CreatedAt", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.OwnedVehicles", "LastUsage", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.OwnedVehicles", "InGarage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OwnedVehicles", "InGarage");
            DropColumn("dbo.OwnedVehicles", "LastUsage");
            DropColumn("dbo.OwnedVehicles", "CreatedAt");
        }
    }
}
