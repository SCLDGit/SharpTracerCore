using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MathUtilities
{
    public static class RandomPool
    {
        public static ConcurrentDictionary<int, Random> RandomPoolLUT { get; set; } = new ConcurrentDictionary<int, Random>();
    }
}
