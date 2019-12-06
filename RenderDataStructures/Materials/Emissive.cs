using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;
using RenderDataStructures.Shapes;

namespace RenderDataStructures.Materials
{
    public class Emissive : IMaterial
    {
        public Emissive(ITexture p_color, double p_power)
        {
            Color = p_color;
            Power = p_power;
        }

        public ITexture Color { get; set; }
        public double Power { get; set; }

        public bool ScatterRay(ref Ray p_incomingRay, ref HitRecord p_hitRecord, ref Color p_attenuation, ref Ray p_scatteredRay)
        {
            return false;
        }

        public Color GetEmitted(double p_u, double p_v, Vec3 p_point)
        {
            return Color.GetColor(p_u, p_v, p_point) * Power;
        }
    }
}
