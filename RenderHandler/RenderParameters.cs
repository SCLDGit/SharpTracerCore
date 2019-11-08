using System;
using System.Collections.Generic;
using System.Text;

namespace RenderHandler
{
    public class RenderParameters
    {
        public RenderParameters(int p_xResolution, int p_yResolution, string p_savePath, bool p_printRenderData = true)
        {
            XResolution = p_xResolution;
            YResolution = p_yResolution;

            PrintRenderData = p_printRenderData;

            SavePath = p_savePath;
        }

        public int XResolution { get; private set; }
        public int YResolution { get; private set; }
        public bool PrintRenderData { get; private set; }
        public string SavePath { get; private set; }
    }
}
