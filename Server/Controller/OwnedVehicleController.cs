using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
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
    public class OwnedVehicleController : RoleplayScript
    {
        public OwnedVehicleController()
        {
            WorldEnvironmentController.OnUpdateWorldTime += WorldEnvironmentController_OnUpdateWorldTime;
            GameMode.OnWorldStartup += GameMode_OnWorldStartup;
            GameMode.OnWorldShutdown += GameMode_OnWorldShutdown;
            MenuController.OnPlayerMenuSelectEvent += MenuController_OnPlayerMenuSelectEvent;
        }

        private void GameMode_OnWorldShutdown()
        {
            ExistingVehicles.ForEach(veh =>
            {
                ParkVehicle(veh);
            });
        }

        private void GameMode_OnWorldStartup()
        {
            using (var db = new Database())
            {
                db.OwnedVehicles.Where(x => !x.InGarage || !x.CanParkOutEverywhere).ToList().ForEach(veh =>
                {
                    veh.InGarage = true;
                    veh.CanParkOutEverywhere = true;
                });
                db.SaveChangesAsync();
            }
            InteractionController.AddInteractionDelegate(EntityType.Vehicle, InteractionMenuPositions.FIRST, AddVehicleInteractionMenu);
        }        

        private void WorldEnvironmentController_OnUpdateWorldTime()
        {
            ExistingVehicles.Where(x => x.Handle != null && !x.InGarage).ToList().ForEach(veh =>
            {
                SaveVehicle(veh);
            });
        }

        public static readonly List<OwnedVehicle> ExistingVehicles = new List<OwnedVehicle>();

        public static void ParkOutVehicle(int vehicleId, Vector3 position, Vector3 rotation)
        {
            OwnedVehicle vehicle = null;
            using (var db = new Database())
            {
                vehicle = db.OwnedVehicles.FirstOrDefault(x => x.Id == vehicleId && x.InGarage);
                if(vehicle == null)
                {
                    sharedLogger.Debug($"Fahrzeug Id: {vehicleId} konnte nicht ausgeparkt werden..");
                    return;
                }
                vehicle.Handle = GameMode.sharedAPI.createVehicle(vehicle.ModelHash, position, rotation, vehicle.ColorPrimary, vehicle.ColorSecondary);
                vehicle.Handle.health = vehicle.Health;
                vehicle.Handle.engineHealth = vehicle.EngineHealth;
                vehicle.Handle.dirtLevel = vehicle.DirtLevel;
                vehicle.Handle.numberPlate = vehicle.NumberPlate;
                vehicle.Handle.locked = true;
                vehicle.Handle.engineStatus = false;
                ExistingVehicles.Add(vehicle);
                vehicle.InGarage = false;
                vehicle.LastUsage = DateTime.Now;
                db.SaveChangesAsync();
            }
            
        }

        public static void SaveVehicle(OwnedVehicle vehicle)
        {
            if (vehicle.Id == 0)
                return;
            if(vehicle.Handle != null && !GameMode.sharedAPI.isEntityAttachedToAnything(vehicle.Handle))
            {
                vehicle.Position = vehicle.Handle.position.ToJson();
                vehicle.Rotation = vehicle.Handle.rotation.ToJson();
                vehicle.Health = vehicle.Handle.health;
                vehicle.EngineHealth = vehicle.Handle.engineHealth;
                vehicle.DirtLevel = vehicle.Handle.dirtLevel;
            }
            using (var db = new Database())
            {
                OwnedVehicle veh = db.OwnedVehicles.FirstOrDefault(x => x.Id == vehicle.Id);
                if(veh == null)
                {
                    return;
                }
                veh.OwnerUser = vehicle.OwnerUser;
                veh.OwnerGroup = vehicle.OwnerGroup;
                veh.LastUsage = DateTime.Now;
                veh.InGarage = vehicle.InGarage;
                veh.ColorPrimary = vehicle.ColorPrimary;
                veh.ColorSecondary = vehicle.ColorSecondary;
                veh.NumberPlate = vehicle.NumberPlate;
                veh.Health = vehicle.Health;
                veh.EngineHealth = vehicle.EngineHealth;
                veh.Fuel = vehicle.Fuel;
                veh.IsDeath = vehicle.IsDeath;
                veh.DirtLevel = vehicle.DirtLevel;
                veh.DamageString = vehicle.DamageString;
                veh.TuningString = vehicle.TuningString;
                veh.Position = vehicle.Position;
                veh.Rotation = vehicle.Rotation;
                veh.LastGarageId = vehicle.LastGarageId;
                veh.CanParkOutEverywhere = vehicle.CanParkOutEverywhere;
                db.SaveChangesAsync();
            }
        }

        public static void ParkVehicle(OwnedVehicle vehicle, int garageId = 0, bool canParkOutEverywhere = true)
        {
            vehicle.InGarage = true;
            vehicle.LastGarageId = garageId;
            vehicle.CanParkOutEverywhere = canParkOutEverywhere;
            SaveVehicle(vehicle);
            if(vehicle.Handle != null)
                GameMode.sharedAPI.deleteEntity(vehicle.Handle);
            if(ExistingVehicles.Contains(vehicle))
                ExistingVehicles.Remove(vehicle);
        }

        public static bool IsAnyVehicleBlockingThisArea(Vector3 position, float radius)
        {
            List<OwnedVehicle> blockingVehicles = ExistingVehicles.Where(x => x.Handle.position.DistanceTo(position) <= radius).ToList();
            if(blockingVehicles.Count <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static OwnedVehicle GetNextAccessibleVehicleForPlayer(Client client, float maxDistance)
        {
            return ExistingVehicles.FirstOrDefault(x => client.Account().CurrentCharacter.KeyRing.VehicleKeys.Keys.ToList().Contains(x.Id) &&
            x.Handle.position.DistanceTo(client.position) <= maxDistance);
        }

        public static OwnedVehicle GetNextVehicleToPlayer(Client client, float maxDistance)
        {
            return ExistingVehicles.FirstOrDefault(x => x.Handle.position.DistanceTo(client.position) <= maxDistance);
        }

        private void AddVehicleInteractionMenu(Client client, Menu menu)
        {
            OwnedVehicle vehicle = ExistingVehicles.FirstOrDefault(x => x.Handle.Value == menu.ExtraInt);
            if (vehicle == null)
            {
                MenuController.CloseAllMenus(client);
                return;
            }
            if (client.isInVehicle)
            {
                if(client.vehicleSeat == -1 && client.HasPlayerVehicleKey(client.vehicle))
                {
                    if (client.vehicle.engineStatus)
                    {
                        menu.Items.Add(new MenuItem("Motor abschalten") { EventTrigger = "vehicle_toggle_engine", EventInt = menu.ExtraInt });
                    }
                    else
                    {
                        menu.Items.Add(new MenuItem("Motor anschalten") { EventTrigger = "vehicle_toggle_engine", EventInt = menu.ExtraInt });
                    }
                }
                if (client.seatbelt)
                {
                    menu.Items.Add(new MenuItem("Sicherheitsgurt ablegen") { EventTrigger = "vehicle_toggle_seatbelt", EventInt = menu.ExtraInt });
                }
                else
                {
                    menu.Items.Add(new MenuItem("Sicherheitsgurt anlegen") { EventTrigger = "vehicle_toggle_seatbelt", EventInt = menu.ExtraInt });
                }
            }
            if (client.HasPlayerVehicleKey(vehicle))
            {
                if (vehicle.Handle.locked)
                {
                    menu.Items.Add(new MenuItem("Fahrzeug aufschließen") { EventTrigger = "vehicle_toggle_locked", EventInt = menu.ExtraInt });
                }
                else
                {
                    menu.Items.Add(new MenuItem("Fahrzeug abschließen") { EventTrigger = "vehicle_toggle_locked", EventInt = menu.ExtraInt });
                }
            }
        }

        private void MenuController_OnPlayerMenuSelectEvent(Client client, MenuEventData data)
        {
            if (data.MenuIdentifier != "vehicle_interaction")
                return;
            OwnedVehicle vehicle = ExistingVehicles.FirstOrDefault(x => x.Handle.Value == data.EventInt);
            if (vehicle == null)
            {
                MenuController.CloseAllMenus(client);
                return;
            }
            MenuController.CloseAllMenus(client);
            switch (data.EventTrigger)
            {
                case "vehicle_toggle_engine":
                    if (vehicle.Handle.engineStatus)
                    {
                        client.sendColoredNotification($"Du hast den Motor abgeschaltet", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                    }
                    else
                    {
                        client.sendColoredNotification($"Du hast den Motor eingeschaltet", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                    }
                    vehicle.Handle.engineStatus = !vehicle.Handle.engineStatus;
                    break;
                case "vehicle_toggle_seatbelt":
                    if (client.seatbelt)
                    {
                        client.sendColoredNotification($"Du hast den Sicherheitsgurt entfernt", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                    }
                    else
                    {
                        client.sendColoredNotification($"Du hast den Sicherheitsgurt angelegt", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                    }
                    client.seatbelt = !client.seatbelt;
                    break;
                case "vehicle_toggle_locked":
                    if (vehicle.Handle.locked)
                    {
                        client.sendColoredNotification($"Fahrzeug({vehicle.ModelName}) wurde aufgeschlossen", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                    }
                    else
                    {
                        client.sendColoredNotification($"Fahrzeug({vehicle.ModelName}) wurde abgeschlossen", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
                    }
                    vehicle.Handle.locked = !vehicle.Handle.locked;
                    break;
            }
        }
    }
}
