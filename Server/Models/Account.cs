using GrandTheftMultiplayer.Server.Elements;
using Roleplay.Server.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Server.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string SocialClubName { get; set; }
        public string Password { get; set; }
        public string Ip { get; set; }
        public string HardwareID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsLocked { get; set; }
        public int MaxCharacters { get; set; }
        public AdminLevel AdminLevel { get; set; }
        public string Comment { get; set; }

        public bool IsLoggedIn { get; set; }
        [NotMapped]
        public bool IsSpawned { get; set; }

        [NotMapped]
        public Character CurrentCharacter { get; set; }
        [NotMapped]
        public Client CurrentClient { get; set; }
        [NotMapped]
        public bool IsInEditor { get; set; }

        public Account()
        {
            LastActivity = DateTime.Now;
            CreatedAt = DateTime.Now;
            IsLocked = false;
            MaxCharacters = 1;
            AdminLevel = AdminLevel.User;
            Comment = "";
            IsInEditor = false;
            IsSpawned = false;
        }
    }
}