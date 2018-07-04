namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCharacterPosition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "Position", c => c.String(unicode: false));
            AddColumn("dbo.Characters", "Rotation", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "Rotation");
            DropColumn("dbo.Characters", "Position");
        }
    }
}
