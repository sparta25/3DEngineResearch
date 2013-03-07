using System;
using System.Collections.Generic;

namespace ConvexHelper
{
    [Serializable]
    public class ConvexSettings : IConvexSettings
    {
        public ConvexSettings(){}

        public ConvexSettings(int numberOfPlanes, int partHeight, int partWidth, int minFractureSize, int maxFractureSize)
        {
            NumberOfPlanes = numberOfPlanes;
            PartHeight = partHeight;
            PartWidth = partWidth;
            MinFractureSize = minFractureSize;
            MaxFractureSize = maxFractureSize;
        }

        public int NumberOfPlanes { get; set;}

        public int PartHeight{ get; set;}

        public int PartWidth { get; set; }

        public int MinFractureSize{ get; set;}

        public int MaxFractureSize{ get; set;}

        public BoundaryBox BoundaryBox{ get; set;}

        public List<Color> Colors{ get; set;}

        public List<Point> Vertices { get; set; }

        public List<int> Indices { get; set; }
    }
}
