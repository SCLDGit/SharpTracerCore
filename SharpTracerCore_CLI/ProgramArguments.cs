namespace SharpTracerCore_CLI
{
    class ProgramArguments
    {
        public int XResolution { get; set; }
        public int YResolution { get; set; }
        public int NumberOfSamples { get; set; }
        public int BounceDepth { get; set; }
        public bool Parallel { get; set; }
        public double GammaCorrection { get; set; }
        public string SavePath { get; set; }
    }
}
