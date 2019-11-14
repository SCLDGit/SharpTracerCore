using System;
using System.Collections.Generic;
using System.Text;
using RenderDataStructures.Basics;

namespace RenderDataStructures.Cameras
{
    public class Camera : ICamera
    {
        public Camera()
        {
            LowerLeftCorner = new Vec3(-2, -1, -1);
            Horizontal = new Vec3(4, 0, 0);
            Vertical = new Vec3(0, 2, 0);
            Origin = new Vec3(0, 0, 0);
        }

        public Vec3 Origin { get; set; }
        public Vec3 LowerLeftCorner { get; set; }
        public Vec3 Horizontal { get; set; }
        public Vec3 Vertical { get; set; }
        public Ray GetRay(double p_u, double p_v)
        {
            return new Ray(Origin, LowerLeftCorner + p_u * Horizontal + p_v * Vertical - Origin);
        }
    }
}
