using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
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

namespace Roleplay.Server.Controller.Admin
{
    public class AdminVehicleController : RoleplayScript
    {
        public AdminVehicleController()
        {
            GameMode.OnWorldStartup += GameMode_OnWorldStartup;
            MenuController.OnPlayerMenuSelectEvent += MenuController_OnPlayerMenuSelectEvent;
        }

        private void GameMode_OnWorldStartup()
        {
            InteractionController.AddInteractionDelegate(EntityType.Vehicle, InteractionMenuPositions.ADMIN, AddVehicleInteractionMenu);
        }

        private void MenuController_OnPlayerMenuSelectEvent(Client client, MenuEventData data)
        {
            if (!client.IsLoggedIn() || (int)client.Account().AdminLevel < 1)
            {
                client.sendNotification("", "~r~Keine Berechtigung!");
                return;
            }
            if(data.MenuIdentifier == "vehicle_interaction" && data.EventTrigger == "admin_main")
            {
                OpenVehicleAdminMenu(client, data);
                return;
            }
            if (data.MenuIdentifier == "vehicle_interaction_admin")
            {
                OwnedVehicle vehicle = OwnedVehicleController.ExistingVehicles.FirstOrDefault(x => x.Handle.Value == data.EventInt);
                if (vehicle == null)
                {
                    MenuController.CloseAllMenus(client);
                    return;
                }
                switch (data.EventTrigger)
                {
                    case "admin_park_vehicle":
                        OwnedVehicleController.ParkVehicle(vehicle);
                        MenuController.CloseAllMenus(client);
                        client.sendColoredNotification($"Das Fahrzeug(ID: {vehicle.Id}) wurde erfolgreich eingeparkt.", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                        break;
                    case "admin_create_vehicle_key":
                        if (client.Account().CurrentCharacter.KeyRing.VehicleKeys.ContainsKey(vehicle.Id))
                        {
                            client.Account().CurrentCharacter.KeyRing.VehicleKeys[vehicle.Id].Count++;
                        }
                        else
                        {
                            client.Account().CurrentCharacter.KeyRing.VehicleKeys.Add(vehicle.Id, new KeyData(1, $"{vehicle.ModelName} ({vehicle.NumberPlate})"));
                        }
                        client.sendColoredNotification($"Es wurde erfolgreich ein Schlüssel für das Fahrzeug(ID: {vehicle.Id}) erstellt.", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                        MenuController.CloseAllMenus(client);
                        break;
                }
            }
        }

        private void AddVehicleInteractionMenu(Client client, Menu menu)
        {
            if (!client.IsLoggedIn() || (int)client.Account().AdminLevel < 1)
                return;
            menu.Items.Add(new MenuItem("~r~Admin") { EventInt = menu.ExtraInt, EventTrigger = "admin_main"});
        }

        private void OpenVehicleAdminMenu(Client client, MenuEventData data)
        {
            OwnedVehicle vehicle = OwnedVehicleController.ExistingVehicles.FirstOrDefault(x => x.Handle.handle.Value == data.EventInt);
            if(vehicle == null)
            {
                MenuController.CloseAllMenus(client);
                client.sendNotification("", "~r~Ungültiges Fahrzeug");
                return;
            }
            var menu = new Menu("Vehicle Admin", "Verfügbare Optionen:", "vehicle_interaction_admin") { ExtraInt = data.EventInt};
            menu.Items.Add(new MenuItem("Einparken") { EventTrigger = "admin_park_vehicle", EventInt = data.EventInt });
            if(vehicle.Id != 0)
            {
                menu.Items.Add(new MenuItem("Schlüssel erstellen") { EventTrigger = "admin_create_vehicle_key", EventInt = data.EventInt });
            }
            menu.Show(client);
        }
    }
}
