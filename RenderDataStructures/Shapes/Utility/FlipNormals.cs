using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;

namespace RenderDataStructures.Shapes
{
    public class FlipNormals : IHitTarget
    {
        public FlipNormals(IHitTarget p_originalTarget)
        {
            OriginalTarget = p_originalTarget;
        }
        private IHitTarget OriginalTarget { get; set; }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            if (!OriginalTarget.WasHit(p_ray, p_tMin, p_tMax, ref p_hitRecord)) return false;
            p_hitRecord.Normal = -p_hitRecord.Normal;
            p_hitRecord.P += p_hitRecord.Normal * 0.001d;
            return true;

        }

        public bool GenerateBoundingBox(double p_t1, double p_t2, ref BoundingBox p_bBox)
        {
            return OriginalTarget.GenerateBoundingBox(p_t1, p_t2, ref p_bBox);
        }
    }
}
