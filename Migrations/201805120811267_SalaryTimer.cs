namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalaryTimer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "SalaryTime", c => c.Int(nullable: false));
            AddColumn("dbo.Characters", "TotalPlayTime", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "TotalPlayTime");
            DropColumn("dbo.Characters", "SalaryTime");
        }
    }
}
