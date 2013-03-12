using System;

namespace TestFramework
{
    [Serializable]
    public struct Quadrilateral
    {
        public Point TopLeft { get; set; }

        public Point TopRight { get; set; }

        public Point BottomLeft { get; set; }

        public Point BottomRight { get; set; }

    }
}