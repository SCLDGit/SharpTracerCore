using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;
using RenderDataStructures.Shapes;

namespace RenderDataStructures.Materials
{
    public interface IMaterial
    {
        bool ScatterRay(ref Ray p_incomingRay, ref HitRecord p_hitRecord, ref Color p_attenuation, ref Ray p_scatteredRay);

        public Color GetEmitted(double p_u, double p_v, Vec3 p_point)
        {
            return new Color(0, 0, 0);
        }
    }
}
