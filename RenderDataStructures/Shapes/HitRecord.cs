using MathUtilities;
using RenderDataStructures.Materials;

namespace RenderDataStructures.Shapes
{
    public struct HitRecord
    {
        public double T { get; set; }
        public double U { get; set; }
        public double V { get; set; }
        public Vec3 P { get; set; }
        public Vec3 Normal { get; set; }
        public IMaterial Material { get; set; }
    }
}
