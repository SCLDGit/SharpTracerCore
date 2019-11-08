using System;
using System.Collections.Generic;
using System.Text;

namespace RenderDataStructures
{
    public class Ray
    {
        public Vec3 Origin { get; }
        public Vec3 Direction { get; }

        public Ray(Vec3 p_origin, Vec3 p_direction)
        {
            Origin = p_origin;
            Direction = p_direction;
        }

        public Vec3 PointAt(double p_double)
        {
            return Origin + p_double * Direction;
        }
    }
}
