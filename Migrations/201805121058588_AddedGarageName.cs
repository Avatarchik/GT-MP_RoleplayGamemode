namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGarageName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Garages", "Name", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Garages", "Name");
        }
    }
}
