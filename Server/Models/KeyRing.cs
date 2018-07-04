using System;
using System.Collections.Generic;

namespace Roleplay.Server.Models
{
    public class KeyRing
    {
        public Dictionary<int, KeyData> VehicleKeys { get; set; }

        public KeyRing()
        {
            VehicleKeys = new Dictionary<int, KeyData>();
        }

        internal void ToList()
        {
            throw new NotImplementedException();
        }
    }

    public class KeyData
    {
        public int Count { get; set; }
        public string Label { get; set; }

        public KeyData(int count, string label)
        {
            Count = count;
            Label = label;
        }
    };
}