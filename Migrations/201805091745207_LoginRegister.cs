namespace GTMP_Database_GameMode.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoginRegister : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Characters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SocialClubName = c.String(unicode: false),
                        CreatedAt = c.DateTime(nullable: false, precision: 0),
                        LastActivity = c.DateTime(nullable: false, precision: 0),
                        Locked = c.Boolean(nullable: false),
                        Hunger = c.Int(nullable: false),
                        Thirst = c.Int(nullable: false),
                        Health = c.Int(nullable: false),
                        Armor = c.Int(nullable: false),
                        FirstName = c.String(unicode: false),
                        LastName = c.String(unicode: false),
                        Cash = c.Double(nullable: false),
                        CharacterStyleString = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Accounts", "Ip", c => c.String(unicode: false));
            AddColumn("dbo.Accounts", "HardwareID", c => c.String(unicode: false));
            AddColumn("dbo.Accounts", "MaxCharacters", c => c.Int(nullable: false));
            AddColumn("dbo.Accounts", "IsLoggedIn", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "IsLoggedIn");
            DropColumn("dbo.Accounts", "MaxCharacters");
            DropColumn("dbo.Accounts", "HardwareID");
            DropColumn("dbo.Accounts", "Ip");
            DropTable("dbo.Characters");
        }
    }
}
