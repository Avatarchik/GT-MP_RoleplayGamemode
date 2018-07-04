namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedCharSpawnRotationToDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Characters", "Rotation", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Characters", "Rotation", c => c.Single(nullable: false));
        }
    }
}
