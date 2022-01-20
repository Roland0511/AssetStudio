using AssetStudio.Metas.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudio.Extensions
{
    public static class VectorExtensions
    {
        public static SpriteAlignment ToAlignment(this Vector2 vector)
        {
            var pX = vector.X;
            var pY = vector.Y;
            if (pX == 0.5 && pY == 0.5)
            {
                return SpriteAlignment.Center;
            }
            else if (pX == 0 && pY == 1)
            {
                return SpriteAlignment.TopLeft;
            }
            else if (pX == 0.5 && pY == 1)
            {
                return SpriteAlignment.TopCenter;
            }
            else if (pX == 1 && pY == 1)
            {
                return SpriteAlignment.TopRight;
            }
            else if (pX == 0 && pY == 0.5)
            {
                return SpriteAlignment.LeftCenter;
            }
            else if (pX == 1 && pY == 0.5)
            {
                return SpriteAlignment.RightCenter;
            }
            else if (pX == 0 && pY == 0)
            {
                return SpriteAlignment.BottomLeft;
            }
            else if (pX == 0.5 && pY == 0)
            {
                return SpriteAlignment.BottomCenter;
            }
            else if (pX == 1 && pY == 0)
            {
                return SpriteAlignment.BottomRight;
            }
            else
            {
                return SpriteAlignment.Custom;
            }
        }
    }
}
