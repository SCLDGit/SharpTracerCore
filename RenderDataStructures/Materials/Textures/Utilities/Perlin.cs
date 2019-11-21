using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;

namespace RenderDataStructures.Materials.Textures.Utilities
{
    public class Perlin
    {
        private Vec3[] RandomVector { get; set; }
        private double[] RandomDouble { get; set; }
        private int[] PermutateX { get; set; }
        private int[] PermutateY { get; set; }
        private int[] PermutateZ { get; set; }

        public Perlin(int p_seed)
        {
            RandomVector = GenerateVec3Perlin(p_seed);
            RandomDouble = GenerateDoublePerlin(p_seed);
            PermutateX = GeneratePerlinPermutations(p_seed);
            PermutateY = GeneratePerlinPermutations(p_seed + 1);
            PermutateZ = GeneratePerlinPermutations(p_seed + 2);
        }

        public double GetNoise(Vec3 p_point)
        {
            var u = p_point.X - Math.Floor(p_point.X);
            var v = p_point.Y - Math.Floor(p_point.Y);
            var w = p_point.Z - Math.Floor(p_point.Z);

            var i = (int)(4 * p_point.X) & 255;
            var j = (int)(4 * p_point.Y) & 255;
            var k = (int)(4 * p_point.Z) & 255;
            return RandomDouble[PermutateX[i] ^ PermutateY[j] ^ PermutateZ[k]];
        }

        public double GetSmoothedNoise(Vec3 p_point)
        {
            var u = p_point.X - Math.Floor(p_point.X);
            var v = p_point.Y - Math.Floor(p_point.Y);
            var w = p_point.Z - Math.Floor(p_point.Z);

            var i = (int)Math.Floor(p_point.X);
            var j = (int)Math.Floor(p_point.Y);
            var k = (int)Math.Floor(p_point.Z);

            var c = new Vec3[2, 2, 2];

            for (var di = 0; di < 2; ++di)
            {
                for (var dj = 0; dj < 2; ++dj)
                {
                    for (var dk = 0; dk < 2; ++dk)
                    {
                        c[di, dj, dk] =
                            RandomVector
                                [PermutateX[(i + di) & 255] ^ PermutateY[(j + dj) & 255] ^ PermutateZ[(k + dk) & 255]];
                    }
                }
            }

            return PerlinInterpolate(c, u, v, w);
        }

        private static Vec3[] GenerateVec3Perlin(int p_seed)
        {
            var rng = new Random(p_seed);

            var p = new Vec3[256];
            for (var i = 0; i < 256; ++i)
            {
                p[i] = Vec3.GetUnitVector(new Vec3(-1 + 2 * rng.NextDouble(), -1 + 2 * rng.NextDouble(), -1 + 2 * rng.NextDouble()));
            }

            return p;
        }

        private static double[] GenerateDoublePerlin(int p_seed)
        {
            var rng = new Random(p_seed);

            var p = new double[256];
            for (var i = 0; i < 256; ++i)
            {
                p[i] = rng.NextDouble();
            }

            return p;
        }

        private static void Permute(ref int[] p_p, int p_n, int p_seed)
        {
            var rng = new Random(p_seed);

            for (var i = p_n - 1; i > 0; --i)
            {
                var target = (int)(rng.NextDouble() * (i + 1));
                var temp = p_p[i];
                p_p[i] = p_p[target];
                p_p[target] = temp;
            }
        }

        private static int[] GeneratePerlinPermutations(int p_seed)
        {
            var p = new int[256];
            for (var i = 0; i < 256; ++i)
            {
                p[i] = i;
            }
            Permute(ref p, 256, p_seed);
            return p;
        }

        private static double TrilinearInterpolate(double[,,] p_c, double p_u, double p_v, double p_w)
        {
            var accumulator = 0.0;
            for (var i = 0; i < 2; ++i)
            {
                for (var j = 0; j < 2; ++j)
                {
                    for (var k = 0; k < 2; ++k)
                    {
                        accumulator += (i * p_u + (1 - i) * (1 - p_u)) *
                                       (j * p_v + (1 - j) * (1 - p_v)) *
                                       (k * p_w + (1 - k) * (1 - p_w)) * p_c[i, j, k];
                    }
                }
            }

            return accumulator;
        }

        private static double PerlinInterpolate(Vec3[,,] p_c, double p_u, double p_v, double p_w)
        {
            var uu = p_u * p_u * (3 - 2 * p_u);
            var vv = p_v * p_v * (3 - 2 * p_v);
            var ww = p_w * p_w * (3 - 2 * p_w);

            var accumulator = 0.0;

            for (var i = 0; i < 2; ++i)
            {
                for (var j = 0; j < 2; ++j)
                {
                    for (var k = 0; k < 2; ++k)
                    {
                        var vWeight = new Vec3(p_u - i, p_v - j, p_w - k);
                        accumulator += (i * uu + (1 - i) * (1 - uu)) *
                                       (j * vv + (1 - j) * (1 - vv)) *
                                       (k * ww + (1 - k) * (1 - ww)) * Vec3.GetDotProduct(p_c[i, j, k], vWeight);
                    }
                }
            }

            return accumulator;
        }

        internal double GetTurbulentNoise(Vec3 p_point, int p_depth = 7)
        {
            var accumulator = 0.0;
            var tempPoint = p_point;
            var weight = 1.0;
            for (var i = 0; i < p_depth; ++i)
            {
                accumulator += weight * GetSmoothedNoise(tempPoint);
                weight *= 0.5;
                tempPoint *= 2;
            }

            return Math.Abs(accumulator);
        }
    }
}
