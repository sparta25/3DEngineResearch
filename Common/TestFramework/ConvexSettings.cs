using System;
using System.Collections.Generic;

namespace TestFramework
{
    [Serializable]
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

        public int NumberOfPlanes { get; set; }

        public int PartHeight { get; set; }

        public int PartWidth { get; set; }

        public float MinFractureSize { get; set; }

        public float MaxFractureSize { get; set; }

        public BoundaryBox BoundaryBox { get; set; }

        public List<int> Indices { get; set; }

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