using MathUtilities;

namespace RenderDataStructures.Shapes
{
    public class Translate : IHitTarget
    {
        public Translate(IHitTarget p_originalTarget, Vec3 p_offset)
        {
            OriginalTarget = p_originalTarget;
            Offset = p_offset;
        }

        public IHitTarget OriginalTarget { get; set; }
        public Vec3 Offset { get; set; }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            var movedRay = new Ray(p_ray.Origin - Offset, p_ray.Direction, p_ray.Time);
            if (!OriginalTarget.WasHit(movedRay, p_tMin, p_tMax, ref p_hitRecord)) return false;
            p_hitRecord.P += Offset;
            return true;
        }

        public bool GenerateBoundingBox(double p_t1, double p_t2, ref BoundingBox p_bBox)
        {
            if (!OriginalTarget.GenerateBoundingBox(p_t1, p_t2, ref p_bBox)) return false;
            p_bBox = new BoundingBox(p_bBox.Min + Offset, p_bBox.Max + Offset);
            return true;
        }
    }
}
