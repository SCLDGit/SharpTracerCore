using System;
using System.Collections.Generic;
using System.Text;
using RenderDataStructures.Basics;
using RenderDataStructures.Shapes;

namespace RenderDataStructures.Materials
{
    public class Glossy : IMaterial
    {
        public Glossy(Color p_albedo, double p_roughness = 0.2d)
        {
            Albedo = p_albedo;

            if (p_roughness < 0.001d)
            {
                Roughness = 0;
            }
            else if (p_roughness > 1.0d)
            {
                Roughness = 1.0d;
            }
            else
            {
                Roughness = p_roughness;
            }
        }

        public Color Albedo { get; private set; }
        public double Roughness { get; private set; }

        public bool ScatterRay(ref Ray p_incomingRay, ref HitRecord p_hitRecord, ref Color p_attenuation, ref Ray p_scatteredRay)
        {
            var reflected = MathUtilities.ReflectRay(Vec3.GetUnitVector(p_incomingRay.Direction), p_hitRecord.Normal);
            p_scatteredRay = new Ray(p_hitRecord.P, reflected + Roughness * MathUtilities.GetRandomPointInUnitSphere(), p_incomingRay.Depth + 1);
            p_attenuation = Albedo;
            return Vec3.GetDotProduct(p_scatteredRay.Direction, p_hitRecord.Normal) > 0;
        }
    }
}
