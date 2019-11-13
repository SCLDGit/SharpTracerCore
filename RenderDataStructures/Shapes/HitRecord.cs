using RenderDataStructures.Basics;

namespace RenderDataStructures.Shapes
{
    public sealed class HitRecord
    {
        public double T { get; set; }
        public Vec3 P { get; set; }
        public Vec3 Normal { get; set; }
    }
}
