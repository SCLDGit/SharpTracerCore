using RenderDataStructures.Basics;
using RenderDataStructures.Materials;

namespace RenderDataStructures.Shapes
{
    public sealed class HitRecord
    {
        public double T { get; set; }
        public Vec3 P { get; set; }
        public Vec3 Normal { get; set; }
        public IMaterial Material { get; set; }
    }
}
