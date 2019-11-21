﻿using MathUtilities;

namespace RenderDataStructures.Materials
{
    public class ConstantTexture : ITexture
    {
        private Color Color { get; }

        public ConstantTexture(Color p_color)
        {
            Color = p_color;
        }

        public Color GetColor(double p_u, double p_v, Vec3 p_point)
        {
            return Color;
        }
    }
}
