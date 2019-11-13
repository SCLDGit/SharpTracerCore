using System.Collections.Generic;
using RenderDataStructures.Basics;

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

    }
}
