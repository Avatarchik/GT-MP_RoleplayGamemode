using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Controller
{
    public class AntiCheatController
    {
        public AntiCheatController()
        {

        }

        public static void TeleportPlayer(Client client, Vector3 position, Vector3 rotation, bool withVehicle = false)
        {
            if (client.isInVehicle && withVehicle)
            {
                client.vehicle.position = position;
                client.vehicle.rotation = rotation;
            }
            else
            {
                client.position = position;
                client.rotation = rotation;
            }
        }

        public static void TeleportPlayer(Client client, Vector3 position, bool withVehicle = false)
        {
            TeleportPlayer(client, position, client.rotation, withVehicle);
        }
    }
}
