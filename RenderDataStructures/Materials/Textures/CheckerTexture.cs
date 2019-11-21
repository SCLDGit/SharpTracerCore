using System;
using MathUtilities;

namespace RenderDataStructures.Materials
{
    public class CheckerTexture : ITexture
    {
        private ITexture Odd { get; }
        private ITexture Even { get; }

        public CheckerTexture(ITexture p_texture1, ITexture p_texture2)
        {
            Odd = p_texture1;
            Even = p_texture2;
        }

        public Color GetColor(double p_u, double p_v, Vec3 p_point)
        {
            var sines = Math.Sin(10 * p_point.X) * Math.Sin(10 * p_point.Y) * Math.Sin(10 * p_point.Z);
            if (sines < 0)
            {
                return Odd.GetColor(p_u, p_v, p_point);
            }

            return Even.GetColor(p_u, p_v, p_point);
        }
    }
}
