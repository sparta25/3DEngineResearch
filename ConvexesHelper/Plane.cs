using System;
using System.Collections.Generic;

namespace ConvexHelper
{
    [Serializable]
    public class Plane
    {
        public List<Color> Colors { get; set; }

        public List<Point> Vertices { get; set; }
    }
}