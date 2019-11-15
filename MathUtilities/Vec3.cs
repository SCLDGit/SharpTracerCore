namespace MathUtilities
{
    public class Vec3
    {
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Z { get; protected set; }
        public double R => X;
        public double G => Y;
        public double B => Z;

        public Vec3(double p_double)
        {
            X = p_double;
            Y = p_double;
            Z = p_double;
        }

        public Vec3(double p_x, double p_y, double p_z)
        {
            X = p_x;
            Y = p_y;
            Z = p_z;
        }

        protected Vec3()
        {
        }

        public double GetLength()
        {
            return System.Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double GetLengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public void MakeUnitVector()
        {
            var k = 1.0f / System.Math.Sqrt(X * X + Y * Y + Z * Z);
            X *= k;
            Y *= k;
            Z *= k;
        }

        public static Vec3 GetUnitVector(Vec3 p_v1)
        {
            return p_v1 / p_v1.GetLength();
        }

        public static double GetDotProduct(Vec3 p_v1, Vec3 p_v2)
        {
            return p_v1.X * p_v2.X + p_v1.Y * p_v2.Y + p_v1.Z * p_v2.Z;
        }

        public static Vec3 GetCrossProduct(Vec3 p_v1, Vec3 p_v2)
        {
            return new Vec3(p_v1.Y * p_v2.Z - p_v1.Z * p_v2.Y,
                            -(p_v1.X * p_v2.Z - p_v1.Z * p_v2.X),
                            p_v1.X * p_v2.Y - p_v1.Y * p_v2.X);
        }

        public static Vec3 GetReflection(Vec3 p_incomingVector, Vec3 p_normal)
        {
            return p_incomingVector - 2 * GetDotProduct(p_incomingVector, p_normal) * p_normal;
        }

        public static bool ShouldRefract(Vec3 p_vector, Vec3 p_normal, double p_niOverNt, ref Vec3 p_refracted)
        {
            var uv = GetUnitVector(p_vector);
            var dt = GetDotProduct(uv, p_normal);
            var discriminant = 1.0 - p_niOverNt * p_niOverNt * (1 - dt * dt);
            if (!(discriminant > 0)) return false;
            p_refracted = p_niOverNt * (uv - p_normal * dt) - p_normal * System.Math.Sqrt(discriminant);
            return true;
        }

        public static Vec3 operator -(Vec3 p_v1)
        {
            return new Vec3(-p_v1.X, -p_v1.Y, -p_v1.Z);
        }

        public static Vec3 operator +(Vec3 p_v1, Vec3 p_v2)
        {
            return new Vec3(p_v1.X + p_v2.X, p_v1.Y + p_v2.Y, p_v1.Z + p_v2.Z);
        }

        public static Vec3 operator -(Vec3 p_v1, Vec3 p_v2)
        {
            return new Vec3(p_v1.X - p_v2.X, p_v1.Y - p_v2.Y, p_v1.Z - p_v2.Z);
        }

        public static Vec3 operator *(Vec3 p_v1, Vec3 p_v2)
        {
            return new Vec3(p_v1.X * p_v2.X, p_v1.Y * p_v2.Y, p_v1.Z * p_v2.Z);
        }

        public static Vec3 operator /(Vec3 p_v1, Vec3 p_v2)
        {
            return new Vec3(p_v1.X / p_v2.X, p_v1.Y / p_v2.Y, p_v1.Z / p_v2.Z);
        }

        public static Vec3 operator *(Vec3 p_v1, double p_double)
        {
            return new Vec3(p_v1.X * p_double, p_v1.Y * p_double, p_v1.Z * p_double);
        }

        public static Vec3 operator *(double p_double, Vec3 p_v1)
        {
            return new Vec3(p_v1.X * p_double, p_v1.Y * p_double, p_v1.Z * p_double);
        }

        public static Vec3 operator /(Vec3 p_v1, double p_double)
        {
            return new Vec3(p_v1.X / p_double, p_v1.Y / p_double, p_v1.Z / p_double);
        }
    }
}
