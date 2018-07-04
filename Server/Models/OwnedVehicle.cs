using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Roleplay.Server.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Server.Models
{
    [Table("OwnedVehicles")]
    public class OwnedVehicle
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsage { get; set; }
        public bool InGarage { get; set; }
        public int OwnerUser { get; set; }
        public int OwnerGroup { get; set; }
        public string ModelName { get; set; }
        public int ModelHash { get; set; }
        public int ColorPrimary { get; set; }
        public int ColorSecondary { get; set; }
        public string NumberPlate { get; set; }
        public float Health { get; set; }
        public float EngineHealth { get; set; }
        public double Fuel { get; set; }
        public bool IsDeath { get; set; }
        public float DirtLevel { get; set; }
        public string DamageString { get; set; }
        public string TuningString { get; set; }
        public string Position { get; set; }
        public string Rotation { get; set; }
        public int LastGarageId { get; set; }
        public bool CanParkOutEverywhere { get; set; }
        [NotMapped]
        public Vehicle Handle { get; set; }

        public OwnedVehicle()
        {
            CreatedAt = DateTime.Now;
            LastUsage = DateTime.Now;
            InGarage = true;
            OwnerUser = 0;
            OwnerGroup = 0;
            ColorPrimary = 0;
            ColorSecondary = 0;
            Health = 4000;
            EngineHealth = 4000;
            Fuel = 50;
            IsDeath = false;
            DamageString = "";
            TuningString = "";
            DirtLevel = 0;
            Position = new Vector3().ToJson();
            Rotation = new Vector3().ToJson();
            LastGarageId = 0;
            CanParkOutEverywhere = true;
        }
    }
}