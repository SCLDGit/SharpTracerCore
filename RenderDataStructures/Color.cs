using System;

namespace RenderDataStructures
{
    public class Color
    {
        public double R { get; private set; }
        public double G { get; private set; }
        public double B { get; private set; }

        public Color(double p_r, double p_g, double p_b)
        {
            R = p_r;
            G = p_g;
            B = p_b;
        }

        public void SetR(double p_r)
        {
            R = p_r;
        }
        public void SetG(double p_g)
        {
            G = p_g;
        }
        public void SetB(double p_b)
        {
            B = p_b;
        }
    }
}
