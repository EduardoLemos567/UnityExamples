using UnityEngine;

namespace Game
{
    /// <summary>
    /// Utility to class to work with (and keep) HSV values the same way you would
    /// work with 'Color'. Also supports alpha, so you dont lose its value.
    /// </summary>
    public struct ColorHSV
    {
        public float h;
        public float s;
        public float v;
        public float a;
        public ColorHSV(Color color)
        {
            Color.RGBToHSV(color, out h, out s, out v);
            a = color.a;
        }
        public Color Color
        {
            get
            {
                var color = Color.HSVToRGB(h, s, v);
                color.a = a;
                return color;
            }
        }
    }
}