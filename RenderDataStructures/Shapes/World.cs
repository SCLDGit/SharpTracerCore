using System.Collections.Generic;
using System.Linq;
using MathUtilities;

namespace RenderDataStructures.Shapes
{
    public sealed class World : IHitTarget

    {
        public World()
        {
            WorldHitTargets = new List<IHitTarget>();
        }

        public World(List<IHitTarget> p_worldHitTargets)
        {
            WorldHitTargets = p_worldHitTargets;
        }

        private List<IHitTarget> WorldHitTargets { get; }

        public void AddTarget(IHitTarget p_hitTarget)
        {
            WorldHitTargets.Add(p_hitTarget);
        }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            var tempHitRecord = new HitRecord();

            var hitAnything = false;

            var closestHitSoFar = p_tMax;

            foreach (var target in WorldHitTargets)
            {
                if (!target.WasHit(p_ray, p_tMin, closestHitSoFar, ref tempHitRecord)) continue;
                hitAnything = true;
                closestHitSoFar = tempHitRecord.T;
                p_hitRecord = tempHitRecord;
            }

            return hitAnything;
        }

        public bool GenerateBoundingBox(double p_time1, double p_time2, ref BoundingBox p_box)
        {
            if (WorldHitTargets.Count < 1) return false;
            var tempBox = new BoundingBox(new Vec3(0), new Vec3(0));
            var firstTrue = WorldHitTargets.First().GenerateBoundingBox(p_time1, p_time2, ref tempBox);
            if (!firstTrue)
            {
                return false;
            }

            p_box = tempBox;

            foreach (var target in WorldHitTargets)
            {
                if (target.GenerateBoundingBox(p_time1, p_time2, ref tempBox))
                {
                    p_box = BoundingBox.GetSurroundingBox(p_box, tempBox);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
