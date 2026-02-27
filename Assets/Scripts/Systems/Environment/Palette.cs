using UnityEngine;

namespace SnakePrototype.Systems.Environment
{
    [System.Serializable]
    public class Palette
    {
        public Color Background;
        public Color GridAccent;
        public Color CoreAccent;
        public Color GuardAccent;
        public Color UIAccent;
        public Color AlertAccent;

        public Palette()
        {
            Background = Color.black;
            GridAccent = Color.cyan;
            CoreAccent = Color.yellow;
            GuardAccent = Color.red;
            UIAccent = Color.white;
            AlertAccent = Color.red;
        }

        public Palette(Color background, Color accent, Color neutral)
        {
            Background = background;
            GridAccent = neutral;
            CoreAccent = accent;
            GuardAccent = accent;
            UIAccent = accent;
            AlertAccent = Color.red;
        }
    }
}
