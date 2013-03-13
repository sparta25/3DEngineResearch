using System.Collections.Generic;

namespace TestFramework
{
    public interface IConvexSettings
    {
        int NumberOfPlanes { get; set; }

        int PartHeight { get; set; }

        int PartWidth { get; set; }

        float MinFractureSize { get; set; }

        float MaxFractureSize { get; set; }

        BoundaryBox BoundaryBox { get; set; }

        List<int> Indices { get; set; }

        List<Plane> Planes { get; set; }
    }
}