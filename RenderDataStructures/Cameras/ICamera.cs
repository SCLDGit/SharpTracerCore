using System;
using System.Collections.Generic;
using System.Text;
using RenderDataStructures.Basics;

namespace RenderDataStructures.Cameras
{
    public interface ICamera
    {
        Vec3 Origin { get; set; }
        Vec3 LowerLeftCorner { get; set; }
        Vec3 Horizontal { get; set; }
        Vec3 Vertical { get; set; }

        Ray GetRay(double p_u, double p_v);
    }
}
