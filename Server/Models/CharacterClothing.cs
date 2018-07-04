using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Models
{
    public class CharacterClothing
    {
        public CharacterClothingComponent Mask { get; set; }
        public CharacterClothingComponentTorso Top { get; set; }
        public CharacterClothingComponent Leg { get; set; }
        public CharacterClothingComponent Feet { get; set; }
        public CharacterClothingComponent Vest { get; set; }
        public CharacterClothingComponent Bag { get; set; }
        public CharacterClothingComponent Decal { get; set; } 
        public CharacterClothingComponent Accessories { get; set; }

        public CharacterClothingComponent Hat { get; set; }
        public CharacterClothingComponent Glasses { get; set; }
        public CharacterClothingComponent Ears { get; set; }
        public CharacterClothingComponent Watches { get; set; }
        public CharacterClothingComponent Bracelets { get; set; }



    }
}
