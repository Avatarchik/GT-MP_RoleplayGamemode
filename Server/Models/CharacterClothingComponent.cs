namespace Roleplay.Server.Models
{
    public class CharacterClothingComponent
    {
        public int Drawable { get; set; }
        public int Texture { get; set; }

        public CharacterClothingComponent(int drawable, int texture)
        {
            Drawable = drawable;
            Texture = texture;
        }
    }

    public class CharacterClothingComponentTorso
    {
        public CharacterClothingComponent Torso { get; set; }
        public CharacterClothingComponent Undershirt { get; set; }
        public CharacterClothingComponent Top { get; set; }

        public CharacterClothingComponentTorso(CharacterClothingComponent torso, CharacterClothingComponent undershirt, CharacterClothingComponent top)
        {
            Torso = torso;
            Undershirt = undershirt;
            Top = top;
        }
    }
}