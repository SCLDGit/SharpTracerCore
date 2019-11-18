﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MathUtilities;
using RenderDataStructures.Shapes;

namespace RenderDataStructures.Materials
{
    public class Dielectric : IMaterial
    {
        public Dielectric(Color p_albedo, double p_indexOfRefraction, double p_roughness = 0.0d)
        {
            Albedo = p_albedo;
            IndexOfRefraction = p_indexOfRefraction;

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
        public double IndexOfRefraction { get; private set; }
        public double Roughness { get; private set; }

        public bool ScatterRay(ref Ray p_incomingRay, ref HitRecord p_hitRecord, ref Color p_attenuation, ref Ray p_scatteredRay)
        {
            var rng = RandomPool.RandomPoolLUT[Thread.CurrentThread.ManagedThreadId];

            Vec3 outwardNormal;
            var reflected = Utilities.ReflectRay(p_incomingRay.Direction, p_hitRecord.Normal);
            var refracted = new Vec3(0);
            double niOverNt;

            p_attenuation = Albedo;

            double cosine;

            if (Vec3.GetDotProduct(p_incomingRay.Direction, p_hitRecord.Normal) > 0)
            {
                outwardNormal = -p_hitRecord.Normal;
                niOverNt = IndexOfRefraction;
                cosine = IndexOfRefraction * Vec3.GetDotProduct(p_incomingRay.Direction, p_hitRecord.Normal) /
                         p_incomingRay.Direction.GetLength();
            }
            else
            {
                outwardNormal = p_hitRecord.Normal;
                niOverNt = 1.0 / IndexOfRefraction;
                cosine = -Vec3.GetDotProduct(p_incomingRay.Direction, p_hitRecord.Normal) / p_incomingRay.Direction.GetLength();
            }

            var reflectionProbability = Utilities.RefractRay(p_incomingRay.Direction, outwardNormal, niOverNt, ref refracted) ? Utilities.SchlickApproximation(cosine, IndexOfRefraction) : 1.0d;

            p_scatteredRay = rng.NextDouble() < reflectionProbability
                ? new Ray(p_hitRecord.P, reflected + Roughness * Utilities.GetRandomPointInUnitSphere(), p_incomingRay.Time, p_incomingRay.Depth + 1) : new Ray(p_hitRecord.P, refracted + Roughness * Utilities.GetRandomPointInUnitSphere(), p_incomingRay.Time, p_incomingRay.Depth + 1);

            return true;
        }
    }
}
