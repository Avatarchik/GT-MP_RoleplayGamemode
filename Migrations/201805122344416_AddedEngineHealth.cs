namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEngineHealth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OwnedVehicles", "EngineHealth", c => c.Single(nullable: false));
            AlterColumn("dbo.OwnedVehicles", "Health", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OwnedVehicles", "Health", c => c.Int(nullable: false));
            DropColumn("dbo.OwnedVehicles", "EngineHealth");
        }
    }
}
