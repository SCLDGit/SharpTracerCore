using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;

namespace RenderDataStructures.Materials
{
    public interface ITexture
    {
        Color GetColor(double p_u, double p_v, Vec3 p_point);
    }
}
