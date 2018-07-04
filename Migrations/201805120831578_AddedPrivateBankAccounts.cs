namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPrivateBankAccounts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccounts", "IsPrivate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccounts", "IsPrivate");
        }
    }
}
