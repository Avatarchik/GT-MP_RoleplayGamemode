using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Roleplay.Server.Controller;
using Roleplay.Server.Enums;
using Roleplay.Server.Models;
using System;
using System.Linq;

namespace Roleplay.Server.Extensions
{
    public static class ClientExtensions
    {
        #region Data Helpers

        public static T GetData<T>(this Client player, string dataName, T defaultValue = default)
        {
            if (player == null || !player.exists)
                return defaultValue;
            if (!player.hasData(dataName))
                return defaultValue;
            var tmp = player.getData(dataName);
            if (tmp == null)
                return defaultValue;
            if (typeof(T).HasInterface(typeof(IConvertible)))
                return (T)Convert.ChangeType(tmp, typeof(T));
            else
                return (T)tmp;
        }

        public static T GetSyncedData<T>(this Client player, string dataName, T defaultValue = default)
        {
            if (player == null || !player.exists)
                return defaultValue;
            if (!player.hasSyncedData(dataName))
                return defaultValue;
            var tmp = player.getSyncedData(dataName);
            if (tmp == null)
                return defaultValue;
            if (typeof(T).HasInterface(typeof(IConvertible)))
                return (T)Convert.ChangeType(tmp, typeof(T));
            else
                return (T)tmp;
        }
        #endregion Data Helpers

        public static bool IsAdmin(this Client player)
        {
            return player.GetData("IS_ADMIN", false);
        }

        public static bool IsReady(this Client player)
        {
            return player.GetData("IS_READY", false);
        }

        public static void IsReady(this Client player, bool ready)
        {
            player.setData("IS_READY", ready);
        }

        public static bool IsDead(this Client player)
        {
            return player.GetData("IS_DEAD", false) || player.dead;
        }

        public static int GetCharacterId(this Client player)
        {
            return player.GetData("PLAYER_ID", player.handle.Value);
        }

        public static string GetTeamspeakID(this Client player)
        {
            return player.GetData("PLAYER_TEAMSPEAK_IDENT", "NothingAtAll");
        }

        public static ushort GetTeamspeakClientID(this Client player)
        {
            return player.GetData("VOICE_TS_ID", (ushort)0);
        }

        public static long GetVoiceConnectionID(this Client player)
        {
            return player.GetData("VOICE_ID", (long)0);
        }

        public static string GetName(this Client player)
        {
            return player.GetData("PLAYER_TEAMSPEAK_NAME", "");
        }

        public static string GetCharacterName(this Client player)
        {
            return player.GetData("PLAYER_CHARACTER_NAME", player.name);
        }

        public static void UpdateHUD(this Client player)
        {
            //TODO: Do whatever is needed to updte player HUD
            player.triggerEvent("UPDATE_HUD");
        }

        public static void PlaySound(this Client player, string soundName, float distanceToHear = 0, bool loop = false, bool needStopEvent = false)
        {
            // TODO: Add whatever is necessary to play a sound at the player
        }

        public static void Message(this Client player, string msg)
        {
            player.sendNotification("System", msg, false);
        }

        /// <summary>
        /// Set/Reset virtual voice-position of player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="virtualPosition">null = reset</param>
        public static void SetVoicePosition(this Client player, Vector3 virtualPosition = null)
        {
            if (virtualPosition == null)
                player.resetData("VOICE_POSITION");
            else
                player.setData("VOICE_POSITION", virtualPosition);
        }

        /// <summary>
        /// Get the Voice Position of this player. Takes into account if he is in an interior that is physically somewhere else.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Vector3 GetVoicePosition(this Client player)
        {
            if (player.hasData("VOICE_POSITION"))
                return player.GetData("VOICE_POSITION", new Vector3());
            return player.position;
        }

        #region Radio stuff

        /// <summary>
        /// Set/Reset virtual radio voice-position of player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="virtualPosition">null = reset</param>
        public static void SetRadioVoicePosition(this Client player, Vector3 virtualPosition = null)
        {
            if (virtualPosition == null)
                player.resetData("RADIO_VOICE_POSITION");
            else
                player.setData("RADIO_VOICE_POSITION", virtualPosition);
        }

        /// <summary>
        /// Get the radio Voice Position of this player. Takes into account if he is in an interior that is physically somewhere else.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Vector3 GetRadioVoicePosition(this Client player)
        {
            if (player.hasData("RADIO_VOICE_POSITION"))
                return player.GetData("RADIO_VOICE_POSITION", new Vector3());
            return player.position;
        }

        public static RadioModes GetRadioMode(this Client player)
        {
            switch (player.GetData("RADIO_MODE", "off"))
            {
                case "off":
                    return RadioModes.OFF;
                case "on":
                    return RadioModes.LISTENING;
                case "send":
                    return RadioModes.SPEAKING;
            }
            return RadioModes.OFF;
        }

        public static void SetRadioMode(this Client player, RadioModes newMode)
        {
            switch (newMode)
            {
                case RadioModes.OFF:
                    player.setData("RADIO_MODE", "off");
                    break;
                case RadioModes.LISTENING:
                    player.setData("RADIO_MODE", "on");
                    break;
                case RadioModes.SPEAKING:
                    player.setData("RADIO_MODE", "send");
                    break;
                default:
                    break;
            }
        }

        public static int GetRadioChannel(this Client player)
        {
            return player.GetData("RADIO_CHANNEL", 0);
        }
        public static void SetRadioChannel(this Client player, int channel)
        {
            player.setData("RADIO_CHANNEL", channel);
        }

