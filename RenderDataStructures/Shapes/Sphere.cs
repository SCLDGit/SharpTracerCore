using System;
using MathUtilities;
using RenderDataStructures.Materials;

namespace RenderDataStructures.Shapes
{
    public class Sphere : IHitTarget
    {
        public Sphere(Vec3 p_center, double p_radius, IMaterial p_material)
        {
            Center = p_center;
            Radius = p_radius;
            Material = p_material;
        }

        public Vec3 Center { get; set; }
        public double Radius { get; set; }
        public IMaterial Material { get; set; }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            // If the quadratic is confusing, remember that several 2s are pre-canceled out. - Comment by Matt Heimlich on 06/23/2019 @ 12:15:02
            var oc = p_ray.Origin - Center;
            var a = Vec3.GetDotProduct(p_ray.Direction, p_ray.Direction);
            var b = Vec3.GetDotProduct(oc, p_ray.Direction);
            var c = Vec3.GetDotProduct(oc, oc) - Radius * Radius;
            var discriminant = b * b - a * c;
            if (discriminant > 0)
            {
                var sqrtCache = Math.Sqrt(discriminant);
                var temp = (-b - sqrtCache) / a;
                if (temp < p_tMax && temp > p_tMin)
                {
                    p_hitRecord.T = temp;
                    p_hitRecord.P = p_ray.PointAt(p_hitRecord.T);
                    GetSphereUv((p_hitRecord.P - Center) / Radius, ref p_hitRecord);
                    p_hitRecord.Normal = (p_hitRecord.P - Center) / Radius;
                    p_hitRecord.Material = Material;
                    return true;
                }
                temp = (-b + sqrtCache) / a;
                if (temp < p_tMax && temp > p_tMin)
                {
                    p_hitRecord.T = temp;
                    p_hitRecord.P = p_ray.PointAt(p_hitRecord.T);
                    GetSphereUv((p_hitRecord.P - Center) / Radius, ref p_hitRecord);
                    p_hitRecord.Normal = (p_hitRecord.P - Center) / Radius;
                    p_hitRecord.Material = Material;
                    return true;
                }
            }

            return false;
        }

        public bool GenerateBoundingBox(double p_time1, double p_time2, ref BoundingBox p_box)
        {
            p_box = new BoundingBox(Center - new Vec3(Radius, Radius, Radius), Center + new Vec3(Radius, Radius, Radius));
            return true;
        }

        private static void GetSphereUv(Vec3 p_point, ref HitRecord p_hitRecord)
        {
            var phi = Math.Atan2(p_point.Z, p_point.X);
            var theta = Math.Asin(p_point.Y);
            p_hitRecord.U = 1 - (phi + Math.PI) / (2 * Math.PI);
            p_hitRecord.V = (theta + Math.PI / 2) / Math.PI;
        }
    }
}
