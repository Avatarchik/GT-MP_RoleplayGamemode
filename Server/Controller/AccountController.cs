using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.API;
using Roleplay.Base;
using Roleplay.Server.Enums;
using Roleplay.Server.Extensions;
using Roleplay.Server.Handler;
using Roleplay.Server.Managers;
using Roleplay.Server.Models;
using System;
using System.Linq;
using Roleplay.Server.Base;

namespace Roleplay.Server.Controller
{
    public class AccountController : RoleplayScript
    {
        public static event EventHandler<Client, Account> OnPlayerSuccessfulLoggedIn;

        public AccountController()
        {
            ClientEventManager.RegisterClientEvent("LOGIN_ACCOUNT", ClientTriggeredLogin); // [Args] 0: <string> Password
            ClientEventManager.RegisterClientEvent("REGISTER_ACCOUNT", ClientTriggeredRegister); // [Args] 0: <string> Password; 1: <string> rePassword
        }

        private void ClientTriggeredRegister(Client client, string eventName, object[] arguments)
        {
            string password = GetArg<string>(arguments, 0, "");
            string rePassword = GetArg<string>(arguments, 1, "");
            if (password == "" || rePassword == "")
            {
                client.sendColoredNotification("Die eingegebenen Passwörter müssen übereinstimmen!", (int)HudColor.HUD_COLOUR_WHITE, (int)HudColor.HUD_COLOUR_RED, true);
                return;
            }
            if (password != rePassword)
            {
                client.sendColoredNotification("Die eingegebenen Passwörter stimmen nicht überein..", (int)HudColor.HUD_COLOUR_WHITE, (int)HudColor.HUD_COLOUR_RED, true);
                return;
            }
            LoginPlayer(client, CreateAccount(client, password));
        }

        private void ClientTriggeredLogin(Client client, string eventName, object[] arguments)
        {
            string password = GetArg<string>(arguments, 0, "");
            if (password == "")
                return;

            Account account = LoadAccount(client);
            if (account == null)
                return;

            if (CheckPassword(password, account))
            {
                // Password correct
                LoginPlayer(client, account);
                client.sendColoredNotification("Login erfolgreich!", (int)HudColor.HUD_COLOUR_WHITE, (int)HudColor.HUD_COLOUR_GREEN, true);
                CloseLoginRegisterDialog(client);
                return;
            }
            else
            {
                // Password wrong
                int loginTry = client.GetData<int>("LOGIN_PASSWORD_TRY", 0);
                loginTry++;
                client.setData("LOGIN_PASSWORD_TRY", loginTry);
                logger.Warn($"Login Passwort Falsch! ({loginTry}. Versuch) [{client.socialClubName}] [{client.address}]");
                client.sendColoredNotification("Das Passwort war falsch..", (int)HudColor.HUD_COLOUR_WHITE, (int)HudColor.HUD_COLOUR_RED, true, 255, 0, 0, 220);
                if(loginTry >= 3)
                {
                    ConnectionHandler.TemporaryBlockUser(client, 60, "Du hast zu oft das falsche Passwort eingegeben..");
                    return;
                }
            }
        }

        #region Static Functions
        public static bool CheckPassword(string plainPassword, Account account)
        {
            return GameMode.sharedAPI.verifyPasswordHashBCrypt(plainPassword, account.Password);
        }

        public static Account LoadAccount(Client client)
        {
            using (var db = new Database())
            {
                return db.Accounts.FirstOrDefault(acc => acc.SocialClubName == client.socialClubName);
            }
        }

        public static Account CreateAccount(Client client, string plainPassword)
        {
            var account = new Account
            {
                SocialClubName = client.socialClubName,
                Password = GameMode.sharedAPI.getPasswordHashBCrypt(plainPassword),
                Ip = client.address,
                HardwareID = client.uniqueHardwareId,
                IsLocked = false,
                AdminLevel = AdminLevel.User,
                MaxCharacters = 1,
                CreatedAt = DateTime.Now,
                LastActivity = DateTime.Now,
                Comment = "",
                IsLoggedIn = false
            };
            using (var db = new Database())
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                client.sendColoredNotification("Account wurde erfolgreich erstellt!", (int)HudColor.HUD_COLOUR_WHITE, (int)HudColor.HUD_COLOUR_GREEN, true);
                CloseLoginRegisterDialog(client);
                return db.Accounts.FirstOrDefault(acc => acc.SocialClubName == client.socialClubName);
            }
        }

        public static void LoginPlayer(Client client, Account account)
        {
            account.CurrentClient = client;
            account.LastActivity = DateTime.Now;
            account.IsLoggedIn = true;
            client.Account(account);
            client.IsLoggedIn(true);
            client.FadeScreenOut(200);
            using(var db = new Database())
            {
                var acc = db.Accounts.First(x => x.Id == account.Id);
                acc.IsLoggedIn = true;
                acc.LastActivity = DateTime.Now;
                acc.Ip = client.address;
                acc.HardwareID = client.uniqueHardwareId;
                db.SaveChangesAsync();
            }
            GameMode.sharedAPI.delay(220, true, () =>
            {
                OnPlayerSuccessfulLoggedIn?.Invoke(client, account);
            });
        }

        public static void OpenRegisterDialogForPlayer(Client client)
        {
            client.triggerEvent("showAccountModal", Constants.AccountRegisterUrl, Constants.LoginRegisterCamera, Constants.LoginRegisterCameraLookAt);
            client.DisplayRadar(false);
        }

        public static void OpenLoginDialogForPlayer(Client client)
        {
            client.triggerEvent("showAccountModal", Constants.AccountLoginUrl, Constants.LoginRegisterCamera, Constants.LoginRegisterCameraLookAt);
            client.DisplayRadar(false);
        }

        public static void CloseLoginRegisterDialog(Client client)
        {
            client.triggerEvent("hideAccountModal");
            client.freeze(false);
            client.DisplayRadar(true);
        }

        public static void SaveAccount(Client client)
        {
            if (!client.IsLoggedIn())
                return;
            var account = client.Account();
            using (var db = new Database())
            {
                var acc = db.Accounts.FirstOrDefault(x => x.Id == account.Id);
                acc.LastActivity = DateTime.Now;
                acc.HardwareID = client.uniqueHardwareId;
                acc.Ip = client.address;
                acc.IsLoggedIn = false;
                db.SaveChanges();
            }
        }

        public static void LogoutPlayer(Client client)
        {
            var account = client.Account();
            SaveAccount(client);
            account.CurrentCharacter = null;
            account.CurrentClient = null;
        }
        #endregion Static Functions
    }
}