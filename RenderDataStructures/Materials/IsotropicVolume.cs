using System;
using System.Collections.Generic;
using System.Text;

using MathUtilities;

using RenderDataStructures.Shapes;

namespace RenderDataStructures.Materials
{
    public class IsotropicVolume : IVolumeMaterial
    {
        public IsotropicVolume(ITexture p_albedo)
        {
            Albedo = p_albedo;
        }

        private ITexture Albedo { get; set; }

        public bool ScatterRay(ref Ray p_incomingRay, ref HitRecord p_hitRecord, ref Color p_attenuation, ref Ray p_scatteredRay)
        {
            p_scatteredRay = new Ray(p_hitRecord.P, Utilities.GetRandomPointInUnitSphere(), p_incomingRay.Depth + 1);
            p_attenuation = Albedo.GetColor(p_hitRecord.U, p_hitRecord.V, p_hitRecord.P);
            return true;
        }
    }
}
