namespace RenderHandler
{
    public class RenderParameters
    {
        public RenderParameters(int p_xResolution, int p_yResolution, int p_numberOfSamples, int p_bounceDepth, bool p_parallel, double p_gammaCorrection, string p_savePath, bool p_printRenderData = true)
        {
            XResolution = p_xResolution;
            YResolution = p_yResolution;

            NumberOfSamples = p_numberOfSamples;

            BounceDepth = p_bounceDepth;

            Parallel = p_parallel;

            GammaCorrection = p_gammaCorrection;

            PrintRenderData = p_printRenderData;

            SavePath = p_savePath;
        }

        public int XResolution { get; private set; }
        public int YResolution { get; private set; }
        public int NumberOfSamples { get; private set; }
        public int BounceDepth { get; private set; }
        public bool Parallel { get; set; }
        public double GammaCorrection { get; private set; }
        public bool PrintRenderData { get; private set; }
        public string SavePath { get; private set; }
    }
}
