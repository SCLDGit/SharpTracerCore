﻿using System;
using System.Collections.Generic;
using System.Text;
using RenderDataStructures.Basics;

namespace RenderDataStructures.Materials
{
    public static class MathUtilities
    {
        public static Random SyncRandom { get; set; } = new Random();

        public static Vec3 GetRandomPointInUnitSphere()
        {
            Vec3 p;

            do
            {
                p = 2.0 * new Vec3(SyncRandom.NextDouble(), SyncRandom.NextDouble(),
                        SyncRandom.NextDouble()) - new Vec3(1, 1, 1);
            } while (p.GetLengthSquared() >= 1.0);

            return p;
        }

        public static Vec3 ReflectRay(Vec3 p_vector, Vec3 p_normal)
        {
            return p_vector - 2 * Vec3.GetDotProduct(p_vector, p_normal) * p_normal;
        }

        public static bool RefractRay(Vec3 p_vector, Vec3 p_normal, double p_niOverNt, ref Vec3 p_refractedRay)
        {
            var unitVector = Vec3.GetUnitVector(p_vector);
            var dt = Vec3.GetDotProduct(unitVector, p_normal);
            var discriminant = 1.0 - p_niOverNt * p_niOverNt * (1 - dt * dt);
            if (!(discriminant > 0)) return false;
            p_refractedRay = p_niOverNt * (unitVector - p_normal * dt) - p_normal * Math.Sqrt(discriminant);
            return true;
        }

        public static double SchlickApproximation(double p_cosine, double p_indexOfRefraction)
        {
            var r0 = (1 - p_indexOfRefraction) / (1 + p_indexOfRefraction);
            r0 = r0 * r0;
            return r0 + (1 - r0) * Math.Pow((1 - p_cosine), 5);
        }
    }
}
