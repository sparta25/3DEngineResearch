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

        public float MinFractureSize{ get; set;}

        public float MaxFractureSize{ get; set;}

        public BoundaryBox BoundaryBox{ get; set;}
        
        public List<int> Indices { get; set; }
        
        public List<Plane> Planes { get; set; }
    }
}
