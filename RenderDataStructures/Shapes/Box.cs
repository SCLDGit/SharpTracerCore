using System;
using System.Collections.Generic;
using System.Text;
using MathUtilities;
using RenderDataStructures.Materials;

namespace RenderDataStructures.Shapes
{
    public class Box : IHitTarget
    {
        public Box(Vec3 p_pMin, Vec3 p_pMax, IMaterial p_material)
        {
            PMin = p_pMin;
            PMax = p_pMax;

            Parts = new HitTargetCollection();

            Parts.AddTarget(new XYAlignedRectangle(PMin.X, PMax.X, PMin.Y, PMax.Y, PMax.Z, p_material));
            Parts.AddTarget(new FlipNormals(new XYAlignedRectangle(PMin.X, PMax.X, PMin.Y, PMax.Y, PMin.Z, p_material)));
            Parts.AddTarget(new XZAlignedRectangle(PMin.X, PMax.X, PMin.Z, PMax.Z, PMax.Y, p_material));
            Parts.AddTarget(new FlipNormals(new XZAlignedRectangle(PMin.X, PMax.X, PMin.Z, PMax.Z, PMin.Y, p_material)));
            Parts.AddTarget(new YZAlignedRectangle(PMin.Y, PMax.Y, PMin.Z, PMax.Z, PMax.X, p_material));
            Parts.AddTarget(new FlipNormals(new YZAlignedRectangle(PMin.Y, PMax.Y, PMin.Z, PMax.Z, PMin.X, p_material)));
        }

        public Vec3 PMin { get; set; }
        public Vec3 PMax { get; set; }
        public HitTargetCollection Parts { get; set; }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            return Parts.WasHit(p_ray, p_tMin, p_tMax, ref p_hitRecord);
        }

        public bool GenerateBoundingBox(double p_t1, double p_t2, ref BoundingBox p_bBox)
        {
            p_bBox = new BoundingBox(PMin, PMax);
            return true;
        }
    }
}
