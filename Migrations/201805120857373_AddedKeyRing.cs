namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedKeyRing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "KeyRingString", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "KeyRingString");
        }
    }
}
