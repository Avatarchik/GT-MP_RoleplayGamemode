using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using Roleplay.Base;
using Roleplay.Server.Base;
using Roleplay.Server.Extensions;
using Roleplay.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Controller
{
    public class CharacterController : RoleplayScript
    {
        public static event EventHandler<Client, Account> OnPlayerSelectCharacter;

        public CharacterController()
        {
            AccountController.OnPlayerSuccessfulLoggedIn += AccountController_OnPlayerSuccessfulLoggedIn;
            WorldEnvironmentController.OnUpdateWorldTime += WorldEnvironmentController_OnUpdateWorldTime;
        }

        private void WorldEnvironmentController_OnUpdateWorldTime()
        {
            API.getAllPlayers().ToList().Where(plr => plr.IsLoggedIn() && plr.Account().CurrentCharacter != null && !plr.Account().IsInEditor).ForEach(plr => {
                var ch = plr.Account().CurrentCharacter;
                ch.TotalPlayTime++;
                ch.SalaryTime++;
                if(ch.SalaryTime >= Constants.SalaryTimer)
                {
                    BankController.GiveUnemploymentBenefits(plr);
                    ch.SalaryTime = 0;
                }
                CharacterController.SaveCharacter(plr, plr.Account().CurrentCharacter);
            });
        }

        private void AccountController_OnPlayerSuccessfulLoggedIn(Client client, Account e)
        {
            var characters = GetAllCharactersOfAccount(e.SocialClubName);
            if(characters.Count <= 0)
            {
                var character = new Character
                {
                    SocialClubName = e.SocialClubName
                };
                var ch = CreateCharacter(character);
                ApplyCharacterToSession(client, ch);
                logger.Debug($"Apply New Created Char | ID: {ch.Id} to Player {client.socialClubName}");
                return;
            }
            else
            {
                var ch = characters.First();
                ApplyCharacterToSession(client, ch);
                logger.Debug($"Apply Existing Char | ID: {ch.Id} to Player {client.socialClubName}");
                return;
            }
        }

        public static Character CreateCharacter(Character character)
        {
            using (var db = new Database())
            {
                db.Characters.Add(character);
                db.SaveChanges();
                sharedLogger.Debug($"Create Character (FirstName: {character.FirstName} | LastName: {character.LastName}");
                return db.Characters.FirstOrDefault(ch => ch.FirstName == character.FirstName
                && ch.LastName == character.LastName && ch.SocialClubName == character.SocialClubName);
            }
        }

        public static List<Character> GetAllCharactersOfAccount(string socialClubName)
        {
            using (var db = new Database())
            {
                return db.Characters.AsNoTracking().Where(ch => ch.SocialClubName == socialClubName).ToList();
            }
        }

        public static void ApplyCharacterToSession(Client player, int characterId)
        {
            if (player.Account() == null)
                return;
            using (var db = new Database())
            {
                var chr = db.Characters.FirstOrDefault(ch => ch.Id == characterId);
                player.Account().CurrentCharacter = chr;
                player.health = chr.Health;
                player.armor = chr.Armor;
                chr.StyleStringToStyle();
                chr.ClothingStringToClothing();
                chr.StringToBankAccountAccess();
                chr.StringToData();
                OnPlayerSelectCharacter?.Invoke(player, player.Account());
            }
        }

        public static void ApplyCharacterToSession(Client client, Character character)
        {
            ApplyCharacterToSession(client, character.Id);
        }

        public static void ApplyCharacterClothing(Client client)
        {
            if (!client.IsLoggedIn())
                return;
            if (client.Account().CurrentCharacter == null)
            {
                sharedLogger.Debug($"Applied Clothing to {client.socialClubName} failed character is Null");
                return;
            }
            var style = client.Account().CurrentCharacter.Clothing;
            if (style == null)
            {
                sharedLogger.Debug($"Applied Clothing to {client.socialClubName} failed style is Null");
                return;
            }
            client.setClothes(0, style.Mask.Drawable, style.Mask.Texture);
            client.setClothes(3, style.Top.Torso.Drawable, style.Top.Torso.Texture);
            client.setClothes(8, style.Top.Undershirt.Drawable, style.Top.Undershirt.Texture);
            client.setClothes(11, style.Top.Top.Drawable, style.Top.Top.Texture);
            client.setClothes(4, style.Leg.Drawable, style.Leg.Texture);
            client.setClothes(6, style.Feet.Drawable, style.Feet.Texture);
            client.setClothes(9, style.Vest.Drawable, style.Vest.Texture);
            client.setClothes(5, style.Bag.Drawable, style.Bag.Texture);
            client.setClothes(10, style.Decal.Drawable, style.Decal.Texture);
            client.setClothes(7, style.Accessories.Drawable, style.Accessories.Texture);

            client.setAccessories(0, style.Hat.Drawable, style.Hat.Texture);
            client.setAccessories(1, style.Glasses.Drawable, style.Glasses.Texture);
            client.setAccessories(2, style.Ears.Drawable, style.Ears.Texture);
            client.setAccessories(6, style.Watches.Drawable, style.Watches.Texture);
            client.setAccessories(7, style.Bracelets.Drawable, style.Bracelets.Texture);
            sharedLogger.Debug($"Applied Clothing to {client.socialClubName}");
        }

        public static void SaveCharacter(Client client, Character character, bool includeGeneralData = false)
        {
            
            character.LastActivity = DateTime.Now;
            if (!client.Account().IsInEditor && client.Account().IsSpawned)
            {
                character.Position = client.position.ToJson();
                character.Rotation = client.rotation.Z;
            }
            character.StyleToStyleString();
            character.ClothingToClothingString();
            character.BankAccountAccessToString();
            character.DataToString();
            using (var db = new Database())
            {

                var ch = db.Characters.FirstOrDefault(x => x.Id == character.Id);
                if (ch == null)
                {
                    return;
                }
                //db.Entry(ch).CurrentValues.SetValues(account.CurrentCharacter); Overwrite All Values
                ch.LastActivity = character.LastActivity;
                ch.Position = character.Position;
                ch.Rotation = character.Rotation;
                ch.Hunger = character.Hunger;
                ch.Thirst = character.Thirst;
                ch.Health = character.Health;
                ch.Armor = character.Armor;
                ch.Cash = character.Cash;
                ch.CharacterStyleString = character.CharacterStyleString;
                ch.ClothingString = character.ClothingString;
                ch.BankAccountAccessString = character.BankAccountAccessString;
                ch.SalaryTime = character.SalaryTime;
                ch.TotalPlayTime = character.TotalPlayTime;
                ch.KeyRingString = character.KeyRingString;
                if (includeGeneralData)
                {
                    ch.FirstName = character.FirstName;
                    ch.LastName = character.LastName;
                }
                db.SaveChanges();
            }
        }
    }
}
