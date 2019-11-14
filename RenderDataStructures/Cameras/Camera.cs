using System;
using System.Collections.Generic;
using System.Text;
using RenderDataStructures.Basics;

namespace RenderDataStructures.Cameras
{
    public class Camera : ICamera
    {
        public Camera(Vec3 p_lookFrom, Vec3 p_lookAt, Vec3 p_upVector, double p_verticalFieldOfView, double p_aspectRatio)
        {
            Origin = p_lookFrom;

            var theta = p_verticalFieldOfView * Math.PI / 180;
            var halfHeight = Math.Tan(theta / 2);
            var halfWidth = p_aspectRatio * halfHeight;

            var w = Vec3.GetUnitVector(p_lookFrom - p_lookAt);
            var u = Vec3.GetUnitVector(Vec3.GetCrossProduct(p_upVector, w));
            var v = Vec3.GetCrossProduct(w, u);

            LowerLeftCorner = Origin - halfWidth * u - halfHeight * v - w;
            Horizontal = 2 * halfWidth * u;
            Vertical = 2 * halfHeight * v;
        }

        public Vec3 Origin { get; set; }
        public Vec3 LowerLeftCorner { get; set; }
        public Vec3 Horizontal { get; set; }
        public Vec3 Vertical { get; set; }
        public Ray GetRay(double p_s, double p_t)
        {
            return new Ray(Origin, LowerLeftCorner + p_s * Horizontal + p_t * Vertical - Origin);
        }
    }
}
