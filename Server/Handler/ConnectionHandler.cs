using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Roleplay.Base;
using Roleplay.Server.Base;
using Roleplay.Server.Controller;
using Roleplay.Server.Extensions;
using Roleplay.Server.Managers;
using Roleplay.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roleplay.Server.Handler
{
    public class ConnectionHandler : RoleplayScript
    {
        public ConnectionHandler()
        {
            API.onPlayerBeginConnect += API_onPlayerBeginConnect;
            API.onPlayerConnected += API_onPlayerConnected;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
        }

        private void API_onPlayerBeginConnect(Client player, CancelEventArgs cancelConnection)
        {
            if (!GameMode.IsWorldStarted)
            {
                cancelConnection.Cancel = true;
                cancelConnection.Reason = Messages.ServerIsStarting;
                return;
            }

            if (TemporaryBlockedUsers.ContainsKey(player.socialClubName))
            {
                if(DateTime.Now.Subtract(TemporaryBlockedUsers[player.socialClubName]).TotalSeconds < 0)
                {
                    cancelConnection.Reason = "Du wurdest Temporär gesperrt, versuche es in ein paar Minuten nocheinmal..";
                    cancelConnection.Cancel = true;
                    return;
                }
                else
                {
                    TemporaryBlockedUsers.Remove(player.socialClubName);
                }
            }

            using (var db = new Database())
            {
                var playerAccount = db.Accounts.FirstOrDefault(acc => acc.SocialClubName == player.socialClubName);
                if (playerAccount == null)
                {
                    logger.Warn($"Player {player.socialClubName} ({player.address}) [{player.name}] doesn't have an account..");
                    player.setData("action_after_download", "register");
                    return;
                }
                if (playerAccount.IsLocked)
                {
                    cancelConnection.Reason = "Dein Account ist gesperrt..";
                    cancelConnection.Cancel = true;
                    return;
                }
                logger.Warn($"Player {player.socialClubName} ({player.address}) [{player.name}] have an account(ID: {playerAccount.Id})..");
                player.setData("action_after_download", "login");
            }
        }

        private void API_onPlayerConnected(Client player)
        {
            player.position = Constants.ConnectPosition;
            player.freeze(true);
            player.IsLoggedIn(false);
        }

        private void API_onPlayerFinishedDownload(Client player)
        {

            player.IsReady(true);
            player.nametagVisible = false;
            player.armorbarVisible = false;
            player.healthbarVisible = false;
            DimensionManager.RequestPrivateDimension(player);
            if (player.hasData("action_after_download"))
            {
                if((string)player.getData("action_after_download") == "login")
                {
                    AccountController.OpenLoginDialogForPlayer(player);
                }
                else
                {
                    AccountController.OpenRegisterDialogForPlayer(player);
                }
            }
            else
            {
                player.kick("Please reconnect..");
            }
        }

        private void API_onPlayerDisconnected(Client player, string reason)
        {
            if (player.IsLoggedIn())
            {
                if(player.Account() == null)
                {
                    return;
                }
                if(player.Account().CurrentCharacter == null)
                {
                    return;
                }
                CharacterController.SaveCharacter(player, player.Account().CurrentCharacter);
                AccountController.LogoutPlayer(player);
            }
        }

        #region Static Functions
        public static readonly Dictionary<string, DateTime> TemporaryBlockedUsers = new Dictionary<string, DateTime>();
        public static void TemporaryBlockUser(Client client, int timeInSeconds, string reason)
        {
            if (!TemporaryBlockedUsers.ContainsKey(client.socialClubName))
            {
                TemporaryBlockedUsers.Add(client.socialClubName, DateTime.Now.AddSeconds(timeInSeconds));
                client.kick(reason);
            }
        }
        #endregion Static Functions
    }
}