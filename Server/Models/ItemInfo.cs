using Roleplay.Server.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Server.Models
{
    [Table("ItemInformations")]
    public class ItemInfo
    {
        [Key]
        public int Id { get; set; }

        public ItemType Type { get; set; }
        public string Name { get; set; }
        public string NamePlural { get; set; }
        public string Description { get; set; }
        public int Weight { get; set; }
        public double BuyPrice { get; set; }
        public double SellPrice { get; set; }

        public string UsageTrigger { get; set; }
        public string DisposeTrigger { get; set; }

        public bool Usable { get; set; } // Benutzbar
        public bool Disposable { get; set; } // Wegwerfbar
        public bool Buyable { get; set; } // Kaufbar
        public bool Sellable { get; set; } // Verkäuflich
        public bool Giftable { get; set; } // Verschenkbar
    }
}