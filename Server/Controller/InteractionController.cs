using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Roleplay.Base;
using Roleplay.Server.Enums;
using Roleplay.Server.Extensions;
using Roleplay.Server.Managers;
using Roleplay.Server.Models;
using Roleplay.Server.Models.MenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Controller
{
    public class InteractionController : RoleplayScript
    {
        public delegate void EventHandlerClientVehicle<TSender, TEventArgs>(TSender client, TEventArgs vehicle);

        public static event EventHandler<int, InteractData> OnPlayerInteractWithObject;
        public static event EventHandlerClientVehicle<Client, Vehicle> OnPlayerInteractInVehicle;
        public static event EventHandler<Client> OnPlayerInteractOnFoot;
        public static event EventHandler<Client> OnPlayerOpenInventory;
        public static event EventHandlerClientVehicle<Client, Vehicle> OnPlayerInteractWithVehicle;

        public InteractionController()
        {
            ClientEventManager.RegisterClientEvent("INTERACTION_ONFOOT", OnFootInteraction);
            ClientEventManager.RegisterClientEvent("INTERACTION_IN_VEHICLE", InVehicleInteraction);
            ClientEventManager.RegisterClientEvent("INTERACTION_WITH_OBJECT", ObjectInteraction);
            ClientEventManager.RegisterClientEvent("INTERACTION_WITH_VEHICLE", VehicleInteraction);
            ClientEventManager.RegisterClientEvent("INTERACTION_WITH_UNKNOWN_OBJECT", UnknownObjectInteraction);
            ClientEventManager.RegisterClientEvent("INTERACTION_ONFOOT_INVENTORY", OnFootInventoryInteraction);
        }

        private void OnFootInventoryInteraction(Client sender, string eventName, object[] arguments)
        {
            OnPlayerOpenInventory?.Invoke(this, sender);
        }

        public static List<int> InteractionObjects = new List<int>();
        public static List<MenuHandlerDelegate> _InteractionDelegates = new List<MenuHandlerDelegate>();

        private void UnknownObjectInteraction(Client client, string eventName, object[] arguments)
        {
            NetHandle target = (NetHandle)arguments[0];
            int model = Convert.ToInt32(arguments[1]);
            Vector3 position = (Vector3)arguments[2];
            Vector3 rotation = (Vector3)arguments[3];
            EntityType entType = (EntityType)Convert.ToInt32(arguments[4]);
            if(client.position.DistanceTo(position) > 10f)
            {
                return;
            }
            logger.Debug($"Admin Object Interaction: {target} [{model}] {API.getEntityType(target)} {entType} {position}");
        }

        private void VehicleInteraction(Client client, string eventName, object[] arguments)
        {
            NetHandle target = (NetHandle)arguments[0];
            int model = Convert.ToInt32(arguments[1]);
            Vector3 position = (Vector3)arguments[2];
            Vector3 rotation = (Vector3)arguments[3];
            EntityType entType = (EntityType)Convert.ToInt32(arguments[4]);
            if (client.position.DistanceTo(position) > 10f)
            {
                return;
            }
            logger.Debug($"Vehicle Interaction: {target} [{model}] {API.getEntityType(target)} {entType} {position}");
            if (target == null)
                return;
            Vehicle vehicle = API.getEntityFromHandle<Vehicle>(target);
            if (vehicle == null)
                return;
            OwnedVehicle oveh = OwnedVehicleController.ExistingVehicles.FirstOrDefault(x => x.Handle.handle == vehicle.handle);
            if (oveh == null)
                return;
            var menu = new Menu("Interaktion", "Verfügbare Optionen:", "vehicle_interaction") { ExtraInt = vehicle.handle.Value};

            foreach (var item in _InteractionDelegates.Where(id => id.EntityType == EntityType.Vehicle).OrderBy(id => id.position))
            {
                item.Handler(client, menu);
            }
            if(menu.Items.Count != 0)
            {
                menu.Show(client);
            }
            OnPlayerInteractWithVehicle?.Invoke(client, vehicle);
        }

        private void ObjectInteraction(Client client, string eventName, object[] arguments)
        {
            NetHandle target = (NetHandle)arguments[0];
            int model = Convert.ToInt32(arguments[1]);
            Vector3 position = (Vector3)arguments[2];
            Vector3 rotation = (Vector3)arguments[3];
            EntityType entType = (EntityType)Convert.ToInt32(arguments[4]);
            if (client.position.DistanceTo(position) > 10f)
            {
                return;
            }
            if (InteractionObjects.Contains(model))
            {
                var iData = new InteractData
                {
                    Client = client,
                    Handle = target,
                    Model = model,
                    Position = position,
                    Rotation = rotation,
                    EntityType = entType
                };
                OnPlayerInteractWithObject?.Invoke(model, iData);
                //logger.Debug($"Player {client.socialClubName} interact with valid Object ({model})");
            }
        }

        private void InVehicleInteraction(Client client, string eventName, object[] arguments)
        {
            if (!client.IsLoggedIn())
                return;
            if (client.isInVehicle)
            {
                var menu = new Menu("Interaktion", "Verfügbare Optionen:", "vehicle_interaction") { ExtraInt = client.vehicle.handle.Value };

                foreach (var item in _InteractionDelegates.Where(id => id.EntityType == EntityType.Vehicle).OrderBy(id => id.position))
                {
                    item.Handler(client, menu);
                }
                if (menu.Items.Count != 0)
                {
                    menu.Show(client);
                }
                OnPlayerInteractInVehicle?.Invoke(client, client.vehicle);
                //logger.Debug($"Player {client.socialClubName} interact InVehicle");
            }
        }

        private void OnFootInteraction(Client client, string eventName, object[] arguments)
        {
            if (!client.isInVehicle)
            {
                OnPlayerInteractOnFoot?.Invoke(this, client);
                //logger.Debug($"Player {client.socialClubName} interact OnFoot");
            }
        }

        public static void UpdateInteractableObjectsForPlayer(Client client)
        {
            client.triggerEvent("REGISTER_INTERACTION_MODEL", JsonConvert.SerializeObject(InteractionObjects));
        }

        public static void UpdateInteractableObjectsForAll()
        {
            GameMode.sharedAPI.getAllPlayers().ToList().Where(plr => plr.IsLoggedIn()).ForEach(plr =>
            {
                UpdateInteractableObjectsForPlayer(plr);
            });
        }

        public static void AddInteractionDelegate(EntityType eType, InteractionMenuPositions position, Action<Client, Menu> inventoryAction)
        {
            _InteractionDelegates.Add(new MenuHandlerDelegate() { EntityType = eType, position = (int)position, Handler = inventoryAction });
        }
    }
}
