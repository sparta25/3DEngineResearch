using System.Collections.Generic;

namespace ConvexHelper
{
    public interface IConvexSettings
    {
        int NumberOfPlanes { get; set; }

        int PartHeight { get; set; }

        int PartWidth { get; set; }

        float MinFractureSize { get; set; }

        float MaxFractureSize { get; set; }

        BoundaryBox BoundaryBox { get; set; }

        List<Color> Colors { get; set; }

        List<Point> Vertices { get; set; }

        List<int> Indices { get; set; }
    }
}
