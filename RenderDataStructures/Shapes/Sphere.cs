using System;
using RenderDataStructures.Basics;

namespace RenderDataStructures.Shapes
{
    public class Sphere : IHitTarget
    {
        public Sphere(Vec3 p_center, double p_radius)
        {
            Center = p_center;
            Radius = p_radius;
        }

        public Vec3 Center { get; set; }
        public double Radius { get; set; }

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

                return true;
            }

            temp = (-b + Math.Sqrt(discriminant)) / a;

            if (temp < p_tMax && temp > p_tMin)
            {
                p_hitRecord.T = temp;
                p_hitRecord.P = p_ray.PointAt(p_hitRecord.T);
                p_hitRecord.Normal = (p_hitRecord.P - Center) / Radius;

                return true;
            }

            return false;
        }
    }
}
