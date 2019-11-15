namespace MathUtilities
{
    public class Ray
    {
        public Ray(Vec3 p_origin, Vec3 p_direction, int p_depth = 0)
        {
            Origin = p_origin;
            Direction = p_direction;
            Depth = p_depth;
        }

        public Vec3 Origin { get; }
        public Vec3 Direction { get; }
        public int Depth { get; }

        public Vec3 PointAt(double p_double)
        {
            return Origin + p_double * Direction;
        }
    }
}
