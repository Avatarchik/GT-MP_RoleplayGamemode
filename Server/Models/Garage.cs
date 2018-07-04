using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Roleplay.Server.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Models
{
    [Table("Garages")]
    public class Garage
    {
        [Key]
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Position { get; set; }
        public string Name { get; set; }

        public string PedPosition { get; set; }
        public float PedRotation { get; set; }

        public int OwnerUser { get; set; }
        public int OwnerGroup { get; set; }
        public int OwnerProperty { get; set; }

        public GarageType Type { get; set; }

        public bool NeedKey { get; set; }

        public int MaxTotalVehicle { get; set; }
        public int MaxBigVehicle { get; set; }
        public int MaxMediumVehicle { get; set; }
        public int MaxSmallVehicle { get; set; }

        [NotMapped]
        public List<GarageParkOutSpot> ParkOutSpots { get; set; }
        public string ParkOutSpotsString { get; set; }

        [NotMapped]
        public List<GarageParkInSpot> ParkInSpots { get; set; }
        public string ParkInSpotsString { get; set; }

        // Map Entitys
        [NotMapped]
        public Ped WorldPed { get; set; }
        [NotMapped]
        public Marker WorldMarker { get; set; }
        [NotMapped]
        public Blip WorldBlip { get; set; }

        public Garage()
        {
            Name = "";
            Enabled = true;
            OwnerUser = 0;
            OwnerGroup = 0;
            OwnerProperty = 0;
            NeedKey = false;
            Type = GarageType.Cars;
            MaxTotalVehicle = -1;
            MaxBigVehicle = -1;
            MaxMediumVehicle = -1;
            MaxSmallVehicle = -1;
        }

        public void SpotsToSpotStrings()
        {
            ParkOutSpotsString = JsonConvert.SerializeObject(ParkOutSpots);
            ParkInSpotsString = JsonConvert.SerializeObject(ParkInSpots);
        }

        public void SpotStringsToSpots()
        {
            if (ParkInSpotsString != "" && ParkInSpotsString != null)
            {
                ParkInSpots = JsonConvert.DeserializeObject<List<GarageParkInSpot>>(ParkInSpotsString);
            }
            else
            {
                ParkInSpots = new List<GarageParkInSpot>();
            }

            if (ParkOutSpotsString != "" && ParkOutSpotsString != null)
            {
                ParkOutSpots = JsonConvert.DeserializeObject<List<GarageParkOutSpot>>(ParkOutSpotsString);
            }
            else
            {
                ParkOutSpots = new List<GarageParkOutSpot>();
            }
        }
    }

    public class GarageParkOutSpot
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public bool BigVehicleAllowed { get; set; }
        public float Radius { get; set; }
    }

    public class GarageParkInSpot
    {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
    }
}
