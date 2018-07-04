using GrandTheftMultiplayer.Server.Elements;
using Roleplay.Base;
using Roleplay.Server.Enums;
using Roleplay.Server.Extensions;
using Roleplay.Server.Models;
using Roleplay.Server.Models.MenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Controller
{
    public class CharacterInventoryController : RoleplayScript
    {
        public CharacterInventoryController()
        {
            InteractionController.OnPlayerOpenInventory += InteractionController_OnPlayerOpenInventory;
            MenuController.OnPlayerMenuSelectEvent += MenuController_OnPlayerMenuSelectEvent;
        }

        private void InteractionController_OnPlayerOpenInventory(object sender, Client client)
        {
            Menu menu = new Menu("Inventar", "Verfügbare Optionen:", "inventory");
            menu.Items.Add(new MenuItem("Gegenstände")); // WIP
            menu.Items.Add(new MenuItem("Schlüssel"){EventTrigger = "key_overview"});
            menu.Items.Add(new MenuItem("Scheine & Lizenzen")); // WIP
            menu.Show(client);
        }

        private void MenuController_OnPlayerMenuSelectEvent(Client client, MenuEventData data)
        {
            Character ch = client.Account().CurrentCharacter;
            if (data.MenuIdentifier != "inventory")
                return;
            switch (data.EventTrigger)
            {
                case "key_overview":
                    OpenKeyOverviewForPlayer(client);
                    break;
                case "show_vehicle_keys":
                    OpenVehicleKeyOverviewForPlayer(client);
                    break;
                case "show_vehicle_key_options":
                    OpenVehicleKeyOptions(client, data);
                    break;
                case "throw_away_vehicle_key":
                    {
                        int keyCount = Convert.ToInt32(data.UserInput);
                        if (!ch.KeyRing.VehicleKeys.ContainsKey(data.EventInt))
                        {
                            client.sendColoredNotification("Der Schlüssel konnte nicht gefunden werden..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_ORANGE);
                            return;
                        }
                        if (keyCount <= 0)
                        {
                            client.sendColoredNotification("Es wurde nichts weggeworfen..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_ORANGE);
                            OpenVehicleKeyOverviewForPlayer(client);
                            return;
                        }
                        if (keyCount >= ch.KeyRing.VehicleKeys[data.EventInt].Count)
                        {
                            client.Account().CurrentCharacter.KeyRing.VehicleKeys.Remove(data.EventInt);
                            client.sendColoredNotification($"Der Schlüssel ({data.EventString}) wurde weggeworfen..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                            OpenVehicleKeyOverviewForPlayer(client);
                            return;
                        }
                        ch.KeyRing.VehicleKeys[data.EventInt].Count -= keyCount;
                        client.sendColoredNotification($"Der Schlüssel ({data.EventString}) wurde {keyCount}x weggeworfen..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                        OpenVehicleKeyOverviewForPlayer(client);
                    }
                    break;
                case "give_away_vehicle_key":
                    {
                        int keyCount = Convert.ToInt32(data.UserInput);
                        OpenVehicleKeyOverviewForPlayer(client);
                        if (!ch.KeyRing.VehicleKeys.ContainsKey(data.EventInt))
                        {
                            client.sendColoredNotification("Der Schlüssel konnte nicht gefunden werden..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_ORANGE);
                            return;
                        }
                        var plrs = API.getAllPlayers().ToList().Where(x => x.IsLoggedIn() && client.position.DistanceTo(x.position) <= 1f && x != client).ToList();
                        if(plrs.Count > 1)
                        {
                            client.sendColoredNotification("Es befinden sich zu viele Personen um dich herum..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_ORANGE);
                            return;
                        }
                        if(plrs.Count <= 0)
                        {
                            client.sendColoredNotification("Ich hoffe dir ist bewusst das sich niemand um dich herum befindet..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_ORANGE);
                            return;
                        }
                        if(keyCount > ch.KeyRing.VehicleKeys[data.EventInt].Count)
                        {
                            client.sendColoredNotification("Du besitzt nicht so viele Schlüssel..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_ORANGE);
                            return;
                        }
                        if(keyCount <= 0)
                        {
                            client.sendColoredNotification("Du kannst nicht nichts weggeben..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_ORANGE);
                            return;
                        }
                        if (keyCount <= 0)
                        {
                            client.sendColoredNotification("Es wurde nichts weggegeben..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_ORANGE);
                            OpenVehicleKeyOverviewForPlayer(client);
                            return;
                        }
                        if (keyCount == ch.KeyRing.VehicleKeys[data.EventInt].Count)
                        {
                            ch.KeyRing.VehicleKeys.Remove(data.EventInt);
                        }
                        else
                        {
                            ch.KeyRing.VehicleKeys[data.EventInt].Count -= keyCount;
                        }
                        var cht = plrs[0].Account().CurrentCharacter;
                        if (cht.KeyRing.VehicleKeys.ContainsKey(data.EventInt))
                        {
                            cht.KeyRing.VehicleKeys[data.EventInt].Count += keyCount;
                        }
                        else
                        {
                            cht.KeyRing.VehicleKeys.Add(data.EventInt, new KeyData(keyCount, data.EventString));
                        }
                        client.sendColoredNotification($"Der Schlüssel ({data.EventString}) wurde {keyCount}x weggegeben..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                        plrs[0].sendColoredNotification($"Du hast {keyCount}x den Schlüssel ({data.EventString}) erhalten..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                    }
                    break;
            }
        }

        private void OpenKeyOverviewForPlayer(Client client)
        {
            Menu menu = new Menu("Schlüsselbund", "Verfügbare Optionen:", "inventory");
            menu.Items.Add(new MenuItem("Fahrzeugschlüssel") { EventTrigger = "show_vehicle_keys"});
            menu.Show(client);
        }

        private void OpenVehicleKeyOverviewForPlayer(Client client)
        {
            Menu menu = new Menu("Fahrzeugschlüssel", "Vefügbare Optionen:", "inventory");
            foreach (var key in client.Account().CurrentCharacter.KeyRing.VehicleKeys)
            {
                menu.Items.Add(new MenuItem(key.Value.Label, "", $"{key.Value.Count}x", true, "show_vehicle_key_options", key.Value.Label, key.Key));
            }
            if(menu.Items.Count == 0)
            {
                menu.Items.Add(new MenuItem("Keine Schlüssel vorhanden") { EventTrigger = "key_overview" });
            }
            menu.Show(client);
        }

        private void OpenVehicleKeyOptions(Client client, MenuEventData data)
        {
            Menu menu = new Menu("Schlüssel", "Was möchtest du tun?", "inventory");
            menu.Items.Add(new MenuItem("Geben") { EventTrigger = "give_away_vehicle_key", EventInt = data.EventInt, EventString = data.EventString, OpenUserInput = true, UserInputDefaultText = "0" });
            menu.Items.Add(new MenuItem("Wegwerfen") { EventTrigger = "throw_away_vehicle_key", EventInt = data.EventInt, EventString = data.EventString, OpenUserInput = true, UserInputDefaultText = "0" });
            menu.Show(client);
        }
    }
}
