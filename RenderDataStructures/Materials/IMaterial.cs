using System;
using System.Collections.Generic;
using System.Text;
using RenderDataStructures.Basics;
using RenderDataStructures.Shapes;

namespace RenderDataStructures.Materials
{
    public interface IMaterial
    {
        bool ScatterRay(ref Ray p_incomingRay, ref HitRecord p_hitRecord, ref Color p_attenuation, ref Ray p_scatteredRay);
    }
}
