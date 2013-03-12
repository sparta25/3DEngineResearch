using System.Collections.Generic;

namespace TestFramework
{
    public static class ConvexSettingsEx
    {
        public static void FillIndices(this ConvexSettings settings)
        {
            settings.Indices = new List<int>();
            settings.Indices.AddRange(ConvexGenerator.GetGridFaces(settings.PartWidth, settings.PartHeight));
        }

        public static void FillPlanes(this ConvexSettings settings)
        {
            settings.Planes = new List<Plane>();
            for (int i = 0; i < settings.NumberOfPlanes; i++)
            {
                var plane = new Plane();
                var colors = ConvexGenerator.GetFaceColors(settings.PartWidth, settings.PartHeight);
                
                plane.Colors = colors;
                var quadrilateral = ConvexGenerator.GenerateRandomQuadrilateral(settings.BoundaryBox,
                                                                                settings.MinFractureSize,
                                                                                settings.MaxFractureSize);
                var vertices = ConvexGenerator.GetGridVertices(quadrilateral, settings.PartWidth, settings.PartHeight);
                plane.Vertices = new List<Point>();
                plane.Vertices.AddRange(vertices);
                settings.Planes.Add(plane);
            }
        }

    }
}