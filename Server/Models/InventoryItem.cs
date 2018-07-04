using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Models
{
    public class InventoryItem
    {
        public int ItemId { get; set; }
        public int Count { get; set; }
        public List<int> ItemModifier { get; set; }
    }
}
