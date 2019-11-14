using System;
using System.Collections.Generic;
using System.Text;
using RenderDataStructures.Basics;

namespace RenderDataStructures.Materials
{
    public static class MathUtilities
    {
        public static Random SyncRandom { get; set; } = new Random();

        public static Vec3 GetRandomPointInUnitSphere()
        {
            Vec3 p;

            do
            {
                p = 2.0 * new Vec3(SyncRandom.NextDouble(), SyncRandom.NextDouble(),
                        SyncRandom.NextDouble()) - new Vec3(1, 1, 1);
            } while (p.GetLengthSquared() >= 1.0);

            return p;
        }
    }
}
