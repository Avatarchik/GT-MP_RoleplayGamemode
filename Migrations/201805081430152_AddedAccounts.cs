namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAccounts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SocialClubName = c.String(unicode: false),
                        Password = c.String(unicode: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                        LastActivity = c.DateTime(nullable: false, precision: 0),
                        IsLocked = c.Boolean(nullable: false),
                        AdminLevel = c.Int(nullable: false),
                        Comment = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Accounts");
        }
    }
}
