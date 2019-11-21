using System;
using MathUtilities;
using RenderDataStructures.Materials.Textures.Utilities;

namespace RenderDataStructures.Materials
{
    public enum NoiseTypes
    {
        REGULAR,
        SMOOTHED,
        TURBULENCE,
        MARBLE
    }

    public class NoiseTexture : ITexture
    {
        internal Color Color { get; }
        internal double Scale { get; }
        private Perlin Noise { get; }
        internal NoiseTypes NoiseType { get; }
        internal double MultiplyStrength { get; }
        internal double DistortionPower { get; }

        public NoiseTexture(Color p_color, double p_scale, NoiseTypes p_noiseType, double p_multiplyStrength = 0.5, double p_distortionPower = 1.0, int p_seed = 0)
        {
            Color = p_color;
            Scale = p_scale;
            NoiseType = p_noiseType;
            MultiplyStrength = p_multiplyStrength;
            DistortionPower = p_distortionPower;
            Noise = new Perlin(p_seed);
        }

        public Color GetColor(double p_u, double p_v, Vec3 p_point)
        {
            return NoiseType switch
            {
                NoiseTypes.REGULAR => (Color * MultiplyStrength * (DistortionPower * Noise.GetNoise(p_point * Scale))),
                NoiseTypes.SMOOTHED => (Color * MultiplyStrength *
                                        (1 + DistortionPower * Noise.GetSmoothedNoise(p_point * Scale))),
                NoiseTypes.TURBULENCE => (Color * MultiplyStrength *
                                          (DistortionPower * Noise.GetTurbulentNoise(p_point * Scale))),
                NoiseTypes.MARBLE => (Color * MultiplyStrength *
                                      (1 + Math.Sin(Scale * p_point.Z +
                                                    DistortionPower * Noise.GetTurbulentNoise(p_point)))),
                _ => (Color * MultiplyStrength * (DistortionPower * Noise.GetNoise(p_point * Scale)))
            };
        }
    }
}
