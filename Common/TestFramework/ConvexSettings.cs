using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TestFramework
{
    [Serializable]
    [DataContract]
    public class ConvexSettings : IConvexSettings
    {
        public ConvexSettings()
        {
        }

        public ConvexSettings(int numberOfPlanes, int partHeight, int partWidth, int minFractureSize,
                              int maxFractureSize)
        {
            NumberOfPlanes = numberOfPlanes;
            PartHeight = partHeight;
            PartWidth = partWidth;
            MinFractureSize = minFractureSize;
            MaxFractureSize = maxFractureSize;
        }

        [DataMember]
        public int NumberOfPlanes { get; set; }

        [DataMember]
        public int PartHeight { get; set; }

        [DataMember]
        public int PartWidth { get; set; }

        [DataMember]
        public float MinFractureSize { get; set; }

        [DataMember]
        public float MaxFractureSize { get; set; }

        [DataMember]
        public BoundaryBox BoundaryBox { get; set; }

        [DataMember]
        public List<int> Indices { get; set; }

        [DataMember]
        public List<Plane> Planes { get; set; }

        public Quadrilateral GetQuadrilateralByPosition(Plane plane, int positionX, int positionY )
        {
            var source = plane.Vertices.ToArray();
            
            int topLeft = ConvexGenerator.GetVertexIndex(PartWidth, positionX, positionY);
            int topRight = ConvexGenerator.GetVertexIndex(PartWidth, positionX + 1, positionY);
            int bottomRight = ConvexGenerator.GetVertexIndex(PartWidth, positionX + 1, positionY + 1);
            int bottomLeft = ConvexGenerator.GetVertexIndex(PartWidth, positionX, positionY + 1);
            
            return new Quadrilateral
                {
                    TopLeft = source[topLeft],
                    TopRight = source[topRight],
                    BottomRight = source[bottomRight],
                    BottomLeft = source[bottomLeft]
                };
        }
    }
}