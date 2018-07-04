namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOwnerFieldsToBankAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccounts", "OwnerUser", c => c.Int(nullable: false));
            AddColumn("dbo.BankAccounts", "OwnerGroup", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccounts", "OwnerGroup");
            DropColumn("dbo.BankAccounts", "OwnerUser");
        }
    }
}