        public static bool CanUseRadio(this Client player)
        {
            return true;
        }
        #endregion

        public static bool IsLoggedIn(this Client player)
        {
            return player.GetData("LOGGED_IN", false);
        }

        public static void IsLoggedIn(this Client player, bool isLoggedIn)
        {
            player.setData("LOGGED_IN", isLoggedIn);
        }

        public static Account Account(this Client player)
        {
            return player.GetData<Account>("PLAYER_ACCOUNT", null);
        }

        public static void Account(this Client player, Account account)
        {
            player.setData("PLAYER_ACCOUNT", account);
        }

        public static Character Character(this Client player)
        {
            return player.Account().CurrentCharacter;
        }

        public static void DisplayRadar(this Client player, bool visible)
        {
            player.sendNative(Hash.DISPLAY_RADAR, visible);
        }

        public static void AdminMode(this Client player, bool enabled)
        {
            player.triggerEvent("ADMIN_MODE", enabled);
        }

        public static void BlockInteractionKeys(this Client player, bool block)
        {
            player.triggerEvent("KEYBLOCK", block);
        }

        public static bool HasPlayerVehicleKey(this Client client, NetHandle vehicle)
        {
            var veh = OwnedVehicleController.ExistingVehicles.FirstOrDefault(x => x.Handle.handle.Value == vehicle.Value);
            if (veh == null)
                return false;
            return client.HasPlayerVehicleKey(veh.Id);
        }

        public static bool HasPlayerVehicleKey(this Client client, OwnedVehicle vehicle)
        {
            return client.HasPlayerVehicleKey(vehicle.Id);
        }

        public static bool HasPlayerVehicleKey(this Client client, int vehicleId)
        {
            if (!client.IsLoggedIn())
                return false;
            return client.Account().CurrentCharacter.KeyRing.VehicleKeys.ContainsKey(vehicleId);
        }

        #region Wrapper
        public static void ShowLoadingPrompt(this Client player, string loadingText, BusySpinnerType busySpinnerType)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowLoadingPrompt", loadingText, (int)busySpinnerType);
        }

        public static void HideLoadingPrompt(this Client player)
        {
            player.triggerEvent("CLIENT_WRAPPER", "HideLoadingPrompt");
        }

        public static void FadeScreenOut(this Client player, int time = 1000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "FadeScreenOut", time);
        }

        public static void FadeScreenIn(this Client player, int time = 1000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "FadeScreenIn", time);
        }

        public static void ShowMissionPassedMessage(this Client player, string message, int time = 3000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowMissionPassedMessage", message, time);
        }

        public static void ShowMpMessageLarge(this Client player, string message, string subtitle, int time = 3000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowMpMessageLarge", message, subtitle, time);
        }

        public static void ShowWeaponPurchasedMessage(this Client player, string bigMessage, string weaponName, WeaponHash weapon, int time = 3000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowWeaponPurchasedMessage", bigMessage, weaponName, (int)weapon, time);
        }

        public static void ShowRankupMessage(this Client player, string message, string subtitle, int rank, int time = 3000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowRankupMessage", message, subtitle, rank, time);
        }

        public static void ShowOldMessage(this Client player, string message, int time = 3000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowOldMessage", message, time);
        }

        public static void ShowColoredShard(this Client player, string message, string description, HudColor textColor, HudColor bgColor, int time = 3000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowColoredShard", message, description, (int)textColor, (int)bgColor, time);
        }

        public static void DisplaySubtitle(this Client player, string message, int time = 3000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "DisplaySubtitle", message, time);
        }

        public static void PlayScreenEffect(this Client player, ScreenEffect effectName, int duration = 5000, bool looped = false)
        {
            player.triggerEvent("CLIENT_WRAPPER", "PlayScreenEffect", effectName.ToString(), duration, looped);
        }

        public static void StopScreenEffect(this Client player, ScreenEffect effectName)
        {
            player.triggerEvent("CLIENT_WRAPPER", "StopScreenEffect", effectName.ToString());
        }

        public static void StopAllScreenEffects(this Client player)
        {
            player.triggerEvent("CLIENT_WRAPPER", "StopAllScreenEffects");
        }

        public static void ShowSimpleMessage(this Client player, string title, string message, int time = 5000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowSimpleMessage", title, message, time);
        }

        public static void ShowMidsizedMessage(this Client player, string title, string message, HudColor bgColor, bool useDarkerShard, bool condensed, int time = 3000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowMidsizedMessage", title, message, (int)bgColor, useDarkerShard, condensed, time);
        }

        public static void SetThermalVisionEnabled(this Client player, bool enabled)
        {
            player.triggerEvent("CLIENT_WRAPPER", "SetThermalVisionEnabled", enabled);
        }

        public static void SetNightVisionEnabled(this Client player, bool enabled)
        {
            player.triggerEvent("CLIENT_WRAPPER", "SetNightVisionEnabled", enabled);
        }

        public static void DisplayHelpText(this Client player, string text, HudColor color, int time = 5000, int alpha = 255)
        {
            player.triggerEvent("CLIENT_WRAPPER", "DisplayHelpText", text, time, (int)color, alpha);
        }

        public static void ShowPlaneMessage(this Client player, string title, string planeName, int planeHash, int time = 4000)
        {
            player.triggerEvent("CLIENT_WRAPPER", "ShowPlaneMessage", title, planeName, planeHash, time);
        }
        #endregion Wrapper
    }
}