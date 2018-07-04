namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPedOptionsToGargae : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Garages", "PedPosition", c => c.String(unicode: false));
            AddColumn("dbo.Garages", "PedRotation", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Garages", "PedRotation");
            DropColumn("dbo.Garages", "PedPosition");
        }
    }
}
