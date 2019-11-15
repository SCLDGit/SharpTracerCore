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
            var objectCenter = p_ray.Origin - Center;

            var a = Vec3.GetDotProduct(p_ray.Direction, p_ray.Direction);
            var b = Vec3.GetDotProduct(objectCenter, p_ray.Direction);
            var c = Vec3.GetDotProduct(objectCenter, objectCenter) - Radius * Radius;

            var discriminant = b * b - a * c;

            if (!(discriminant > 0)) return false;

            var temp = (-b - Math.Sqrt(discriminant)) / a;

            if (temp < p_tMax && temp > p_tMin)
            {
                p_hitRecord.T = temp;
                p_hitRecord.P = p_ray.PointAt(p_hitRecord.T);
                p_hitRecord.Normal = (p_hitRecord.P - Center) / Radius;
                p_hitRecord.Material = Material;

                return true;
            }

            temp = (-b + Math.Sqrt(discriminant)) / a;

            if (temp < p_tMax && temp > p_tMin)
            {
                p_hitRecord.T = temp;
                p_hitRecord.P = p_ray.PointAt(p_hitRecord.T);
                p_hitRecord.Normal = (p_hitRecord.P - Center) / Radius;
                p_hitRecord.Material = Material;

                return true;
            }

            return false;
        }
    }
}
