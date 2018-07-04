using GrandTheftMultiplayer.Server.Elements;
using Newtonsoft.Json;
using Roleplay.Server.Base;
using Roleplay.Server.Controller;
using Roleplay.Server.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Server.Models
{
    [Table("Characters")]
    public class Character
    {
        [Key]
        public int Id { get; set; }

        public string SocialClubName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivity { get; set; }
        public bool Locked { get; set; }

        public string Position { get; set; }
        public double Rotation { get; set; }

        public int Hunger { get; set; }
        public int Thirst { get; set; }

        public int Health { get; set; }
        public int Armor { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public double Cash { get; set; }

        public int  SalaryTime { get; set; }
        public int TotalPlayTime { get; set; }

        [NotMapped]
        public List<int> BankAccountAccess { get; set; }
        public string BankAccountAccessString { get; set; }

        [NotMapped]
        public CharacterStyle CharacterStyle { get; set; }
        public string CharacterStyleString { get; set; }

        [NotMapped]
        public List<InventoryItem> Inventory { get; set; }
        public string InventoryString { get; set; }

        [NotMapped]
        public CharacterClothing Clothing { get; set; }
        public string ClothingString { get; set; }

        [NotMapped]
        public KeyRing KeyRing { get; set; }
        public string KeyRingString { get; set; }

        [NotMapped]
        public Account Account { get; set; }

        public Character()
        {
            CreatedAt = DateTime.Now;
            LastActivity = DateTime.Now;
            Locked = false;
            Hunger = 100;
            Thirst = 100;
            Health = 100;
            Cash = Constants.StartCash;
            Position = Constants.DefaultSpawnPosition.ToJson();
            Rotation = Constants.DefaultSpawnRotation;
            BankAccountAccess = new List<int>();
            Armor = 0;
            SalaryTime = 0;
            TotalPlayTime = 0;
            KeyRing = new KeyRing();
        }

        public void StyleToStyleString()
        {
            CharacterStyleString = JsonConvert.SerializeObject(CharacterStyle);
        }

        public void StyleStringToStyle()
        {
            if(CharacterStyleString != "" && CharacterStyleString != null)
            {
                CharacterStyle = JsonConvert.DeserializeObject<CharacterStyle>(CharacterStyleString);
            }
            else
            {
                CharacterStyle = new CharacterStyle();
            }
        }

        public void DataToString()
        {
            KeyRingString = JsonConvert.SerializeObject(KeyRing);
            InventoryString = JsonConvert.SerializeObject(Inventory);
        }

        public void StringToData()
        {
            if (KeyRingString != "" && KeyRingString != null)
            {
                KeyRing = JsonConvert.DeserializeObject<KeyRing>(KeyRingString);
            }
            else
            {
                KeyRing = new KeyRing();
            }

            if (InventoryString != "" && InventoryString != null)
            {
                Inventory = JsonConvert.DeserializeObject<List<InventoryItem>>(InventoryString);
            }
            else
            {
                Inventory = new List<InventoryItem>();
            }
        }

        public void BankAccountAccessToString()
        {
            BankAccountAccessString = JsonConvert.SerializeObject(BankAccountAccess);
        }

        public void StringToBankAccountAccess()
        {
            if (BankAccountAccessString != "" && BankAccountAccessString != null)
            {
                BankAccountAccess = JsonConvert.DeserializeObject<List<int>>(BankAccountAccessString);
            }
            else
            {
                BankAccountAccess = new List<int>();
            }
        }

        public void ClothingToClothingString()
        {
            ClothingString = JsonConvert.SerializeObject(Clothing);
        }

        public void ClothingStringToClothing()
        {
            if (ClothingString != "" && ClothingString != null)
            {
                Clothing = JsonConvert.DeserializeObject<CharacterClothing>(ClothingString);
            }
            else
            {
                if(CharacterStyle.Gender == 0) // Mann
                {
                    Clothing = new CharacterClothing {
                        Mask = new CharacterClothingComponent(0, 0),
                        Top = new CharacterClothingComponentTorso(new CharacterClothingComponent(0, 0), new CharacterClothingComponent(57, 0), new CharacterClothingComponent(39, GameMode.random.Next(0, 1))),
                        Leg = new CharacterClothingComponent(0, 0),
                        Feet = new CharacterClothingComponent(1, 0),
                        Vest = new CharacterClothingComponent(0, 0),
                        Bag = new CharacterClothingComponent(0, 0),
                        Decal = new CharacterClothingComponent(0, 0),
                        Accessories = new CharacterClothingComponent(0, 0),
                        Hat = new CharacterClothingComponent(255, 0),
                        Glasses = new CharacterClothingComponent(255, 0),
                        Ears = new CharacterClothingComponent(255, 0),
                        Watches = new CharacterClothingComponent(255, 0),
                        Bracelets = new CharacterClothingComponent(255, 0)
                    };
                }
                else
                {
                    Clothing = new CharacterClothing
                    {
                        Mask = new CharacterClothingComponent(0, 0),
                        Top = new CharacterClothingComponentTorso(new CharacterClothingComponent(2, 0), new CharacterClothingComponent(34, 0), new CharacterClothingComponent(2, GameMode.random.Next(0, 15))),
                        Leg = new CharacterClothingComponent(0, 0),
                        Feet = new CharacterClothingComponent(1, 0),
                        Vest = new CharacterClothingComponent(0, 0),
                        Bag = new CharacterClothingComponent(0, 0),
                        Decal = new CharacterClothingComponent(0, 0),
                        Accessories = new CharacterClothingComponent(0, 0),
                        Hat = new CharacterClothingComponent(255, 0),
                        Glasses = new CharacterClothingComponent(255, 0),
                        Ears = new CharacterClothingComponent(255, 0),
                        Watches = new CharacterClothingComponent(255, 0),
                        Bracelets = new CharacterClothingComponent(255, 0)
                    };
                }
            }
        }

        public void Save(Client client)
        {
            CharacterController.SaveCharacter(client, this);
        }
    }
}