using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;

namespace RenderDataStructures.Shapes
{
    public class BoundingBox
    {
        internal Vec3 Min { get; }
        internal Vec3 Max { get; }

        internal BoundingBox(Vec3 p_a, Vec3 p_b)
        {
            Min = p_a;
            Max = p_b;
        }

        public virtual bool WasHit(Ray p_ray, double p_tMin, double p_tMax)
        {
            // Original version. - Comment by Matt Heimlich on 06/24/2019 @ 12:59:28
            //var t0 = Math.GetMin((Min.X - p_ray.Origin.X) / p_ray.Direction.X,
            //                     (Max.X - p_ray.Origin.X) / p_ray.Direction.X);
            //var t1 = Math.GetMax((Min.X - p_ray.Origin.X) / p_ray.Direction.X,
            //                     (Max.X - p_ray.Origin.X) / p_ray.Direction.X);

            //p_tMin = Math.GetMax(t0, p_tMin);
            //p_tMax = Math.GetMin(t1, p_tMax);
            //if ( p_tMax <= p_tMin ) return false;
            //t0 = Math.GetMin((Min.Y - p_ray.Origin.Y) / p_ray.Direction.Y,
            //                     (Max.Y - p_ray.Origin.Y) / p_ray.Direction.Y);
            //t1 = Math.GetMax((Min.Y - p_ray.Origin.Y) / p_ray.Direction.Y,
            //                     (Max.Y - p_ray.Origin.Y) / p_ray.Direction.Y);

            //p_tMin = Math.GetMax(t0, p_tMin);
            //p_tMax = Math.GetMin(t1, p_tMax);
            //if (p_tMax <= p_tMin) return false;
            //t0 = Math.GetMin((Min.Z - p_ray.Origin.Z) / p_ray.Direction.Z,
            //                     (Max.Z - p_ray.Origin.Z) / p_ray.Direction.Z);
            //t1 = Math.GetMax((Min.Z - p_ray.Origin.Z) / p_ray.Direction.Z,
            //                     (Max.Z - p_ray.Origin.Z) / p_ray.Direction.Z);

            //p_tMin = Math.GetMax(t0, p_tMin);
            //p_tMax = Math.GetMin(t1, p_tMax);
            //if (p_tMax <= p_tMin) return false;
            //return true;

            var invD = 1.0d / p_ray.Direction.X;
            var t0 = (Min.X - p_ray.Origin.X) * invD;
            var t1 = (Max.X - p_ray.Origin.X) * invD;
            if (invD < 0.0)
            {
                Swap(ref t0, ref t1);
            }

            p_tMin = t0 > p_tMin ? t0 : p_tMin;
            p_tMax = t1 < p_tMax ? t1 : p_tMax;

            if (p_tMax <= p_tMin) return false;

            invD = 1.0d / p_ray.Direction.Y;
            t0 = (Min.Y - p_ray.Origin.Y) * invD;
            t1 = (Max.Y - p_ray.Origin.Y) * invD;
            if (invD < 0.0)
            {
                Swap(ref t0, ref t1);
            }

            p_tMin = t0 > p_tMin ? t0 : p_tMin;
            p_tMax = t1 < p_tMax ? t1 : p_tMax;

            if (p_tMax <= p_tMin) return false;

            invD = 1.0d / p_ray.Direction.Z;
            t0 = (Min.Z - p_ray.Origin.Z) * invD;
            t1 = (Max.Z - p_ray.Origin.Z) * invD;
            if (invD < 0.0)
            {
                Swap(ref t0, ref t1);
            }

            p_tMin = t0 > p_tMin ? t0 : p_tMin;
            p_tMax = t1 < p_tMax ? t1 : p_tMax;

            return !(p_tMax <= p_tMin);
        }

        private static void Swap<T>(ref T p_left, ref T p_right)
        {
            var temp = p_left;
            p_left = p_right;
            p_right = temp;
        }

        public static BoundingBox GetSurroundingBox(BoundingBox p_box1, BoundingBox p_box2)
        {
            var small = new Vec3(Utilities.GetMin(p_box1.Min.X, p_box2.Min.X),
                Utilities.GetMin(p_box1.Min.Y, p_box2.Min.Y),
                Utilities.GetMin(p_box1.Min.Z, p_box2.Min.Z));

            var big = new Vec3(Utilities.GetMax(p_box1.Max.X, p_box2.Max.X),
                Utilities.GetMax(p_box1.Max.Y, p_box2.Max.Y),
                Utilities.GetMax(p_box1.Max.Z, p_box2.Max.Z));

            return new BoundingBox(small, big);
        }
    }
}
