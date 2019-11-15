using System;
using System.Collections.Generic;
using System.Text;

namespace RenderHandler
{
    public class RenderChunk
    {
        public RenderChunk(int p_startRow, int p_endRow)
        {
            StartRow = p_startRow;
            EndRow = p_endRow;
        }

        public int StartRow { get; set; }
        public int EndRow { get; set; }
    }
}
