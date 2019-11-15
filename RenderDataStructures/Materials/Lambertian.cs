using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;
using RenderDataStructures.Shapes;

namespace RenderDataStructures.Materials
{
    public class Lambertian : IMaterial
    {
        public Lambertian(Color p_albedo)
        {
            Albedo = p_albedo;
        }

        public Color Albedo { get; private set; }

        public bool ScatterRay(ref Ray p_incomingRay, ref HitRecord p_hitRecord, ref Color p_attenuation, ref Ray p_scatteredRay)
        {
            var target = p_hitRecord.P + p_hitRecord.Normal + Utilities.GetRandomPointInUnitSphere();
            p_scatteredRay = new Ray(p_hitRecord.P, target - p_hitRecord.P, p_incomingRay.Depth + 1);
            p_attenuation = Albedo;

            return true;
        }
    }
}
