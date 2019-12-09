using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

using MathUtilities;

using RenderDataStructures.Materials;

namespace RenderDataStructures.Shapes
{
    public class ConstantMedium : IHitTarget
    {
        public ConstantMedium(IHitTarget p_boundary, double p_density, IVolumeMaterial p_volumeMaterial)
        {
            Boundary = p_boundary;
            Density = p_density;
            PhaseFunction = p_volumeMaterial;
        }

        public IHitTarget Boundary { get; set; }
        public double Density { get; set; }
        public IVolumeMaterial PhaseFunction { get; set; }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            var rng = RandomPool.RandomPoolLUT[Thread.CurrentThread.ManagedThreadId];

            var debugging = false && rng.NextDouble() < 0.00001;

            var hitRecord1 = new HitRecord();
            var hitRecord2 = new HitRecord();

            if ( !Boundary.WasHit(p_ray, double.MinValue, double.MaxValue, ref hitRecord1) ) return false;
            if ( !Boundary.WasHit(p_ray, hitRecord1.T + 0.0001, double.MaxValue, ref hitRecord2) ) return false;

            if ( debugging )
            {
                Debug.WriteLine($@"t1: {hitRecord1.T}");
                Debug.WriteLine($@"t2: {hitRecord2.T}{Environment.NewLine}");
            }

            if ( hitRecord1.T < p_tMin )
            {
                hitRecord1.T = p_tMin;
            }

            if ( hitRecord2.T > p_tMax )
            {
                hitRecord2.T = p_tMax;
            }

            if ( hitRecord1.T >= hitRecord2.T )
            {
                return false;
            }

            if ( hitRecord1.T < 0 )
            {
                hitRecord1.T = 0;
            }

            var distanceInsideOfVolumeBoundary = (hitRecord2.T - hitRecord1.T) * p_ray.Direction.GetLength();
            var hitDistance                    = -(1 / Density) * Math.Log(rng.NextDouble());

            if ( !(hitDistance < distanceInsideOfVolumeBoundary) ) return false;

            p_hitRecord.T = hitRecord1.T + hitDistance / p_ray.Direction.GetLength();
            p_hitRecord.P = p_ray.PointAt(p_hitRecord.T);

            if ( debugging )
            {
                Debug.WriteLine($@"Hit Distance: {hitDistance}");
                Debug.WriteLine($@"HitRecord.T: {p_hitRecord.T}");
                Debug.WriteLine($@"HitRecord.P: {p_hitRecord.P}{Environment.NewLine}");
            }

            // This is an arbitrary normal.
            p_hitRecord.Normal   = new Vec3(1, 0, 0);
            p_hitRecord.Material = PhaseFunction;

            return true;

        }

        public bool GenerateBoundingBox(double p_t1, double p_t2, ref BoundingBox p_bBox)
        {
            return Boundary.GenerateBoundingBox(p_t1, p_t2, ref p_bBox);
        }
    }
}
