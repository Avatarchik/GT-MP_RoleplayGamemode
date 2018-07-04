namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInventoryToCharacter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "InventoryString", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "InventoryString");
        }
    }
}
