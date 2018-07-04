using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using Roleplay.Base;
using Roleplay.Server.Controller;
using Roleplay.Server.Enums;
using Roleplay.Server.Extensions;
using Roleplay.Server.Models;
using Roleplay.Server.Models.MenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Admin
{
    public class Commands : RoleplayScript
    {
        public Commands()
        {

        }

        [Command("aveh")]
        public void SpawnAdminVehicle(Client client, VehicleHash vehicle)
        {
            if (!client.IsLoggedIn())
                return;
            var acc = client.Account();
            if ((int)acc.AdminLevel < 2)
                return;
            var veh = API.createVehicle(vehicle, client.position, client.rotation, 25, 30, client.dimension);
            veh.numberPlate = "ADMIN";
            veh.waitForPlayerSynchronization(client);
            API.setPlayerIntoVehicle(client, veh, -1);
            OwnedVehicleController.ExistingVehicles.Add(new OwnedVehicle
            {
                Id = 0,
                Handle = veh,
                NumberPlate = "ADMIN"
            });
        }

        [Command("editcharacter")]
        public void AdminAllowReCreateCharacter(Client client, Client targetPlayer)
        {
            if (!client.IsLoggedIn())
                return;
            var acc = client.Account();
            if ((int)acc.AdminLevel < 5)
                return;
            CharacterEditorController.OpenCharacterEditorForPlayer(targetPlayer, true);
            API.sendColoredNotificationToPlayer(client, $"{targetPlayer.socialClubName} darf nun seinen Charakter überarbeiten",
                (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
        }

        [Command("addgarage", GreedyArg = true)]
        public void AdminAddGarage(Client client, string garageName)
        {
            if (!client.IsLoggedIn())
                return;
            var acc = client.Account();
            if ((int)acc.AdminLevel < 5)
                return;
            Garage garage = new Garage();
            garage.Position = client.position.ToJson();
            garage.PedPosition = garage.Position;
            garage.PedRotation = client.rotation.Z;
            garage.Name = garageName;
            using (var db = new Database())
            {
                db.Garages.Add(garage);
                db.SaveChanges();
                API.sendNotificationToPlayer(client, $"Garage mit der ID: {garage.Id} wurde hinzugefügt.");
            }
        }

        [Command("addgaragespawn")]
        public void AdminAddGarageSpawn(Client client, int garageId)
        {
            if (!client.IsLoggedIn())
                return;
            var acc = client.Account();
            if ((int)acc.AdminLevel < 5)
                return;
            using (var db = new Database())
            {
                Garage garage = db.Garages.FirstOrDefault(x => x.Id == garageId);
                if (garage == null)
                {
                    API.sendNotificationToPlayer(client, $"Die Gargae mit der ID: {garageId} wurde nicht gefunden.");
                    return;
                }
                garage.SpotStringsToSpots();
                garage.ParkOutSpots.Add(new GarageParkOutSpot
                {
                    BigVehicleAllowed = true,
                    Position = client.position,
                    Rotation = client.rotation,
                    Radius = 6f
                });
                garage.SpotsToSpotStrings();
                db.SaveChanges();
                API.sendNotificationToPlayer(client, $"Ausparkpunkt wurde zur Garage(ID: {garageId}) hinzugefügt.");
            }
        }

        [Command("addgarageparkspot")]
        public void AdminAddGarageParkInSpot(Client client, int garageId)
        {
            if (!client.IsLoggedIn())
                return;
            var acc = client.Account();
            if ((int)acc.AdminLevel < 5)
                return;
            using (var db = new Database())
            {
                Garage garage = db.Garages.FirstOrDefault(x => x.Id == garageId);
                if (garage == null)
                {
                    API.sendNotificationToPlayer(client, $"Die Gargae mit der ID: {garageId} wurde nicht gefunden.");
                    return;
                }
                garage.SpotStringsToSpots();
                garage.ParkInSpots.Add(new GarageParkInSpot
                {
                    Position = client.position,
                    Radius = 10f
                });
                garage.SpotsToSpotStrings();
                db.SaveChanges();
                API.sendNotificationToPlayer(client, $"Einparkpunkt wurde zur Garage(ID: {garageId}) hinzugefügt.");
            }
        }

        [Command("addvehiclekey")]
        public void AdminAddVehicleKey(Client client, int vehicleId)
        {
            if (!client.IsLoggedIn())
                return;
            var acc = client.Account();
            if ((int)acc.AdminLevel < 5)
                return;
            acc.CurrentCharacter.KeyRing.VehicleKeys.Add(vehicleId, new KeyData(1, "Admin Key Veh: " + vehicleId));
            API.sendNotificationToPlayer(client, $"Schlüsself für die Fahrzeug ID: {vehicleId} wurde zum Schlüsselbund hinzugefügt.");
        }

    }
}
