using UnityEngine;
using System.Collections.Generic;
using System;

namespace RedHeartsFirst
{
    internal class LoadTexture
    {
        // ======== NOTE ~ RESOURCES ========
        // Project resources are already a byte[] array! c:

        FilterMode texFilter = FilterMode.Point;

        public LoadTexture(FilterMode filter)
        {
            texFilter = filter;
        }

        public Texture2D TextureFromBytes(byte[] array, float opacity = 0)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.filterMode = texFilter;
            ImageConversion.LoadImage(tex, array);

            if (opacity > 0 && opacity <= 1)
                tex = ChangeOpacity(tex, opacity);

            return tex; // Return Texture2D c:
        }

        public Texture2D ChangeOpacity(Texture2D tex, float opacity)
        {
            for (int y = 0; y < tex.height; y++) // Go through the Y-Axis
            {
                for (int x = 0; x < tex.width; x++) // Go through the X-Axis
                {
                    Color32 color = new Color(tex.GetPixel(x, y).r, tex.GetPixel(x, y).g, tex.GetPixel(x, y).b, opacity);

                    tex.SetPixel(x, y, color);
                }
            }
            tex.Apply();
            return tex;
        }

        public Sprite SpriteFromTexture(Texture2D tex)
        {
            Rect texRect = new Rect(0, 0, tex.width, tex.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(tex, texRect, pivot);
        }

        public Sprite MakeSprite(byte[] image, float opacity = 0)
        {
            return SpriteFromTexture(TextureFromBytes(image, opacity));
        }
    }
}