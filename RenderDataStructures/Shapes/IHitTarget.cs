using MathUtilities;

namespace RenderDataStructures.Shapes
{
    public interface IHitTarget
    {
        bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord);
        bool GenerateBoundingBox(double p_t1, double p_t2, ref BoundingBox p_bBox);
    }
}
