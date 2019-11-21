﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MathUtilities;
using RenderDataStructures.Shapes;

namespace RenderDataStructures.Materials
{
    public class Glossy : IMaterial
    {
        public Glossy(ITexture p_albedoTexture, double p_roughness = 0.0d)
        {
            Albedo = p_albedoTexture;

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

        public ITexture Albedo { get; private set; }
        public double Roughness { get; private set; }

        public bool ScatterRay(ref Ray p_incomingRay, ref HitRecord p_hitRecord, ref Color p_attenuation, ref Ray p_scatteredRay)
        {
            var reflected = Utilities.ReflectRay(Vec3.GetUnitVector(p_incomingRay.Direction), p_hitRecord.Normal);
            p_scatteredRay = new Ray(p_hitRecord.P, reflected + Roughness * Utilities.GetRandomPointInUnitSphere(), p_incomingRay.Time, p_incomingRay.Depth + 1);
            p_attenuation = Albedo.GetColor(0, 0, p_hitRecord.P);
            return Vec3.GetDotProduct(p_scatteredRay.Direction, p_hitRecord.Normal) > 0;
        }
    }
}
