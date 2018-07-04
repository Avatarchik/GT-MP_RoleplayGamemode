using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Roleplay.Base;
using Roleplay.Server.Base;
using Roleplay.Server.Enums;
using Roleplay.Server.Extensions;
using Roleplay.Server.Managers;
using Roleplay.Server.Models;
using Roleplay.Server.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Controller
{
    public class GarageController : RoleplayScript
    {
        public GarageController(){
            GameMode.OnWorldStartup += GameMode_OnWorldStartup;
            GameMode.OnWorldReloadConfig += GameMode_OnWorldReloadConfig;
            ClientEventManager.RegisterClientEvent("GARAGE_PARK_IN", ParkInGarageEvent);
            ClientEventManager.RegisterClientEvent("GARAGE_PARK_OUT", ParkOutGarageEvent);
            ClientEventManager.RegisterClientEvent("GARAGE_CLOSE", CloseParkMenu);
            ClientEventManager.RegisterClientEvent("GARAGE_PARK_OUT_VEHICLE", ParkOutVehicleEvent);
            InteractionController.OnPlayerInteractOnFoot += InteractionController_OnPlayerInteractOnFoot;
        }

        private void ParkOutVehicleEvent(Client client, string eventName, object[] arguments)
        {
            CloseWindow(client);
            int id = Convert.ToInt32(arguments[0]);
            var garage = Garages.FirstOrDefault(x => x.Position.FromJson<Vector3>().DistanceTo(client.position) <= 2f);
            if (garage == null)
                return;
            GarageParkOutSpot foundSpot = null;
            foreach (GarageParkOutSpot spot in garage.ParkOutSpots)
            {
                if(OwnedVehicleController.IsAnyVehicleBlockingThisArea(spot.Position, spot.Radius))
                {
                    continue;
                }
                else
                {
                    foundSpot = spot;
                    break;
                }
            }
            if(foundSpot != null)
            {
                OwnedVehicleController.ParkOutVehicle(id, foundSpot.Position, foundSpot.Rotation);
                client.sendColoredNotification("Das Fahrzeug wurde erfolgreich ausgeparkt.", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
            }
            else
            {
                client.sendColoredNotification("Es wurde kein freier Parkplatz gefunden..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_RED);
            }
        }

        private void InteractionController_OnPlayerInteractOnFoot(object sender, Client e)
        {
            var garage = Garages.FirstOrDefault(x => x.Position.FromJson<Vector3>().DistanceTo(e.position) <= 1f);
            if (garage == null)
                return;
            OpenParkOverviewWindow(e, garage);
        }

        private void CloseParkMenu(Client sender, string eventName, object[] arguments)
        {
            CloseWindow(sender);
        }

        private void ParkOutGarageEvent(Client client, string eventName, object[] arguments)
        {
            Character ch = client.Account().CurrentCharacter;
            var garage = Garages.FirstOrDefault(x => x.Position.FromJson<Vector3>().DistanceTo(client.position) <= 1f);
            if (garage == null)
                return;
            List<GarageVehicleList> list = new List<GarageVehicleList>();
            List<OwnedVehicle> vehicles = new List<OwnedVehicle>();
            using (var db = new Database())
            {
                vehicles = db.OwnedVehicles.Where(x => ch.KeyRing.VehicleKeys.Keys.ToList().Contains(x.Id) && x.InGarage && x.CanParkOutEverywhere ||
                ch.KeyRing.VehicleKeys.Keys.ToList().Contains(x.Id) && !x.CanParkOutEverywhere && garage.Id == x.LastGarageId).ToList();
            }

            foreach (OwnedVehicle vehicle in vehicles)
            {
                list.Add(new GarageVehicleList
                {
                    Id = vehicle.Id.ToString(),
                    Title = vehicle.ModelName
                });
            }

            if (list.Count == 0)
            {
                list.Add(new GarageVehicleList
                {
                    Id = "0",
                    Title = "Es wurden keine Fahrzeuge gefunden.."
                });
            }
            client.triggerEvent("showBrowser", Constants.GarageParkOutListUrl, "FillWindow", JsonConvert.SerializeObject(list));
        }

        private void ParkInGarageEvent(Client client, string eventName, object[] arguments)
        {
            CloseWindow(client);
            var garage = Garages.FirstOrDefault(x => x.Position.FromJson<Vector3>().DistanceTo(client.position) <= 2f);
            if (garage == null)
            {
                client.sendColoredNotification("Du bist zu weit von einer Garage entfernt..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_RED);
                return;
            }
            if(garage.ParkOutSpots.Count == 0)
            {
                client.sendColoredNotification($"Es wurde kein Ausparkpunkt gefunden.. Melde dies einem Admin ({garage.Id})", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_RED);
                return;
            }
            OwnedVehicle vehicle = OwnedVehicleController.ExistingVehicles.FirstOrDefault(x => client.Account().CurrentCharacter.KeyRing.VehicleKeys.Keys.ToList().Contains(x.Id) &&
            x.Handle.position.DistanceTo(garage.ParkInSpots[0].Position) <= garage.ParkInSpots[0].Radius);
            if(vehicle == null)
            {
                client.sendColoredNotification("Es wurde kein einparkbares Fahrzeug gefunden..", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_RED);
                return;
            }
            OwnedVehicleController.ParkVehicle(vehicle, garage.Id, false);
            client.sendColoredNotification("Das Fahrzeug wurde erfolgreich eingeparkt.", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
        }

        public static List<Garage> Garages = new List<Garage>();

        private void GameMode_OnWorldReloadConfig()
        {
            LoadGarages(true);
            logger.Debug($"Start reloading Garages..");
        }

        private void GameMode_OnWorldStartup()
        {
            LoadGarages();
            logger.Debug($"Start loading Garages..");
        }

        private void LoadGarages(bool reload = false)
        {
            if (reload)
            {
                // Clean
                Garages.ForEach(garage =>
                {
                    if(garage.WorldBlip != null)
                    {
                        API.deleteEntity(garage.WorldBlip);
                    }
                    if (garage.WorldMarker != null)
                    {
                        API.deleteEntity(garage.WorldMarker);
                    }
                    if (garage.WorldPed != null)
                    {
                        API.deleteEntity(garage.WorldPed);
                    }
                });
                Garages.Clear();
            }
            using (var db = new Database())
            {
                var garages = db.Garages.Where(x => x.Enabled).ToList();
                logger.Debug($"Loading {garages.Count} Garages..");
                garages.ForEach(garage =>
                {
                    garage.SpotStringsToSpots();
                    garage.WorldPed = API.createPed(PedHash.AfriAmer01AMM, garage.PedPosition.FromJson<Vector3>(), garage.PedRotation);
                    garage.WorldBlip = API.createBlip(garage.Position.FromJson<Vector3>());
                    garage.WorldBlip.shortRange = true;
                    garage.WorldBlip.name = garage.Name;
                    garage.WorldBlip.sprite = 50;
                    garage.WorldBlip.color = 26;
                    garage.WorldBlip.scale = 0.7f;
                    Garages.Add(garage);
                });
            }
        }

        public void OpenParkOverviewWindow(Client client, Garage garage)
        {
            client.triggerEvent("showBrowser", Constants.GarageOverviewUrl, "FillWindow", garage.Name);
        }

        private void CloseWindow(Client client)
        {
            client.triggerEvent("hideAccountModal");
        }
    }
}
