using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;

namespace RenderDataStructures.Shapes.Utility
{
    public class RotateY : IHitTarget
    {
        public RotateY(IHitTarget p_originalTarget, double p_angle)
        {
            OriginalTarget = p_originalTarget;

            var radians = (Math.PI / 180.0d) * p_angle;

            SinTheta = Math.Sin(radians);
            CosTheta = Math.Cos(radians);

            m_boundingBox = new BoundingBox(new Vec3(0), new Vec3(0));

            HasBoundingBox = OriginalTarget.GenerateBoundingBox(0, 1, ref m_boundingBox);

            var min = new Vec3(double.MaxValue, double.MaxValue, double.MaxValue);
            var max = new Vec3(double.MinValue, double.MinValue, double.MinValue);

            for (var i = 0; i < 2; ++i)
            {
                for (var j = 0; j < 2; ++j)
                {
                    for (var k = 0; k < 2; ++k)
                    {
                        var x = i * m_boundingBox.Max.X + (1 - i) * m_boundingBox.Min.X;
                        var y = j * m_boundingBox.Max.Y + (1 - j) * m_boundingBox.Min.Y;
                        var z = k * m_boundingBox.Max.Z + (1 - k) * m_boundingBox.Min.Z;

                        var newX = CosTheta * x + SinTheta * z;
                        var newZ = -SinTheta * x + CosTheta * z;

                        var tester = new Vec3(newX, y, newZ);

                        if (tester.X > max.X) max.X = tester.X;
                        if (tester.X < min.X) min.X = tester.X;

                        if (tester.Y > max.Y) max.Y = tester.Y;
                        if (tester.Y < min.Y) min.Y = tester.Y;

                        if (tester.Z > max.Z) max.Z = tester.Z;
                        if (tester.Z < min.Z) min.Z = tester.Z;
                    }
                }
            }

            m_boundingBox = new BoundingBox(min, max);
        }

        public IHitTarget OriginalTarget { get; set; }
        public double SinTheta { get; set; }
        public double CosTheta { get; set; }
        public bool HasBoundingBox { get; set; }
        public BoundingBox m_boundingBox;

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            var origin = p_ray.Origin;
            var direction = p_ray.Direction;

            origin.X = CosTheta * p_ray.Origin.X - SinTheta * p_ray.Origin.Z;
            origin.Z = SinTheta * p_ray.Origin.X + CosTheta * p_ray.Origin.Z;

            direction.X = CosTheta * p_ray.Direction.X - SinTheta * p_ray.Direction.Z;
            direction.Z = SinTheta * p_ray.Direction.X + CosTheta * p_ray.Direction.Z;

            var rotatedRay = new Ray(origin, direction, p_ray.Time, p_ray.Depth);

            if (!OriginalTarget.WasHit(rotatedRay, p_tMin, p_tMax, ref p_hitRecord)) return false;
            var p = p_hitRecord.P;
            var normal = p_hitRecord.Normal;
            p.X = CosTheta * p_hitRecord.P.X + SinTheta * p_hitRecord.P.Z;
            p.Z = -SinTheta * p_hitRecord.P.X + CosTheta * p_hitRecord.P.Z;

            normal.X = CosTheta * p_hitRecord.Normal.X + SinTheta * p_hitRecord.Normal.Z;
            normal.Z = -SinTheta * p_hitRecord.Normal.X + CosTheta * p_hitRecord.Normal.Z;

            p_hitRecord.P = p;
            p_hitRecord.Normal = normal;

            return true;
        }

        public bool GenerateBoundingBox(double p_t1, double p_t2, ref BoundingBox p_bBox)
        {
            p_bBox = m_boundingBox;
            return HasBoundingBox;
        }
    }
}
