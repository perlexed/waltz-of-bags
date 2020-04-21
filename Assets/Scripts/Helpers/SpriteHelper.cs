
using UnityEngine;


namespace Helpers
{
    public static class SpriteHelper
    {
        public static Color SetAlpha(Color color, float alpha)
        {
            Color newColor = new Color(
                color.r,
                color.g,
                color.b,
                alpha
            );

            return newColor;
        }
    }
}