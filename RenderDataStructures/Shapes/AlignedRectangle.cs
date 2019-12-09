using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;
using RenderDataStructures.Materials;

namespace RenderDataStructures.Shapes
{
    public class XYAlignedRectangle : IHitTarget
    {
        private double X1 { get; }
        private double X2 { get; }
        private double Y1 { get; }
        private double Y2 { get; }
        private double K { get; }
        private IMaterial Material { get; }

        public XYAlignedRectangle(double p_x1, double p_x2, double p_y1, double p_y2, double p_k, IMaterial p_material)
        {
            X1 = p_x1;
            X2 = p_x2;
            Y1 = p_y1;
            Y2 = p_y2;
            K = p_k;
            Material = p_material;
        }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            var t = (K - p_ray.Origin.Z) / p_ray.Direction.Z;

            if (t < p_tMin || t > p_tMax)
            {
                return false;
            }

            var x = p_ray.Origin.X + t * p_ray.Direction.X;
            var y = p_ray.Origin.Y + t * p_ray.Direction.Y;

            if (x < X1 || x > X2 || y < Y1 || y > Y2)
            {
                return false;
            }

            p_hitRecord.U = (x - X1) / (X2 - X1);
            p_hitRecord.V = (y - Y1) / (Y2 - Y1);
            p_hitRecord.T = t;
            p_hitRecord.Material = Material;
            p_hitRecord.P = p_ray.PointAt(t);
            p_hitRecord.Normal = new Vec3(0, 0, 1);

            return true;
        }

        public bool GenerateBoundingBox(double p_time1, double p_time2, ref BoundingBox p_box)
        {
            p_box = new BoundingBox(new Vec3(X1, Y1, K - 0.0001), new Vec3(X2, Y2, K + 0.0001));
            return true;
        }
    }

    public class XZAlignedRectangle : IHitTarget
    {
        private double X1 { get; }
        private double X2 { get; }
        private double Z1 { get; }
        private double Z2 { get; }
        private double K { get; }
        private IMaterial Material { get; }

        public XZAlignedRectangle(double p_x1, double p_x2, double p_z1, double p_z2, double p_k, IMaterial p_material)
        {
            X1 = p_x1;
            X2 = p_x2;
            Z1 = p_z1;
            Z2 = p_z2;
            K = p_k;
            Material = p_material;
        }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            var t = (K - p_ray.Origin.Y) / p_ray.Direction.Y;

            if (t < p_tMin || t > p_tMax)
            {
                return false;
            }

            var x = p_ray.Origin.X + t * p_ray.Direction.X;
            var z = p_ray.Origin.Z + t * p_ray.Direction.Z;

            if (x < X1 || x > X2 || z < Z1 || z > Z2)
            {
                return false;
            }

            p_hitRecord.U = (x - X1) / (X2 - X1);
            p_hitRecord.V = (z - Z1) / (Z2 - Z1);
            p_hitRecord.T = t;
            p_hitRecord.Material = Material;
            p_hitRecord.P = p_ray.PointAt(t);
            p_hitRecord.Normal = new Vec3(0, 1, 0);

            return true;
        }

        public bool GenerateBoundingBox(double p_time1, double p_time2, ref BoundingBox p_box)
        {
            p_box = new BoundingBox(new Vec3(X1, K - 0.0001, Z1), new Vec3(X2, K + 0.0001, Z2));
            return true;
        }
    }

    public class YZAlignedRectangle : IHitTarget
    {
        private double Y1 { get; }
        private double Y2 { get; }
        private double Z1 { get; }
        private double Z2 { get; }
        private double K { get; }
        private IMaterial Material { get; }

        public YZAlignedRectangle(double p_y1, double p_y2, double p_z1, double p_z2, double p_k, IMaterial p_material)
        {
            Y1 = p_y1;
            Y2 = p_y2;
            Z1 = p_z1;
            Z2 = p_z2;
            K = p_k;
            Material = p_material;
        }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            var t = (K - p_ray.Origin.X) / p_ray.Direction.X;

            if (t < p_tMin || t > p_tMax)
            {
                return false;
            }

            var y = p_ray.Origin.Y + t * p_ray.Direction.Y;
            var z = p_ray.Origin.Z + t * p_ray.Direction.Z;

            if (y < Y1 || y > Y2 || z < Z1 || z > Z2)
            {
                return false;
            }

            p_hitRecord.U = (y - Y1) / (Y2 - Y1);
            p_hitRecord.V = (z - Z1) / (Z2 - Z1);
            p_hitRecord.T = t;
            p_hitRecord.Material = Material;
            p_hitRecord.P = p_ray.PointAt(t);
            p_hitRecord.Normal = new Vec3(1, 0, 0);

            return true;
        }

        public bool GenerateBoundingBox(double p_time1, double p_time2, ref BoundingBox p_box)
        {
            p_box = new BoundingBox(new Vec3(K - 0.0001, Y1, Z1), new Vec3(K + 0.0001, Y2, Z2));
            return true;
        }
    }
}
