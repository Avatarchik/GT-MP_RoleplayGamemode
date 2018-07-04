namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GarageEnableDisableFnc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Garages", "Enabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Garages", "Enabled");
        }
    }
}
