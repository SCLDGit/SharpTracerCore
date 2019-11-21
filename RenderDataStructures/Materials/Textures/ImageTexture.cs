using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;

namespace RenderDataStructures.Materials.Textures
{
    public class ImageTexture : ITexture
    {
        private int X { get; }
        private int Y { get; }
        private byte[] Data { get; }

        public ImageTexture(byte[] p_pixelData, int p_x, int p_y)
        {
            Data = p_pixelData;
            X = p_x;
            Y = p_y;
        }

        public Color GetColor(double p_u, double p_v, Vec3 p_point)
        {
            var i = (int)(p_u * X);
            var j = (int)((1 - p_v) * Y - 0.001);
            if (i < 0) i = 0;
            if (j < 0) j = 0;
            if (i > X - 1) i = X - 1;
            if (j > Y - 1) j = Y - 1;
            var r = Data[3 * i + 3 * X * j] / 255.0;
            var g = Data[3 * i + 3 * X * j + 1] / 255.0;
            var b = Data[3 * i + 3 * X * j + 2] / 255.0;
            return new Color(r, g, b);
        }
    }
}
