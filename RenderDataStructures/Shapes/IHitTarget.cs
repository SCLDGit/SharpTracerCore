using MathUtilities;

namespace RenderDataStructures.Shapes
{
    public interface IHitTarget
    {
        bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord);
    }
}
