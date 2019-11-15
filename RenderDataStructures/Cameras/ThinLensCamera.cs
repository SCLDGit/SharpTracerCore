using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;

namespace RenderDataStructures.Cameras
{
    public class ThinLensCamera : ICamera
    {
        public ThinLensCamera(Vec3 p_lookFrom, Vec3 p_lookAt, Vec3 p_upVector, double p_verticalFieldOfView, double p_aspectRatio, double p_aperture, double p_focalLength)
        {
            LensRadius = p_aperture / 2;
            Origin = p_lookFrom;

            var theta = p_verticalFieldOfView * Math.PI / 180;
            var halfHeight = Math.Tan(theta / 2);
            var halfWidth = p_aspectRatio * halfHeight;

            W = Vec3.GetUnitVector(p_lookFrom - p_lookAt);
            U = Vec3.GetUnitVector(Vec3.GetCrossProduct(p_upVector, W));
            V = Vec3.GetCrossProduct(W, U);

            LowerLeftCorner = Origin - halfWidth * p_focalLength * U - halfHeight * p_focalLength * V - p_focalLength * W;
            Horizontal = 2 * halfWidth * p_focalLength * U;
            Vertical = 2 * halfHeight * p_focalLength * V;
        }

        public Vec3 Origin { get; set; }
        public Vec3 LowerLeftCorner { get; set; }
        public Vec3 Horizontal { get; set; }
        public Vec3 Vertical { get; set; }
        public Vec3 U { get; set; }
        public Vec3 V { get; set; }
        public Vec3 W { get; set; }
        private double LensRadius { get; set; }

        public Ray GetRay(double p_s, double p_t)
        {
            var rd = LensRadius * Utilities.GetRandomPointInUnitDisk();
            var offset = U * rd.X + V * rd.Y;
            return new Ray(Origin + offset, LowerLeftCorner + p_s * Horizontal + p_t * Vertical - Origin - offset);
        }
    }
}
