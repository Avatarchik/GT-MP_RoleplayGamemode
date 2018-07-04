using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace Roleplay.Server.Models
{
    public class InteractData
    {
        public Client Client { get; set; }
        public NetHandle Handle { get; set; }
        public int Model { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public EntityType EntityType { get; set; }
    }
}